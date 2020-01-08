//
// Copyright 2013, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Linq;
using System.Xml;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomDocumentTests {

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
            Assert.Equal("<e s=\"&amp; &lt; &quot; &gt; &apos;\"></e>", doc.OuterXml);
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
            // doc.AppendDocumentType("html");
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            body.AppendElement("h1").AppendText("Hello, world");
            html.Attribute("lang", "en");

            // N.B. By default, xml decl is omitted
            Assert.Equal("<html lang=\"en\"><body><h1>Hello, world</h1></body></html>", doc.ToXml());
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
        public void ToString_should_return_basic_xml() {
            var doc = new DomDocument();
            doc.Append(doc.CreateElement("el"));
            Assert.Equal("<el />", doc.ToString());
        }

        [Fact]
        public void Append_should_disallow_text_in_document() {
            DomDocument doc = new DomDocument();
            Assert.Throws<InvalidOperationException>(
                () => doc.AppendText("text"));
        }

        [Fact]
        public void InnerText_should_be_null() {
            DomDocument doc = new DomDocument();
            Assert.Null(doc.InnerText);
        }

        [Fact]
        public void InnerText_should_be_null_with_elements() {
            DomDocument doc = new DomDocument();
            doc.AppendElement("a");
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
        public void DescendentNodes_should_be_breadth_first() {
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
        public void DescendentNodes_should_be_empty_by_default() {
            DomDocument doc = new DomDocument();
            Assert.Empty(doc.DescendantNodes);
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
            Assert.Equal(xml, doc.ToXml());
        }

        [Theory]
        [InlineData("   <doc />")]
        [InlineData("<doc />   ")]
        [InlineData("  <doc /> ")]
        public void LoadXml_parses_leading_and_or_trailing_ws(string input) {
            var d = new DomDocument();
            d.LoadXml(input);
            Assert.Equal(input, d.ToXml());
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
    }
}
