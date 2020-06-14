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

using Carbonfrost.Commons.Web.Dom;
using Carbonfrost.Commons.Spec;
using System.Collections.Generic;
using System;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomElementVisitorSpecializationTests {

        public IEnumerable<Func<DomElement, DomElement>> AppendElementOperations {
            get {
                return new Func<DomElement, DomElement>[] {
                    d => d.AppendElement("append"),
                    d => d.PrependElement("append"),
                };
            }
        }

        public IEnumerable<Func<DomElement, DomAttribute>> AppendAttributeOperations {
            get {
                return new Func<DomElement, DomAttribute>[] {
                    d => d.AppendAttribute("append", "value"),
                    d => d.PrependAttribute("prepend", "value"),
                    d => {
                        d.Attribute("attr", "0");
                        return d.Attributes[0];
                    },
                };
            }
        }

        [Fact]
        public void Visitor_should_dispatch_to_delegate_types() {
            var doc = new DomDocument();
            var ele = doc.AppendElement("html");
            ele.AppendElement("body");
            ele.Append(new RElement("name"));

            var rv = new RNodeVisitor();
            rv.Visit(doc);

            // Should delegate to the specialized dispatch and not the default
            Assert.Equal(1, rv.VisitElementCalledCount);
            Assert.Equal(2, rv.VisitDefaultElementCalledCount);
        }

        [Theory]
        [PropertyData(nameof(AppendElementOperations))]
        public void AppendElement_operations_should_use_specialization_type_by_default(Func<DomElement, DomElement> op) {
            var root = new RDocument().AppendElement("documentElement");
            var ele = op(root);
            Assert.IsInstanceOf(typeof(RElement), ele);
        }

        [Theory]
        [PropertyData(nameof(AppendAttributeOperations))]
        public void AppendAttribute_operations_should_use_specialization_type_by_default(Func<DomElement, DomAttribute> op) {
            var doc = new RDocument();
            var attribute = op(doc.AppendElement("cool"));
            Assert.IsInstanceOf(typeof(RAttribute), attribute);
        }

        [Fact]
        public void AppendProcessingInstruction_operations_should_use_specialization_type_by_default() {
            var doc = new RDocument();
            var pi = doc.AppendElement("cool").AppendProcessingInstruction("k", "");
            Assert.IsInstanceOf(typeof(RProcessingInstruction), pi);
        }
    }

    class RDocumentFragment : DomDocumentFragment {}
    class RDocument : DomDocument {
        protected override DomProviderFactory DomProviderFactory {
            get {
                return new RProviderFactory();
            }
        }
    }
    class RElement : DomElement<RElement> {
        public RElement(string name) : base(name) {}
    }
    class RText : DomText {}
    class RProviderFactory : DomProviderFactory {
        protected override IDomNodeFactory CreateDomNodeFactory(IDomNodeTypeProvider nodeTypeProvider) {
            return new DomNodeFactory(
                DomNodeTypeProvider.Compose(nodeTypeProvider, new RNodeTypeProvider())
            );
        }
    }

    class RNodeTypeProvider : IDomNodeTypeProvider {
        public DomName GetAttributeName(Type attributeType) {
            return null;
        }

        public Type GetAttributeNodeType(string name) {
            return typeof(RAttribute);
        }

        public DomName GetElementName(Type elementType) {
            return null;
        }

        public Type GetAttributeNodeType(DomName name) {
            return typeof(RAttribute);
        }

        public Type GetElementNodeType(DomName name) {
            return typeof(RElement);
        }

        public Type GetProcessingInstructionNodeType(string target) {
            return typeof(RProcessingInstruction);
        }

        public string GetProcessingInstructionTarget(Type processingInstructionType) {
            return null;
        }
    }
    class RAttribute : DomAttribute<RAttribute> {
        public RAttribute(string name) : base(name) {}
    }
    class RProcessingInstruction : DomProcessingInstruction<RProcessingInstruction> {
        public RProcessingInstruction(string target) : base(target) {}
    }

    [DomProviderFactoryUsage(Extensions = ".rrr")]
    public class RDomProviderFactory : DomProviderFactory {

        protected override DomDocument CreateDomDocument() {
            return new RDocument();
        }

        public override IDomNodeTypeProvider NodeTypeProvider {
            get {
                return new RDomNodeTypeProvider();
            }
        }

        public override bool IsProviderObject(Type providerObjectType) {
            return providerObjectType.Name[0] == 'R' && providerObjectType.Namespace == "Carbonfrost.UnitTests.Web.Dom";
        }

        private class RDomNodeTypeProvider : DomNodeTypeProvider {
            public override DomName GetAttributeName(Type attributeType) {
                return DomName.Create("r:" + attributeType.Name);
            }

            public override DomName GetElementName(Type elementType) {
                return DomName.Create("r:" + elementType.Name);
            }
        }
    }
}
