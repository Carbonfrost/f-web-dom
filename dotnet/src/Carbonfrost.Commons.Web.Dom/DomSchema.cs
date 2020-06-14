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

using System;

namespace Carbonfrost.Commons.Web.Dom {

    public class DomSchema : DomContainerDefinition, IDomNodeTypeProvider {
        private IDomNodeTypeProvider _nodeTypeProvider;
        private readonly DomProcessingInstructionDefinitionCollection _processingInstructionDefinitions = new DomProcessingInstructionDefinitionCollection();

        public DomProcessingInstructionDefinitionCollection ProcessingInstructionDefinitions {
            get {
                return _processingInstructionDefinitions;;
            }
        }

        public DomSchema(string name) : base(name) {
        }

        public DomSchema(DomName name) : base(name) {
        }

        public IDomNodeTypeProvider NodeTypeProvider {
            get {
                return _nodeTypeProvider ?? DomNodeTypeProvider.Default;
            }
            set {
                ThrowIfReadOnly();
                _nodeTypeProvider = value;
            }
        }

        public static DomSchema ReadOnly(DomSchema schema) {
            if (schema == null) {
                throw new ArgumentNullException(nameof(schema));
            }
            var result = schema.Clone();
            result.MakeReadOnly();
            return result;
        }

        public new DomSchema Clone() {
            return (DomSchema) base.Clone();
        }

        protected override DomNodeDefinition CloneCore() {
            var schema = new DomSchema(Name) {
                NodeTypeProvider = NodeTypeProvider,
            };
            foreach (var ed in ElementDefinitions) {
                schema.ElementDefinitions.Add(ed.Clone());
            }
            foreach (var ad in AttributeDefinitions) {
                schema.AttributeDefinitions.Add(ad.Clone());
            }
            foreach (var pd in ProcessingInstructionDefinitions) {
                schema.ProcessingInstructionDefinitions.Add(pd.Clone());
            }
            return schema;
        }

        public Type GetAttributeNodeType(DomName name) {
            var attr = AttributeDefinitions[name];
            Type result = null;
            if (attr != null) {
                result = attr.AttributeNodeType;
            }
            return result ?? NodeTypeProvider.GetAttributeNodeType(name);
        }

        public Type GetAttributeNodeType(string name) {
            return GetAttributeNodeType(DomName.Create(name));
        }

        public Type GetElementNodeType(DomName name) {
            var element = ElementDefinitions[name];
            Type result = null;
            if (element != null) {
                result = element.ElementNodeType;
            }
            return result ?? NodeTypeProvider.GetElementNodeType(name);
        }

        public Type GetElementNodeType(string name) {
            return GetElementNodeType(DomName.Create(name));
        }

        public Type GetProcessingInstructionNodeType(string target) {
            return NodeTypeProvider.GetProcessingInstructionNodeType(target);
        }

        public DomName GetAttributeName(Type attributeType) {
            return NodeTypeProvider.GetAttributeName(attributeType);
        }

        public DomName GetElementName(Type elementType) {
            return NodeTypeProvider.GetElementName(elementType);
        }

        public string GetProcessingInstructionTarget(Type processingInstructionType) {
            return NodeTypeProvider.GetProcessingInstructionTarget(processingInstructionType);
        }

        internal override void MakeReadOnly() {
            base.MakeReadOnly();
            ElementDefinitions.MakeReadOnly();
            AttributeDefinitions.MakeReadOnly();
            ProcessingInstructionDefinitions.MakeReadOnly();
        }

        public DomProcessingInstructionDefinition GetProcessingInstructionDefinition(string target) {
            return GetDomProcessingInstructionDefinition(target);
        }

        protected virtual DomProcessingInstructionDefinition GetDomProcessingInstructionDefinition(string target) {
            return ProcessingInstructionDefinitions[target];
        }

        public DomAttributeDefinition GetAttributeDefinition(DomName name) {
            return GetDomAttributeDefinition(name);
        }

        public DomAttributeDefinition GetAttributeDefinition(string name) {
            return GetAttributeDefinition(DomName.Create(name));
        }

        protected virtual DomAttributeDefinition GetDomAttributeDefinition(DomName name) {
            return AttributeDefinitions[name];
        }

        public DomElementDefinition GetElementDefinition(DomName name) {
            return GetDomElementDefinition(name);
        }

        public DomElementDefinition GetElementDefinition(string name) {
            return GetElementDefinition(DomName.Create(name));
        }

        protected virtual DomElementDefinition GetDomElementDefinition(DomName name) {
            return ElementDefinitions[name];
        }
    }
}
