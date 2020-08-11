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
using System;
using System.Collections.Generic;

namespace Carbonfrost.UnitTests.Web.Dom {

    public partial class DomNameContextTests : TestClass {

        public static readonly DomNamespace ImplicitTestNamespace = "hello";

        public DomDocument Document {
            get;
            private set;
        }

        public DomElement Element {
            get;
            private set;
        }

        // Used to test implicit naming with attributes
        public DomElement ImplicitTestElement {
            get;
            private set;
        }

        public IEnumerable<Func<string, DomObject>> ImplicitNameActions {
            get {
                return new Func<string, DomObject>[] {
                    name => Document.CreateElement(name),
                    name => Document.CreateAttribute(name),
                    name => ImplicitTestElement.SetName(name),
                    name => {
                        ImplicitTestElement.Attribute(DomName.Create("p"), "");
                        ImplicitTestElement.Attributes[0].SetName(name);
                        return ImplicitTestElement.Attributes[0];
                    },
                    name => ImplicitTestElement.AppendElement(name),
                    name => Element.PrependElement(name),
                    name => {
                        ImplicitTestElement.Attribute(name, "");
                        return ImplicitTestElement.Attributes[0];
                    },
                    name => Element.Wrap(name),
                };
            }
        }

        public IEnumerable<Action<DomName>> Actions {
            get {
                return new Action<DomName>[] {
                    name => Document.CreateElement(name),
                    name => Document.CreateAttribute(name),
                    name => Element.SetName(name),
                    name => Element.Attributes[0].SetName(name),
                    name => Element.AppendElement(name),
                    name => Element.PrependElement(name),
                    name => Element.Wrap(name),
                };
            }
        }

        public DomNameContextTests() {
            SetupDocument();
        }

        [Theory]
        [InlineData("a=z")]
        [InlineData("a1")]
        public void Html_name_context_allows_name(string name) {
            Assert.True(
                DomNameContext.Html.IsValidLocalName(name)
            );
        }

        [Theory]
        [InlineData("a=z")]
        [InlineData("1a")]
        [InlineData("a space")]
        [InlineData("á_accent_character")]
        public void Xml_name_context_does_not_allow_name(string name) {
            Assert.False(
                DomNameContext.Xml.IsValidLocalName(name)
            );
        }

        [Theory]
        [PropertyData(nameof(Actions))]
        public void Action_will_throw_on_invalid_name(Action<DomName> action) {
            action = RebindDelegate(action);

            // This name is invalid in XML but valid in HTML
            var invalidXmlName = DomName.Create(DomNamespace.Default, "t=s");
            Given(invalidXmlName).Expect(action).ToThrow.Exception<ArgumentException>();
            Given(invalidXmlName).Expect(action).ToThrow.Message.StartsWith(
                "Name `t=s' cannot be used in this context"
            );
        }

        [Theory]
        [PropertyData(nameof(Actions))]
        public void Action_will_not_throw_on_context_that_allows_it(Action<DomName> action) {
            action = RebindDelegate(action);

            // This name is invalid in XML but valid in HTML
            var invalidXmlName = DomName.Create(DomNamespace.Default, "t=s");
            Document.NameContext = DomNameContext.Html;
            Given(invalidXmlName).Expect(action).Not.ToThrow.Exception();
        }

        [Theory]
        [PropertyData(nameof(ImplicitNameActions))]
        public void ImplicitNameAction_will_apply_name_conversion_rules(Func<string, DomObject> action) {
            action = RebindDelegate(action);

            Document.NameContext = new FDomNameContext();

            var result = action("local");
            Expect(result.Name.Namespace).ToBe.EqualTo(ImplicitTestNamespace);
        }

        [Fact]
        public void Document_OuterXml_should_treat_prefixed_name_as_is() {
            var doc = new DomDocument();
            doc.AppendElement(DomName.Create("prefix:name"));

            // In the default DomNameContext, the prefix is treated as part of the
            // local name, and thus prints out as-is
            Assert.Equal("<prefix:name/>", doc.OuterXml);
        }

        [Fact]
        public void GetName_is_case_insensitive_by_Default_accessing_attributes() {
            var ele = new DomDocument().CreateElement("s");

            Assert.True(ele.NameContext.Comparer.Equals("a", "A"));

            var attr = ele.Attribute("a", "b");
            Assert.Equal("b", ele.Attribute("A"));
            Assert.Same(ele.Attributes[0], ele.Attributes["A"]);
        }

        [Fact]
        public void GetName_is_case_insensitive_by_Default_accessing_elements() {
            var ele = new DomDocument().CreateElement("s");

            Assert.True(ele.NameContext.Comparer.Equals("a", "A"));

            var child = ele.AppendElement("a");
            Assert.Same(child, ele.Element("A"));
        }

        [Fact]
        public void GetName_is_case_sensitive_in_xml() {
            var ele = new DomDocument {
                NameContext = DomNameContext.Xml
            }.AppendElement("s");
            ele.Attribute("a", "b");

            Assert.False(ele.NameContext.Comparer.Equals("a", "A"));
            Assert.Equal(DomNameComparer.Ordinal, ele.Attributes._Comparer);
            Assert.Null(ele.Attribute("A"), "should be null because of case-sensitivity");
        }

        [Fact]
        public void GetName_will_apply_name_context_comparer_when_it_changes() {
            var ele = new DomDocument().CreateElement("s");
            ele.NameContext = DomNameContext.Xml;

            var attr1 = ele.AppendAttribute("a", "b");
            var attr2 = ele.AppendAttribute("A", "B");

            Assume.HasCount(2, ele.Attributes);
            ele.NameContext = DomNameContext.Html;

            Assert.HasCount(1, ele.Attributes);
            Assert.Same(attr1, ele.Attributes[0]);

            Assert.True(attr2.IsUnlinked);
            Assert.Null(attr2.OwnerElement);
        }

        [Fact]
        public void GetName_will_notify_mutation_events_when_name_content_changes() {
            var ele = new DomDocument().CreateElement("s");
            ele.NameContext = DomNameContext.Xml;
            ele.Attribute("a", "a").Attribute("A", "A")
                .Attribute("b", "b").Attribute("B", "B")
                .Attribute("c", "c").Attribute("C", "C");

            var evts = new TestActionDispatcher<DomAttributeEvent>(_ => {});
            var observer = ele.OwnerDocument.ObserveAttributes(ele, evts.Invoke);

            ele.NameContext = DomNameContext.Html;

            // Should have generated several events indicating that attributes were
            Assert.Equal(3, evts.CallCount);
            Assert.Equal("A", evts.ArgsForCall(0).LocalName);
            Assert.Equal("B", evts.ArgsForCall(1).LocalName);
            Assert.Equal("C", evts.ArgsForCall(2).LocalName);
        }

        class FDomNameContext : DomNameContext {
            public override DomName GetName(DomName name) {
                return ImplicitTestNamespace + name.LocalName;
            }
        }

        private void SetupDocument() {
            Document = new DomDocument();
            var root = Document.AppendElement("root");

            Element = root.AppendElement("m");
            Element.Attribute("h", "ello");
            ImplicitTestElement = root.AppendElement("n");
            ImplicitTestElement.NameContext = new FDomNameContext();
        }

        private TDelegate RebindDelegate<TDelegate>(TDelegate action) where TDelegate : Delegate {
            return (TDelegate) Delegate.CreateDelegate(
                typeof(TDelegate), this, action.Method, true
            );
        }

    }
}
