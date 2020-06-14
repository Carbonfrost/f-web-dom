//
// Copyright 2013, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

namespace Carbonfrost.Commons.Web.Dom {

    public class DomAttributeDefinition : DomNodeDefinition {

        private Type _valueType;
        private Type _attributeNodeType;

        public Type ValueType {
            get {
                return _valueType;
            }
            set {
                ThrowIfReadOnly();
                _valueType = value;
            }
        }

        public Type AttributeNodeType {
            get {
                return _attributeNodeType;
            }
            set {
                ThrowIfReadOnly();
                _attributeNodeType = value;
            }
        }

        public DomAttributeDefinition(DomName name) : base(name) {
        }

        public new DomAttributeDefinition Clone() {
            return (DomAttributeDefinition) base.Clone();
        }

        protected override DomNodeDefinition CloneCore() {
            return new DomAttributeDefinition(Name) {
                ValueType = ValueType,
                AttributeNodeType = AttributeNodeType,
            };
        }
    }
}
