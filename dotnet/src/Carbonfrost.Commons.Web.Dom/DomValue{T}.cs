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

using System.Linq;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    public class DomValue<T> : IDomValue, IDomValue<T> {

        private readonly DomValue.Impl<T> _impl;

        public T TypedValue {
            get {
                return _impl.Value;
            }
            set {
                _impl.Value = value;
            }
        }

        private bool HasOverriddenConversion {
            get {
                var type = GetType();
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DomValue<>)) {
                    return false;
                }
                var overrides = new [] {
                    type.GetMethod("Convert"),
                    type.GetMethod("ConvertBack"),
                };
                return overrides.Any(o => o != null);
            }
        }

        protected virtual void Initialize(DomAttribute attribute) {
            _impl.Initialize(attribute);
        }

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
                return _impl.Text;
            }
            set {
                _impl.Text = value;
            }
        }

        object IDomValue.TypedValue {
            get {
                return TypedValue;
            }
            set {
                TypedValue = (T) value;
            }
        }

        public DomValue() {
            if (HasOverriddenConversion) {
                _impl = DomValue.CreateUserDefinedImpl(Convert, ConvertBack);
            } else {
                _impl = DomValue.CreateImpl<T>();
            }
        }

        public virtual void AppendValue(object value) {
            _impl.AppendValue(value);
        }

        public virtual DomValue<T> Clone() {
            return (DomValue<T>) MemberwiseClone();
        }

        IDomValue IDomValue.Clone() {
            return Clone();
        }
    }
}

