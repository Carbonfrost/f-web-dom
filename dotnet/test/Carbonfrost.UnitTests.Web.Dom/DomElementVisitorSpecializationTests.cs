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

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomElementVisitorSpecializationTests {

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
    }

    class RElement : DomElement<RElement> {
        public RElement() {}
        public RElement(string name) : base(name) {}
    }
    class RProviderFactory : DomProviderFactory {}
    class RAttribute : DomAttribute<RAttribute> {}
    class RProcessingInstruction : DomProcessingInstruction<RProcessingInstruction> {}
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
