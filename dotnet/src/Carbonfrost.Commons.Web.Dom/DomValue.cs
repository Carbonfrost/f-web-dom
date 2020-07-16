//
// Copyright 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomValue : IDomValue {

        public virtual bool IsReadOnly {
            get {
                return false;
            }
        }

        public virtual string Value {
            get;
            set;
        }

        object IDomValue.TypedValue {
            get {
                return Value;
            }
            set {
                Value = Utility.ConvertToString(value);
            }
        }

        public virtual void AppendValue(object value) {
            Value += value;
        }

        IDomValue IDomValue.Clone() {
            return Clone();
        }

        object IDomObjectReferenceLifecycle.Clone() {
            return Clone();
        }

        public virtual DomValue Clone() {
            return new DomValue { Value = Value };
        }

        protected virtual void Attaching(DomAttribute attribute) {
        }

        protected virtual void Detaching() {
        }

        void IDomObjectReferenceLifecycle.Attaching(DomObject instance) {
            Attaching((DomAttribute) instance);
        }

        void IDomObjectReferenceLifecycle.Detaching() {
            Detaching();
        }
    }
}

