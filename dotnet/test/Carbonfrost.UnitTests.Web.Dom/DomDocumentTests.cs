//
// Copyright 2013, 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

    public class DomDocumentTests {

        public TestData[] DocumentCreateNodeMethods {
            get {
                return new [] {
                    CreateNodeMethod("Attribute", doc => doc.CreateAttribute("name")),
                    CreateNodeMethod("CDataSection", doc => doc.CreateCDataSection()),
                    CreateNodeMethod("Comment", doc => doc.CreateComment()),
                    CreateNodeMethod("DocumentFragment", doc => doc.CreateDocumentFragment()),
                    CreateNodeMethod("DocumentType", doc => doc.CreateDocumentType("name")),
                    CreateNodeMethod("Element", doc => doc.CreateElement("name")),
                    CreateNodeMethod("Entity", doc => doc.CreateEntity("name")),
                    CreateNodeMethod("Notation", doc => doc.CreateNotation("name")),
                    CreateNodeMethod("EntityReference", doc => doc.CreateEntityReference("name")),
                    CreateNodeMethod("ProcessingInstruction", doc => doc.CreateProcessingInstruction("target")),
                    CreateNodeMethod("Text", doc => doc.CreateText()),
                };
            }
        }

        private TestData CreateNodeMethod(string v, Func<DomDocument, DomObject> p) {
            return new TestData(p).WithName(v);
        }

        [Fact]
        public void CreateElement_implies_owner_document() {
            DomDocument doc = new DomDocument();
            var html = doc.CreateElement("html");

            Assert.Null(html.ParentNode);
            Assert.Same(doc, html.OwnerDocument);
        }

        [Fact]
        public void OwnerDocument_is_null_nominal() {
            DomDocument doc = new DomDocument();
            Assert.Null(doc.OwnerDocument);
        }

        [Fact]
        public void Attributes_null_and_no_values() {
            DomDocument doc = new DomDocument();

            Assert.Null(doc.Attributes);

            // TODO Review this behavior: should it be an error?
            Assert.Same(doc, doc.Attribute("s", "s"));
            Assert.Null(doc.Attribute("s"));
        }

        [Fact]
        public void OuterXml_should_be_equal_to_xml() {
            var d = new DomDocument();
            const string xml = "<html> <head> </head> <body a=\"false\"> </body> </html>";
            d.LoadXml(xml);
            Assert.Equal(xml, d.OuterXml);
        }

        [Fact]
        public void OuterXml_should_escape_XML_entities() {
            var doc = new DomDocument();
            doc.AppendElement("e").AppendText("& < \" > '");
            Assert.Equal("<e>&amp; &lt; &quot; &gt; &apos;</e>", doc.OuterXml);
        }

        [Fact]
        public void OuterXml_should_escape_XML_entities_in_attributes() {
            var doc = new DomDocument();
            doc.AppendElement("e").Attribute("s", "& < \" > '");
            Assert.Equal("<e s=\"&amp; &lt; &quot; &gt; &apos;\"/>", doc.OuterXml);
        }

        [Fact]
        public void InnerXml_should_equal_xml() {
            var d = new DomDocument();
            const string xml = "<html> <head> </head> <body a=\"false\"> </body> </html>";
            d.LoadXml(xml);
            Assert.Equal(xml, d.InnerXml);
        }

        [Fact]
        public void ToXml_converts_to_correct_output() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            body.AppendElement("h1").AppendText("Hello, world");
            html.Attribute("lang", "en");

            // N.B. By default, xml decl is omitted
            Assert.Equal("<html lang=\"en\"><body><h1>Hello, world</h1></body></html>", doc.ToXmlString());
        }

        [Fact]
        public void CreateAttribute_implies_owner_document() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = doc.CreateAttribute("class");
            html.Attributes.Add(attr);
            Assert.Same(html, attr.OwnerElement);
            Assert.Null(attr.ParentNode);
            Assert.Same(doc, attr.OwnerDocument);
        }

        [Fact]
        public void CreateAttribute_unlinked_implies_owner_document() {
            DomDocument doc = new DomDocument();
            var html = doc.CreateAttribute("class");
            Assert.Null(html.OwnerElement);
            Assert.Null(html.ParentNode);
            Assert.Same(doc, html.OwnerDocument);
        }

        [Fact]
        public void CreateAttribute_requires_name_argument() {
            DomDocument doc = new DomDocument();
            Assert.Throws<ArgumentNullException>(() => doc.CreateAttribute(null));
        }

        [Fact]
        public void CreateAttribute_requires_name_argument_nonempty() {
            DomDocument doc = new DomDocument();
            Assert.Throws<ArgumentException>(() => doc.CreateAttribute(""));
        }

        [Fact]
        public void CreateAttribute_uses_dom_value_specified_by_schema() {
            var schema = new DomSchema("custom");
            var attrDef = schema.AttributeDefinitions.AddNew("class");
            attrDef.ValueType = typeof(PDomValue);

            var doc = new DomDocument().WithSchema(schema);
            Assert.IsInstanceOf<PDomValue>(
                doc.CreateAttribute("class").DomValue
            );
        }

        [Fact]
        public void CreateAttribute_uses_primitive_value_specified_by_schema() {
            var schema = new DomSchema("custom");
            var attrDef = schema.AttributeDefinitions.AddNew("lcid");
            attrDef.ValueType = typeof(int);

            var doc = new DomDocument().WithSchema(schema);
            Assert.IsInstanceOf<DomValue<int>>(
                doc.CreateAttribute("lcid").DomValue
            );
        }

        class PDomValue : DomValue<string> {
        }

        [Fact]
        public void Append_should_append_multiple_noncontent_nodes() {
            DomDocument doc = new DomDocument();
            var docType = doc.CreateDocumentType("html");
            var ws = doc.CreateComment("time");
            var html = doc.CreateElement("html");
            doc.Append(docType, ws, html);
            Assert.Same(html, doc.DocumentElement);
            Assert.Equal(3, doc.ChildNodes.Count);
        }

        [Fact]
        public void ReplaceWith_should_replace_element_with_element() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            var head = (DomElement) body.ReplaceWith(doc.CreateElement("head"));
            Assert.Equal("head", ((DomElement) html.ChildNode(0)).Name);
            Assert.Equal("head", head.Name);
        }

        [Fact]
        public void NodeValue_nominal() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            Assert.Null(html.NodeValue);
        }

        [Fact]
        public void ParentElement_should_be_null() {
            DomDocument doc = new DomDocument();
            Assert.Null(doc.ParentElement);
        }

        [Fact]
        public void Elements_should_be_empty_in_document_with_no_document_element() {
            DomDocument doc = new DomDocument();
            Assert.Empty(doc.Elements);

            doc.AppendComment("text");
            Assert.Empty(doc.Elements);
        }

        [Fact]
        public void ToXmlString_should_return_basic_xml() {
            var doc = new DomDocument();
            doc.Append(doc.CreateElement("el"));
            Assert.Equal("<el />", doc.ToXmlString());
        }

        [Fact]
        public void Append_should_disallow_text_in_document() {
            DomDocument doc = new DomDocument();
            Assert.Throws<InvalidOperationException>(
                () => doc.AppendText("text"));
        }

        [Fact]
        public void InnerText_should_be_null_in_empty_document() {
            DomDocument doc = new DomDocument();
            Assert.Null(doc.InnerText);
        }

        [Fact]
        public void IsContainer_should_be_true() {
            DomDocument doc = new DomDocument();
            Assert.True(doc.IsContainer);
        }

        [Fact]
        public void IsDocument_should_be_true() {
            DomDocument doc = new DomDocument();
            Assert.True(doc.IsDocument);
        }

        [Fact]
        public void DescendantNodes_should_be_breadth_first() {
            DomDocument doc = new DomDocument();

            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            var para1 = body.AppendElement("p");
            var para2 = body.AppendElement("div");
            para1.AppendComment("Greeting");
            para1.AppendText("Hello, world!");
            para2.AppendElement("k");

            Assert.Equal("html body p div #comment #text k",
                        string.Join(" ", doc.DescendantNodes.Select(t => t.NodeName)));
        }

        [Fact]
        public void DescendantNodes_should_be_empty_by_default() {
            DomDocument doc = new DomDocument();
            Assert.Empty(doc.DescendantNodes);
        }

        [Fact]
        public void DescendantNodes_should_update_as_document_does() {
            DomDocument doc = new DomDocument();
            var desc = doc.DescendantNodes;
            doc.AppendElement("b").AppendElement("r").AppendElement("ea").AppendElement("d");

            Assert.Equal(
                "bread",
                string.Join("", desc.Select(t => t.NodeName))
            );
        }

        [Fact]
        public void AncestorNodes_should_be_expected_traversal_value() {
            DomDocument doc = new DomDocument();

            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            var para1 = body.AppendElement("p");
            var para2 = body.AppendElement("div");
            para1.AppendComment("Greeting");
            para1.AppendText("Hello, world!");
            var k = para2.AppendElement("k");

            Assert.Equal("div body html #document",
                         string.Join(" ", k.AncestorNodes.Select(t => t.NodeName)));
            Assert.Equal("k div body html #document",
                         string.Join(" ", k.AncestorNodesAndSelf.Select(t => t.NodeName)));
        }

        [Fact]
        public void AncestorNodes_should_be_empty() {
            DomDocument doc = new DomDocument();
            Assert.Empty(doc.AncestorNodes);
            Assert.Equal(new [] { doc }, doc.AncestorNodesAndSelf);
        }

        [Fact]
        public void Append_should_relocate_elements_between_documents() {
            var doc1 = new DomDocument();
            var doc2 = new DomDocument();
            var ele1 = doc1.AppendElement("left");
            var root = doc2.AppendElement("root");
            var elements = new [] {
                root.AppendElement("right1"),
                root.AppendElement("right2"),
                root.AppendElement("right3"),
            };

            ele1.Append(root.ChildNodes);
            Assert.Same(doc1, elements[0].OwnerDocument);
        }

        [Fact]
        public void CreateElement_should_throw_on_empty_string() {
            var doc = new DomDocument();
            Assert.Throws<ArgumentException>(() => doc.CreateElement(""));
        }

        [Fact]
        public void CreateElement_should_throw_on_ws() {
            var doc = new DomDocument();
            Assert.Throws<ArgumentException>(() => doc.CreateElement(" s"));
        }

        [Fact]
        public void LoadXml_from_xml_should_roundtrip() {
            var doc = new DomDocument();
            var xml = "<html> <body a=\"true\" b=\"false\"> Text <p a=\"b\" /> <span> <time /> </span>  </body> </html>";
            doc.LoadXml(xml);
            Assert.Equal(xml, doc.ToXmlString());
        }

        [Theory]
        [InlineData("   <doc />")]
        [InlineData("<doc />   ")]
        [InlineData("  <doc /> ")]
        public void LoadXml_parses_leading_and_or_trailing_ws(string input) {
            var d = new DomDocument();
            d.LoadXml(input);
            Assert.Equal(input, d.ToXmlString());
        }

        [Fact]
        public void Append_cannot_have_two_roots() {
          var doc = new DomDocument();
          doc.AppendElement("s");
          Assert.Throws<InvalidOperationException>(() => doc.AppendElement("t"));
        }

        [Fact]
        public void NodeName_equals_special_name() {
            var doc = new DomDocument();
            Assert.Equal("#document", doc.NodeName);
        }

        [Fact]
        public void NodeValue_is_null() {
            var doc = new DomDocument();
            Assert.Null(doc.NodeValue);
        }

        [Fact]
        public void Doctype_should_get_the_document_type() {
            var doc = new DomDocument();
            var dt = doc.CreateDocumentType("html");
            dt.AppendTo(doc);
            doc.AppendText(" ");
            Assert.Same(dt, doc.Doctype);
        }

        [Theory]
        [PropertyData(nameof(DocumentCreateNodeMethods))]
        public void Create_any_node_will_add_it_unlinked(object o) {
            Func<DomDocument, DomObject> action = (Func<DomDocument, DomObject>) o;
            var doc = new DomDocument();
            var node = action(doc);
            Assert.Same(doc, node.OwnerDocument);
        }

        [Fact]
        public void Children_should_equal_the_document_element() {
            var doc = new DomDocument();
            var node = doc.AppendElement("root");
            doc.PrependDocumentType("html");
            Assert.HasCount(1, doc.Children);
            Assert.Equal("root", doc.Children[0].NodeName);
        }

        [Fact]
        public void InnerText_gets_contents_of_child_nodes() {
            var doc = new DomDocument();
            doc.LoadXml("<?xml version=\"1.0\" ?><!-- comment--><hello>world</hello>");
            Assert.Equal("world", doc.InnerText);
        }

        [Fact]
        public void InnerText_set_will_replace_inner_element_with_text() {
            var doc = new DomDocument();
            doc.LoadXml("<!-- comment--><hello></hello>");
            doc.InnerText = "world";
            Assert.Equal("<!-- comment--><hello>world</hello>", doc.OuterXml);
        }

        [Fact]
        public void InnerText_set_will_throw_if_no_root_element() {
            var doc = new DomDocument();
            doc.AppendComment("comment");
            Assert.Throws<InvalidOperationException>(() => doc.InnerText = "world");
        }
    }
}
