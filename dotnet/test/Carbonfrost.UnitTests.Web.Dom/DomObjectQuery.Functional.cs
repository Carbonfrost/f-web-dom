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
using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public partial class DomObjectQueryTests {

        [Theory]
        [FixtureData("DomObjectQuery-{operation}.fixture")]
        public void Applying_operation_should_equal_expected_output(DomObjectQueryFixture f) {
            var doc = new DomDocument();
            var input = doc.CreateDocumentFragment();
            var expectedOutput = doc.CreateDocumentFragment();
            input.LoadXml(f.Input);
            expectedOutput.LoadXml(f.Output);

            f.Action(
                input.Select(f.Selector ?? "target")
            );

            // Compress whitespace to simplify comparison
            input.CompressWhitespace();
            expectedOutput.CompressWhitespace();

            Assert.Equal(expectedOutput.InnerXml.Trim(), input.InnerXml.Trim());
        }
    }

    public class DomObjectQueryFixture {
        public string Case { get; set; }
        public string Operation { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string Selector { get; set; }

        public string Named { get; set; } // For operations on a named attribute or element
        public string To { get; set; } // For operations that change a name
        public string Data { get; set; } // For operations with data
        public string With { get; set; } // For operations with a node

        public Action<DomObjectQuery> Action {
            get {
                var result = NiladicOperation()
                    ?? NameOperation(To ?? Named ?? "this")
                    ?? ExampleDataOperation(Data ?? "some example data")
                    ?? NameAndDataOperation(Named ?? "this", Data ?? "data")
                    ?? NodeOperation();

                if (result == null) {
                    return _ => Assert.Pending("Not defined operation: " + Operation);
                }
                return result;
            }
        }

        private Action<DomObjectQuery> NiladicOperation() {
            switch (Operation) {
                case "unwrap":
                    return e => e.Unwrap();
                case "removeAttributes":
                    return e => e.RemoveAttributes();
            }
            return null;
        }

        private Action<DomObjectQuery> NameOperation(string named) {
            switch (Operation) {
                case "removeAttribute":
                    return e => e.RemoveAttribute(named);
                case "prependElement":
                    return e => e.PrependElement(named);
                case "appendElement":
                    return e => e.AppendElement(named);
                case "addClass":
                    return e => e.AddClass(named);
                case "removeClass":
                    return e => e.RemoveClass(named);
                case "setName":
                    return e => e.SetName(named);
                case "wrap":
                    return e => e.Wrap(named);
            }
            return null;
        }

        private Action<DomObjectQuery> ExampleDataOperation(string data) {
            switch (Operation) {
                case "appendComment":
                    return e => e.AppendComment(data);
                case "prependText":
                    return e => e.PrependText(data);
                case "prependCDataSection":
                    return e => e.PrependCDataSection(data);
                case "prependComment":
                    return e => e.PrependComment(data);
                case "appendText":
                    return e => e.AppendText(data);
                case "appendCDataSection":
                    return e => e.AppendCDataSection(data);
            }
            return null;
        }

        private Action<DomObjectQuery> NameAndDataOperation(string name, string data) {
            switch (Operation) {
                case "prependAttribute":
                    return e => e.PrependAttribute(name, data);
                case "prependProcessingInstruction":
                    return e => e.PrependProcessingInstruction(name, data);
                case "appendAttribute":
                    return e => e.AppendAttribute(name, data);
                case "appendProcessingInstruction":
                    return e => e.AppendProcessingInstruction(name, data);
            }
            return null;
        }

        private Action<DomObjectQuery> NodeOperation() {
            switch (Operation) {
                case "wrapUsingNode":
                    return e => e.Wrap(Nodes[0]);
                case "prepend":
                    return e => e.Prepend(With);
                case "append":
                    return e => e.Append(With);
                case "after":
                    return e => e.After(With);
                case "before":
                    return e => e.Before(With);
                case "replaceWith":
                    return e => e.ReplaceWith(Nodes);
            }
            return null;
        }

        private DomNode[] Nodes {
            get {
                var doc = new DomDocument();
                var frag = doc.CreateDocumentFragment();
                frag.LoadXml(With);
                return frag.ChildNodes.ToArray();
            }
        }

        public override string ToString() {
            return Case;
        }
    }
}
