//
// Copyright 2014, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using Carbonfrost.Commons.Core.Runtime;
using System.Text.RegularExpressions;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomValue {

        internal static Impl<T> CreateImpl<T>() {
            if (typeof(T) == typeof(Uri)) {
                return (Impl<T>) (object) new UriImpl();
            }
            if (typeof(T) == typeof(string)) {
                return new BasicImpl<T>();
            }

            // If the user implements DomValue<T> where T is a collection type, we use
            // add method to do the work of appending a value
            var collectionType = typeof(T).GetInterface(typeof(ICollection<>).FullName);
            if (collectionType != null) {
                return CreateCollectionImpl<T>(collectionType);
            }

            return new BasicImpl<T>();
        }

        internal static Impl<T> CreateUserDefinedImpl<T>(Func<string, T> convert, Func<T, string> convertBack) {
            return new UserDefinedImpl<T>(convert, convertBack);
        }

        private static Impl<T> CreateCollectionImpl<T>(Type collectionType) {
            var collection = typeof(T).GetInterfaceMap(collectionType);
            var index = Array.FindIndex(collection.InterfaceMethods, m => m.Name == "Add");
            var addMethod = collection.TargetMethods[index];
            return new ListImpl<T>(addMethod);
        }

        internal static Impl<T> CreateImpl<T>(T value) {
            var result = CreateImpl<T>();
            result.Value = value;
            return result;
        }

        internal abstract class Impl<T> {
            public abstract string Text { get; set; }
            public abstract T Value { get; set; }

            public virtual void AppendValue(object value) {
                Text += value;
            }

            public virtual Impl<T> Clone() {
                return (Impl<T>) MemberwiseClone();
            }

            public virtual void SetAttribute(DomAttribute attribute) {
            }
        }

        internal class UriImpl : Impl<Uri> {

            private DomAttribute _attribute;
            private Uri _value;

            public override Uri Value {
                get {
                    return new Uri(BaseUri, _value);
                }
                set {
                    _value = value;
                }
            }

            public override string Text {
                get {
                    return _value.ToString();
                }
                set {
                    Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out _value);
                }
            }

            private Uri BaseUri {
                get {
                    if (_attribute == null) {
                        return null;
                    }
                    return _attribute.BaseUri;
                }
            }

            public override void SetAttribute(DomAttribute attribute) {
                _attribute = attribute;
            }
        }

        class BasicImpl<T> : Impl<T> {
            private T _valueCache;
            private string _textCache;

            public override T Value {
                get {
                    return _valueCache;
                }
                set {
                    _valueCache = value;
                    _textCache = ConvertBack(value);
                }
            }

            protected virtual string ConvertBack(T value) {
                return System.Convert.ToString(value);
            }

            protected virtual T Convert(string text) {
                return Activation.FromText<T>(text);
            }

            public override string Text {
                get {
                    if (_textCache == null) {
                        _textCache = ConvertBack(Value);
                    }
                    return _textCache;
                }
                set {
                    _textCache = value;
                    _valueCache = Convert(value);
                }
            }
        }

        class UserDefinedImpl<T> : BasicImpl<T> {

            private readonly Func<string, T> _convert;
            private readonly Func<T, string> _convertBack;

            public UserDefinedImpl(Func<string, T> convert, Func<T, string> convertBack) {
                _convert = convert;
                _convertBack = convertBack;
            }

            protected override string ConvertBack(T value) {
                return _convertBack(value);
            }

            protected override T Convert(string text) {
                return _convert(text);
            }
        }

        class ListImpl<TCollection> : BasicImpl<TCollection> {
            private readonly MethodInfo _addMethod;
            private readonly Type _itemType;

            public ListImpl(MethodInfo addMethod) {
                _addMethod = addMethod;
                _itemType = _addMethod.GetParameters()[0].ParameterType;
            }

            protected override string ConvertBack(TCollection value) {
                IEnumerable<object> values = ((IEnumerable) value).Cast<object>();
                return string.Join(" ", values);
            }

            protected override TCollection Convert(string text) {
                var result = (TCollection) Activator.CreateInstance(typeof(TCollection));
                foreach (var item in Regex.Split(text, @"\s+").Select(i => Activation.FromText(_itemType, i))) {
                    _addMethod.Invoke(result, new [] { item });
                }

                return result;
            }

            public override void AppendValue(object value) {
                _addMethod.Invoke(Value, new [] { value });
            }

        }
    }

}
