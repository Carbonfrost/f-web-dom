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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomObserverTests : TestClass {

        public IEnumerable<Action<DomElement>> AttributeMutationOperations {
            get {
                return new Action<DomElement>[] {
                    e => e.Attribute("attr", "newValue"),
                    e => e.Attributes["attr"].Value = "newValue",
                    e => e.Attributes[0].ReplaceWith(e.ParentNode.Attributes[0]),
                    e => e.Attributes.RemoveAt(0),
                    e => e.Attributes.Remove("attr"),
                    e => e.RemoveAttribute("attr"),
                };
            }
        }

        public IEnumerable<Action<DomElement>> AttributeRemoveOperations {
            get {
                return new Action<DomElement>[] {
                    e => e.RemoveAttribute("attr"),
                    e => e.Attributes.Clear(),
                    e => e.Attributes.RemoveAt(0),
                    e => e.Attributes.Remove("attr"),
                    e => e.Attributes.Remove(e.Attributes.First()),
                    e => e.RemoveAttributes(),
                };
            }
        }

        public IEnumerable<MutationData> ChildListMutationOperations {
            get {
                return new MutationData[] {
                    Mutation(
                        "Add element",
                        e => e.Add(e.OwnerDocument.CreateElement("e")),
                        e => Added(e, "e", "c", null)
                    ),
                    Mutation(
                        "Add multiple elements",
                        e => e.AddRange(
                            e.OwnerDocument.CreateElement("e"),
                            e.OwnerDocument.CreateElement("f"),
                            e.OwnerDocument.CreateElement("g")
                        ),
                        e => Added(e, "e f g", "c", null)
                    ),
                    Mutation(
                        "Add element (using Append)",
                        e => e.Append(e.OwnerDocument.CreateElement("e")),
                        e => Added(e, "e", "c", null)
                    ),
                    Mutation(
                        "Empty()",
                        e => e.Empty(),
                        e => Removed(e, "a b c")
                    ),
                    Mutation(
                        "Remove first child",
                        e => e.FirstChild.Remove(),
                        e => Removed(e, "a", null, "b")
                    ),
                    Mutation(
                        "RemoveChildNodes()",
                        e => e.RemoveChildNodes(),
                        e => Removed(e, "a b c")
                    ),
                    Mutation(
                        "Add fragment",
                        e => {
                            var frag = e.OwnerDocument.CreateDocumentFragment().LoadXml("<t/><u/><v/>");
                            e.Add(frag);
                        },
                        e => Added(e, "t u v", "c")
                    ),
                    Mutation(
                        "Add fragment and element",
                        e => {
                            var frag = e.OwnerDocument.CreateDocumentFragment().LoadXml("<t/><u/><v/>");
                            var other = e.OwnerDocument.CreateElement("w");
                            e.AddRange(frag, other);
                        },
                        e => Added(e, "t u v w", "c") // Only one event
                    ),
                };
            }
        }

        public IEnumerable<MutationData> NopChildListMutationOperations {
            get {
                return new MutationData[] {
                    Mutation(
                        "Insert child at same position",
                        e => {
                            var a = e.Element("a");
                            e.ChildNodes.Insert(0, a);
                        }
                    ),
                };
            }
        }

        [Theory]
        [PropertyData(nameof(AttributeMutationOperations))]
        public void ObserveAttributes_callback_should_be_invoked_on_setting_an_attribute(Action<DomElement> callback) {
            var doc = new DomDocument().LoadXml(@"
                <root attr='root value'>
                    <e attr='oldValue' />
                </root>
            ".Replace("'", "\""));

            var element = doc.QuerySelector("e");

            var evts = new TestActionDispatcher<DomAttributeEvent>(_ => {});
            doc.ObserveAttributes(element, evts.Invoke);

            callback(element);

            Assert.Equal(1, evts.CallCount);
            Assert.Equal("oldValue", evts.ArgsForCall(0).OldValue);
            Assert.Equal("attr", evts.ArgsForCall(0).LocalName);
        }

        [Theory]
        [PropertyData(nameof(AttributeRemoveOperations))]
        public void ObserveAttributes_callback_should_be_invoked_on_removing_an_attribute(Action<DomElement> callback) {
            var doc = new DomDocument().LoadXml(@"
                <root attr='root value' />
            ".Replace("'", "\""));

            var element = doc.QuerySelector("root");

            var evts = new TestActionDispatcher<DomAttributeEvent>(_ => {});
            doc.ObserveAttributes(element, evts.Invoke);

            callback(element);

            Assert.Equal(1, evts.CallCount);
            Assert.Equal("root value", evts.ArgsForCall(0).OldValue);
            Assert.Null(evts.ArgsForCall(0).Value);
            Assert.Equal("attr", evts.ArgsForCall(0).LocalName);
        }


        [Fact]
        public void ObserveAttributes_should_invoke_optimized_registration() {
            var doc = new DomDocument();
            doc.AppendElement("hello").AppendElement("world");

            var evts = new TestActionDispatcher<DomAttributeEvent>(_ => {});
            var observer = doc.ObserveAttributes(doc.DocumentElement, "l", evts.Invoke, DomScope.TargetAndDescendants);

            doc.QuerySelectorAll("*").Attribute("l", "k");
            Assert.Equal(2, evts.CallCount);
            Assert.Equal("hello", evts.ArgsForCall(0).Target.LocalName);
            Assert.Equal("world", evts.ArgsForCall(1).Target.LocalName);
            Assert.Equal("l", evts.ArgsForCall(1).LocalName);
        }

        [Fact]
        public void ObserveAttributes_callback_should_be_invoked_at_time_after_attribute_has_changed() {
            var doc = new DomDocument();
            var element = doc.AppendElement("e");
            element.Attribute("attr", "oldValue");

            string attributeValueInCallback = null;
            doc.ObserveAttributes(element, e => {
                attributeValueInCallback = element.Attribute("attr");
            });
            element.Attribute("attr", "value");

            Assert.Equal("value", attributeValueInCallback);
        }

        [Theory]
        [PropertyData(nameof(AttributeRemoveOperations))]
        public void ObserveAttributes_callback_should_be_invoked_at_time_after_attribute_value_has_changed(Action<DomElement> op) {
            var doc = new DomDocument();
            var element = doc.AppendElement("e");
            element.Attribute("attr", "oldValue");

            string attributeValueInCallback = null;
            doc.ObserveAttributes(element, e => {
                attributeValueInCallback = element.Attribute("attr");
            });
            op(element);

            Assert.NotEqual("oldValue", attributeValueInCallback);
        }


        [Theory]
        [PropertyData(nameof(ChildListMutationOperations))]
        public void ObserveChildNodes_callback_should_be_invoked_on_mutation_nodes(MutationData data) {
            var doc = new DomDocument().LoadXml(@"
                <root><a/><b/><c/></root>
            ");

            var element = doc.DocumentElement;
            var evts = new TestActionDispatcher<DomMutationEvent>(_ => {});
            var expected = data.Event(element);
            doc.ObserveChildNodes(element, evts.Invoke);

            data.Action(element);
            var actuallyAddedNodes = data.Event(element).AddedNodes;

            Assert.Equal(1, evts.CallCount);
            Assert.Equal(actuallyAddedNodes.NodeNames(), evts.ArgsForCall(0).AddedNodes.NodeNames());
            Assert.Equal(expected.RemovedNodes.NodeNames(), evts.ArgsForCall(0).RemovedNodes.NodeNames());
            Assert.Same(expected.NextSiblingNode, evts.ArgsForCall(0).NextSiblingNode);
            Assert.Same(expected.PreviousSiblingNode, evts.ArgsForCall(0).PreviousSiblingNode);
        }

        [Theory]
        [PropertyData(nameof(NopChildListMutationOperations))]
        public void ObserveChildNodes_callback_should_be_noop(MutationData data) {
            var doc = new DomDocument().LoadXml(@"
                <root><a/><b/><c/></root>
            ");

            var element = doc.DocumentElement;
            var evts = new TestActionDispatcher<DomMutationEvent>(_ => {});
            doc.ObserveChildNodes(element, evts.Invoke);
            data.Action(element);

            Assert.Equal(0, evts.CallCount);
        }

        [Fact]
        public void Dispose_causes_handler_not_to_be_called() {
            var doc = new DomDocument();
            var attr = doc.AppendElement("hello").AppendAttribute("a");

            var evts = new TestActionDispatcher<DomAttributeEvent>(_ => {});
            var observer = doc.ObserveAttributes(doc.DocumentElement, evts.Invoke);
            observer.Dispose();

            attr.Value = "changed";
            Assert.Equal(0, evts.CallCount);
        }

        internal static DomMutationEvent Added(DomElement e, string nodes, string previous = null, string next = null) {
            return DomMutationEvent.Added(
                e,
                nodes.Split(' ').Select(n => e.Element(n)),
                previous == null ? null : e.Element(previous),
                next == null ? null : e.Element(next)
            );
        }

        internal static DomMutationEvent Removed(DomElement e, string nodes, string previous = null, string next = null) {
            return DomMutationEvent.Removed(
                e,
                nodes.Split(' ').Select(n => e.Element(n)),
                previous == null ? null : e.Element(previous),
                next == null ? null : e.Element(next)
            );
        }

        public struct MutationData {
            public readonly Action<DomElement> Action;
            public readonly Func<DomElement, DomMutationEvent> Event;
            public readonly string Name;

            public MutationData(string name, Action<DomElement> action, Func<DomElement, DomMutationEvent> evt) {
                Name = name;
                Action = action;
                Event = evt;
            }

            public override string ToString() {
                return Name;
            }
        }

        static MutationData Mutation(string name, Action<DomElement> action, Func<DomElement, DomMutationEvent> evt = null) {
            return new MutationData(name, action, evt);
        }

    }
}
