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
using System.Collections.Generic;
using System.Linq;

using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class XmlNameContextTests : TestClass {

        public IEnumerable<Func<DomElement, DomElement>> AppendElementOperations {
            get {
                return new Func<DomElement, DomElement>[] {
                    e => e.AppendElement("a:hello"),
                    e => e.PrependElement("a:hello"),
                    e => e.Wrap("a:hello"),
                    e => e.SetName("a:hello"),

                    e => e.AppendElement("x").AppendElement("a:hello"),
                    e => e.AppendElement("x").PrependElement("a:hello"),
                    e => e.AppendElement("x").Wrap("a:hello"),
                    e => e.AppendElement("x").SetName("a:hello"),
                };
            }
        }

        public IEnumerable<Func<DomElement, DomAttribute>> AppendAttributeOperations {
            get {
                return new Func<DomElement, DomAttribute>[] {
                    e => {
                        e.Attribute("a:hello", "_");
                        return e.Attributes.Last();
                    },
                    e => e.AppendAttribute("a:hello"),
                    e => e.PrependAttribute("a:hello"),
                };
            }
        }

        [Theory]
        [PropertyData(nameof(AppendElementOperations))]
        public void Xmlns_should_allow_binding_prefix_names_from_AppendElement(Func<DomElement, DomElement> op) {
            var doc = new DomDocument {
                NameContext = DomNameContext.Xml
            };
            var root = doc.AppendElement("root");
            root.Attribute("xmlns:a", "https://example.com/");

            var newElement = op(root);

            Assert.Equal(
                "hello",
                newElement.LocalName,
                "Local name should re-map without the prefix because of namespace binding"
            );
            Assert.Equal(
                "https://example.com/",
                newElement.NamespaceUri
            );
            Assert.Equal("a", newElement.Prefix);
        }

        [Fact]
        public void Xmlns_should_move_attributes_to_wrap() {
            var doc = new DomDocument {
                NameContext = DomNameContext.Xml
            };
            var root = doc.AppendElement("root");
            root.Attribute("xmlns:a", "https://example.com/");

            var newElement = root.Wrap("a:ugust");

            // The XML name context moves to the wrapped element
            Assert.IsInstanceOf(typeof(XmlNameContext), newElement.ActualNameContext);
            Assert.Equal(
                new [] { "xmlns:a" },
                newElement.Attributes.Select(a => a.Name.ToString("P"))
            );
        }

        [Fact]
        public void Xmlns_should_move_attributes_to_replacement_on_SetName() {
            var doc = new DomDocument {
                NameContext = DomNameContext.Xml
            };
            var root = doc.AppendElement("root");
            root.Attribute("xmlns:a", "https://example.com/");

            var newElement = root.SetName("a:ugust");

            // The XML name context moves to the wrapped element
            Assert.IsInstanceOf(typeof(XmlNameContext), newElement.ActualNameContext);
            Assert.Same(newElement, ((XmlNameContext) newElement.ActualNameContext).Container);

            Assert.Equal(
                new [] { "xmlns:a" },
                newElement.Attributes.Select(a => a.Name.ToString("P"))
            );
            Assert.Equal("ugust", newElement.LocalName);
        }

        [Theory]
        [PropertyData(nameof(AppendAttributeOperations))]
        public void Xmlns_should_allow_binding_prefix_names_from_AppendAttribute(Func<DomElement, DomAttribute> op) {
            var doc = new DomDocument();
            doc.NameContext = DomNameContext.Xml;
            var root = doc.AppendElement("root");
            root.Attribute("xmlns:a", "https://example.com/");

            var newAttr = op(root);

            Assert.Equal(
                "https://example.com/",
                newAttr.NamespaceUri
            );
            Assert.Equal(
                "hello",
                newAttr.LocalName,
                "Local name should re-map without the prefix because of namespace binding"
            );
        }

        [Theory]
        [InlineData("ancestor")]
        [InlineData("parent")]
        public void Xmlns_should_update_name_on_taking_as_a_descendant_from_unlinked_ownership(string newOwner) {
            var doc = new DomDocument();
            doc.NameContext = DomNameContext.Xml;
            doc.AppendElement("ancestor").AppendElement("parent");
            doc.DocumentElement.Attribute("xmlns:a", "namespace-a");

            var orphan = doc.CreateElement("a:b");
            Assume.Equal("a:b", orphan.LocalName);

            doc.QuerySelector(newOwner).Append(orphan);

            // The name gets rewritten to use the namespace semantics
            Expect(orphan.LocalName).ToBe.EqualTo("b");
            Expect(orphan.NamespaceUri).ToBe.EqualTo("namespace-a");
        }

        [Theory]
        [InlineData("ancestor")]
        [InlineData("parent")]
        public void Xmlns_should_update_name_on_ancestor_taking_name_context(string newOwner) {
            var doc = new DomDocument();
            doc.NameContext = DomNameContext.Xml;
            var element = doc.AppendElement("ancestor").AppendElement("parent").AppendElement("a:b");

            Assume.Equal("a:b", element.LocalName);

            doc.QuerySelector(newOwner).Attribute("xmlns:a", "namespace-a");

            // The name gets rewritten to use the namespace semantics
            Expect(element.NameContext).ToBe.InstanceOf(typeof(XmlNameContext));
            Expect(element.Name).ToBe.InstanceOf(typeof(DomName.QName));
            Expect(element.NamespaceUri).ToBe.EqualTo("namespace-a");
            Expect(element.LocalName).ToBe.EqualTo("b");
        }

        [Fact]
        public void Attach_should_set_up_xmlns_attribute_on_document_element() {
            var doc = new DomDocument();
            var root = doc.AppendElement("root");

            // The xml context should propagate to document
            doc.NameContext = DomNameContext.Xml;
            Assert.IsInstanceOf(typeof(XmlNameContext), doc.DocumentElement.ActualNameContext);
        }

        [Fact]
        public void Attach_should_set_up_xmlns_attribute_on_document_element_after_the_fact() {
            var doc = new DomDocument {
                NameContext = DomNameContext.Xml
            };

            // Creating the document element after the fact should set up XMLNS on it
            var root = doc.AppendElement("root");
            Assert.IsInstanceOf(typeof(XmlNameContext), root.ActualNameContext);
        }

        [Fact]
        public void Attach_should_set_up_xmlns_attribute_only_when_not_redundant() {
            var doc = new DomDocument();
            var root = doc.AppendElement("root");
            root.NameContext = DomNameContext.Xml;

            // No need to set this when it is redundant
            var child = root.AppendElement("child");
            child.NameContext = DomNameContext.Xml;

            Assert.Empty(child.Attributes);
        }

        [Fact]
        public void Attach_should_propagate_name_context_to_elements_with_xmlns_attribute() {
            var doc = new DomDocument {
                NameContext = DomNameContext.Xml
            };

            var e = doc.AppendElement("root").AppendElement("e");
            e.Attribute("xmlns:a", "https://example.com");

            // Create a new XmlNameContext so that we can start binding prefixes
            Assert.IsInstanceOf<XmlNameContext>(e.NameContext);
            Assert.NotSame(doc.NameContext, e.NameContext);
            Assert.HasCount(1, e.Attributes, "should not implicitly add any other attributes");
            // Assert.ContainsKeyWithValue("a", DomNamespace.Create("https://example.com"), ((XmlNameContext) e.NameContext).PrefixMap);
        }

        [Fact]
        public void GetName_should_apply_prefix_naming() {
            var doc = new DomDocument();
            var root = doc.AppendElement("root");
            root.NameContext = DomNameContext.Xml;
            root.Attribute("xmlns:a", "https://example.com/");

            Assert.Equal(
                "https://example.com/",
                root.NameContext.GetName("a:hello").NamespaceUri
            );
            Assert.Equal(
                "hello",
                root.NameContext.GetName("a:hello").LocalName,
                "Local name should re-map without the prefix because of namespace binding"
            );
        }

        [Fact]
        public void GetName_should_resolve_prefix_mapping() {
            var doc = new DomDocument();
            var root = doc.AppendElement("root");
            root.NameContext = DomNameContext.Xml;
            root.Attribute("xmlns:a", "https://example.com/");
            var qualified = DomNamespace.Create("https://example.com/") + "hello";

            Assert.Equal(
                "https://example.com/", root.NameContext.GetName(qualified).NamespaceUri
            );
            Assert.Equal("hello", root.NameContext.GetName(qualified).LocalName);
            Assert.Equal("a", root.NameContext.GetName(qualified).Prefix);
        }

        // TODO Consolidate XmlNameContext when they share scope

        [XFact]
        public void Document_OuterXml_should_not_print_prefixed_name_prefix_in_XML_name_context() {
            var doc = new DomDocument {
                NameContext = DomNameContext.Xml,
            };
            doc.AppendElement(DomName.Create("prefix:name"));

            // The prefix was used but not bound to a namespace, hence it
            // is not printed out in XML, and there are no XMLNS attributes.

            // TODO It is possible that XMLNS might not even get added until _at least_ one
            // binding is present
            Assert.Equal("<name xmlns=\"http://www.w3.org/2000/xmlns/\"/>", doc.OuterXml);
        }

    }
}
