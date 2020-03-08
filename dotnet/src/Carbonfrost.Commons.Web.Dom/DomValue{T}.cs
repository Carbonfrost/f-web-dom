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

using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    public class DomValue<T> : IDomValue {

        private T _valueCache;
        private string _textCache;
        private Action<object> _appendValueThunk;

        public T TypedValue {
            get {
                return _valueCache;
            }
            set {
                _valueCache = value;
                _textCache = ConvertBack(value);
            }
        }

        protected virtual void Initialize(DomAttribute attribute) {}

        void IDomValue.Initialize(DomAttribute attribute) {
            Initialize(attribute);
        }

        protected virtual string ConvertBack(T value) {
            return value.ToString();
        }

        protected virtual T Convert(string text) {
            return Activation.FromText<T>(text);
        }

        public virtual bool IsReadOnly {
            get {
                return false;
            }
        }

        public string Value {
            get {
                if (_textCache == null) {
                    _textCache = ConvertBack(TypedValue);
                }
                return _textCache;
            }
            set {
                _textCache = value;
                _valueCache = Convert(value);
            }
        }

        public virtual void AppendValue(object value) {
            if (_appendValueThunk == null) {
                _appendValueThunk = AppendValueThunk();
            }
            _appendValueThunk(value);
        }

        public virtual DomValue<T> Clone() {
            return (DomValue<T>) MemberwiseClone();
        }

        IDomValue IDomValue.Clone() {
            return Clone();
        }

        private Action<object> AppendValueThunk() {
            // If the user implements DomValue<T> where T is a collection type, we use
            // add method to do the work of appending a value
            var collectionType = typeof(T).GetInterface(typeof(ICollection<>).FullName);
            if (collectionType != null) {
                var collection = typeof(T).GetInterfaceMap(collectionType);
                var index = Array.FindIndex(collection.InterfaceMethods, m => m.Name == "Add");
                var addMethod = collection.TargetMethods[index];
                return (value) => addMethod.Invoke(TypedValue, new [] { value });
            }

            return (value) => Value += value;
        }
    }
}

