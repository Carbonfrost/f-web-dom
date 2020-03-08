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
    class RProviderFactory : DomProviderFactory {
        protected override IDomNodeFactory CreateDomNodeFactory(IDomNodeTypeProvider nodeTypeProvider) {
            return new DomNodeFactory(
                DomNodeTypeProvider.Compose(nodeTypeProvider, new RNodeTypeProvider())
            );
        }
    }
    class RNodeTypeProvider : IDomNodeTypeProvider {
        public Type GetAttributeNodeType(string name) {
            return typeof(RAttribute);
        }
        public Type GetElementNodeType(string name) {
            return typeof(RElement);
        }
        public Type GetProcessingInstructionNodeType(string target) {
            return typeof(RProcessingInstruction);
        }
    }
    class RAttribute : DomAttribute<RAttribute> {
        public RAttribute(string name) : base(name) {}
    }
    class RProcessingInstruction : DomProcessingInstruction<RProcessingInstruction> {
        public RProcessingInstruction(string target) : base(target) {}
    }
    class RNodeVisitor : DomNodeVisitor, IDomAttributeVisitor<RAttribute>, IDomElementVisitor<RElement>, IDomProcessingInstructionVisitor<RProcessingInstruction> {
        public int VisitInstructionCalledCount;
        public int VisitAttributeCalledCount;
        public int VisitElementCalledCount;
        public int VisitDefaultElementCalledCount;

        public void Visit(RAttribute attribute) {
            VisitAttributeCalledCount += 1;
        }

        public void Visit(RElement element) {
            VisitElementCalledCount += 1;
        }

        public void Visit(RProcessingInstruction instruction) {
            VisitInstructionCalledCount += 1;
        }

        protected override void VisitElement(DomElement element) {
            VisitDefaultElementCalledCount += 1;
            base.VisitElement(element);
        }

        void IDomAttributeVisitor<RAttribute>.Dispatch(RAttribute attribute) {
            Visit(attribute);
        }

        void IDomElementVisitor<RElement>.Dispatch(RElement element) {
            Visit(element);
        }

        void IDomProcessingInstructionVisitor<RProcessingInstruction>.Dispatch(RProcessingInstruction instruction) {
            Visit(instruction);
        }
    }
}
