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
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomNodeVisitorSpecializationTests {

        [Fact]
        public void Visit_should_dispatch_to_slow_derived_interface_method() {
            var doc = new DomDocument();
            var ele = doc.AppendElement("ok");
            ele.Append(new RText());

            var r = new RNodeVisitor();
            DomNodeVisitor.Visit(doc, r);
            Assert.Equal(1, r.VisitTextCallCount);
            Assert.Equal(0, r.VisitDefaultTextCalledCount);
        }

        [Fact]
        public void Visit_should_dispatch_to_most_specific_slow_derived_interface_method() {
            var doc = new DomDocument();
            var ele = doc.AppendElement("ok");
            ele.Append(new RTextDerivedAlso());

            var r = new RNodeVisitor();
            DomNodeVisitor.Visit(doc, r);
            Assert.Equal(1, r.VisitTextDerivedAlsoCallCount); // Because Visit(RTextDerivedAlso) exists
            Assert.Equal(0, r.VisitTextCallCount);
            Assert.Equal(0, r.VisitDefaultTextCalledCount);
        }

        [Fact]
        public void Visit_should_dispatch_to_less_specific_slow_derived_interface_method() {
            var doc = new DomDocument();
            var ele = doc.AppendElement("ok");
            ele.Append(new RTextDerived());

            var r = new RNodeVisitor();
            DomNodeVisitor.Visit(doc, r);
            Assert.Equal(1, r.VisitTextCallCount); // Because Visit(RTextDerived) does not exist
            Assert.Equal(0, r.VisitDefaultTextCalledCount);
        }

        [Fact]
        public void Visit_should_unwrap_reflection_exceptions() {
            var doc = new DomDocument();
            var ele = doc.AppendElement("ok");
            ele.Append(new RTextDerived());

            var r = new RNodeVisitorThatThrows();
            Assert.Throws<ExpectedException>(() => DomNodeVisitor.Visit(doc, r));
        }
    }

    class ExpectedException : Exception {}
    class RTextDerived : RText {}
    class RTextDerivedAlso : RTextDerived {}

    interface IRVisitor : IDomNodeVisitor {
        void Visit(RText text);
        void Visit(RTextDerivedAlso text);
        void Visit(RAttribute attribute);
        void Visit(RElement element);
        void Visit(RProcessingInstruction instruction);
    }

    class RNodeVisitor : DomNodeVisitor, IRVisitor {
        public int VisitInstructionCalledCount;
        public int VisitAttributeCalledCount;
        public int VisitElementCalledCount;
        public int VisitDefaultElementCalledCount;
        public int VisitTextCallCount;
        public int VisitDefaultTextCalledCount;
        public int VisitTextDerivedAlsoCallCount;

        public void Visit(RAttribute attribute) {
            VisitAttributeCalledCount += 1;
        }

        public virtual void Visit(RElement element) {
            VisitElementCalledCount += 1;
        }

        public void Visit(RProcessingInstruction instruction) {
            VisitInstructionCalledCount += 1;
        }

        public virtual void Visit(RText text) {
            VisitTextCallCount += 1;
        }

        public void Visit(RTextDerivedAlso text) {
            VisitTextDerivedAlsoCallCount += 1;
        }

        protected override void VisitElement(DomElement element) {
            VisitDefaultElementCalledCount += 1;
            base.VisitElement(element);
        }

        protected override void VisitText(DomText text) {
            VisitDefaultTextCalledCount += 1;
            base.VisitText(text);
        }
    }

    class RNodeVisitorThatThrows : RNodeVisitor {
        public override void Visit(RText element) {
            throw new ExpectedException();
        }
    }
}
