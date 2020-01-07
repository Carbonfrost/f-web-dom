//
// - IdDomValue.cs -
//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;

namespace Carbonfrost.Commons.Web.Dom {

    class IdDomValue : IDomValue {

        private string _value;
        private DomAttribute _attribute;

        public object Clone() {
            return MemberwiseClone();
        }

        public bool IsReadOnly {
            get {
                return false;
            }
        }

        public string Value {
            get {
                return _value;
            }
            set {
                if (_value != value) {
                    _value = value;
                    _attribute.OwnerDocument.UpdateElementIndex(_value, _attribute.OwnerElement);
                }
            }
        }

        void IDomValue.Initialize(DomAttribute attribute) {
            _attribute = attribute;
        }

    }
}
