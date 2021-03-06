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

    public abstract class DomContainerDefinition : DomNodeDefinition {

        private readonly DomElementDefinitionCollection _elementDefinitions;
        private readonly DomAttributeDefinitionCollection _attributeDefinitions;

        public DomAttributeDefinitionCollection AttributeDefinitions {
            get {
                return _attributeDefinitions;
            }
        }

        public DomElementDefinitionCollection ElementDefinitions {
            get {
                return _elementDefinitions;
            }
        }

        protected DomContainerDefinition(DomName name) : base(name) {
            _elementDefinitions = new DomElementDefinitionCollection(this);
            _attributeDefinitions = new DomAttributeDefinitionCollection(this as DomElementDefinition);
        }

        protected DomContainerDefinition(string name) : this(DomName.Create(name)) {
        }
    }
}
