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

using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public partial class DomElementTests {

        [Fact]
        public void AppendAttribute_creates_attribute_if_necessary() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("t");
            var attr = e.AppendAttribute("class", "me");

            Assert.NotNull(attr);
            Assert.Same(attr, e.Attributes[0]);
        }

        [Fact]
        public void AppendAttribute_appends_to_existing_attribute() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("t");
            var attr = doc.CreateAttribute("class", "me");
            e.Attributes.Add(attr);
            e.AppendAttribute("class", "you");

            Assert.Same(attr, e.Attributes[0]);
            Assert.Equal("meyou", e.Attributes[0].Value);
        }

        [Fact]
        public void PrependAttribute_creates_attribute_first_if_necessary() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("t").Attribute("hello", "world").Attribute("goodbye", "earth");
            var attr = e.PrependAttribute("class", "me");

            Assert.NotNull(attr);
            Assert.Same(attr, e.Attributes[0]);
        }

        [Fact]
        public void IsDocumentElement_should_be_true_for_root() {
            DomDocument doc = new DomDocument();
            doc.AppendComment("c");
            var e = doc.AppendElement("t");

            Assert.True(e.IsDocumentElement);
        }

        [Fact]
        public void OuterText_domelement() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            html.Attribute("lang", "en");
            html.Attribute("data-cast", "true");
            body.Attribute("dir", "ltr");
            body.Attribute("class", "hl");
            body.AppendText("Hello, world!");
            Assert.Equal("Hello, world!", html.OuterText);
        }

        [Fact]
        public void ToString_domelement() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            html.Attribute("lang", "en");
            html.Attribute("data-cast", "true");
            body.Attribute("dir", "ltr");
            body.Attribute("class", "hl");
            body.AppendText("Hello, world!");
            string htmlText = "<html lang=\"en\" data-cast=\"true\"><body dir=\"ltr\" class=\"hl\">Hello, world!</body></html>";
            Assert.Equal(htmlText, html.ToString());
        }

        [Fact]
        public void InnerText_domelement() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            body.Attribute("dir", "ltr");
            body.Attribute("class", "hl");
            body.AppendText("Hello, world!");
            Assert.Equal("Hello, world!", html.InnerText);
        }

        [Fact]
        public void InnerXml_should_parse_from_and_rebuild_nodes() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.InnerXml = "<head> <title /> </head> <body c=\"f\"> </body>";
            Assert.Equal("<html><head> <title /> </head> <body c=\"f\"> </body></html>", doc.ToXml());
        }

        [Fact]
        public void RemoveSelf_implies_parent_and_owner() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("head");

            DomElement e = body.RemoveSelf();
            Assert.Equal(0, html.ChildNodes.Count);
            Assert.Same(body, e);
            Assert.Same(doc, e.OwnerDocument);
            Assert.Null(e.ParentNode);
            Assert.Null(e.ParentElement);
        }

        [Fact]
        public void ReplaceWith_should_replace_elements() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            var h1 = body.AppendElement("h1");
            var h3 = doc.CreateElement("h3");

            var result = h1.ReplaceWith(h3, doc.CreateElement("h4"), doc.CreateElement("h5"));
            Assert.Equal("<html><body><h3 /><h4 /><h5 /></body></html>", doc.ToXml());
            Assert.Same(h3, result);
        }

        [Fact]
        public void DescendantsAndSelf_implied_by_document_tree() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            var para1 = body.AppendElement("p");
            var para2 = body.AppendElement("div");
            para1.AppendText("Hello, world!");
            para2.AppendText("Hello, world!");

            Assert.Equal("html body p div",
                         string.Join(" ", html.DescendantsAndSelf.Select(t => t.NodeName)));
            Assert.Equal("body p div",
                         string.Join(" ", html.Descendants.Select(t => t.NodeName)));
        }

        [Fact]
        public void AppendElement_implies_Element_access() {
            DomDocument doc = new DomDocument();
            var svg = doc.AppendElement("svg");
            var g1 = svg.AppendElement("g");
            var g2 = svg.AppendElement("g");
            var g3 = svg.AppendElement("g");

            Assert.Same(g2, svg.Elements[1]);
            Assert.Same(g3, svg.Elements[2]);

            Assert.Same(g2, svg.Element(1));
            Assert.Same(g3, svg.Element(2));

            Assert.Same(g1, svg.Element("g"));
        }

        [Fact]
        public void Attributes_test_detached_element_temporary_attributes() {
            // It is possible that DomElement might be created outside of DomDocument such as this,
            // or that initialization order of element might cause attributes to be created
            DomElement e = new DomElement("detached");
            e.Attribute("hello", "world");

            Assert.Equal("world", e.Attribute("hello"));

            DomDocument doc = new DomDocument();
            doc.Append(e);

            Assert.Equal("world", e.Attribute("hello"));
            e.Attribute("hello", "earth");

            Assert.Equal("hello", e.Attributes[0].Name);
            Assert.Equal("earth", e.Attributes[0].Value);
        }

        [Fact]
        public void PreviousSibling_should_process_correct_value() {
            DomDocument doc = new DomDocument();
            var svg = doc.AppendElement("svg");
            var g1 = svg.AppendElement("g");
            var g2 = svg.AppendElement("g");
            var g3 = svg.AppendElement("g");

            Assert.Same(g1, g2.PreviousSibling);
            Assert.Same(g2, g3.PreviousSibling);
            Assert.Null(g1.PreviousSibling);
        }

        [Fact]
        public void NextSibling_should_process_correct_value() {
            DomDocument doc = new DomDocument();
            var svg = doc.AppendElement("svg");
            var g1 = svg.AppendElement("g");
            var g2 = svg.AppendElement("g");
            var g3 = svg.AppendElement("g");

            Assert.Same(g3, g2.NextSibling);
            Assert.Null(g3.NextSibling);
            Assert.Same(g2, g1.NextSibling);
        }

        [Fact]
        public void PreviousSibling_should_process_single_child() {
            DomDocument doc = new DomDocument();
            var svg = doc.AppendElement("svg");
            var g1 = svg.AppendElement("g");

            Assert.Null(g1.PreviousSibling);
        }

        [Fact]
        public void NextSibling_should_process_single_child() {
            DomDocument doc = new DomDocument();
            var svg = doc.AppendElement("svg");
            var g1 = svg.AppendElement("g");

            Assert.Null(g1.NextSibling);
        }

        [Fact]
        public void PreviousSibling_should_process_single_child_with_space() {
            DomDocument doc = new DomDocument();
            var svg = doc.AppendElement("svg");
            svg.AppendText(" ");
            var g1 = svg.AppendElement("g");
            svg.AppendText(" ");

            Assert.Null(g1.PreviousSibling);
        }

        [Fact]
        public void NextSibling_should_process_single_child_with_space() {
            DomDocument doc = new DomDocument();
            var svg = doc.AppendElement("svg");
            svg.AppendText(" ");
            var g1 = svg.AppendElement("g");
            svg.AppendText(" ");

            Assert.Null(g1.NextSibling);
        }

        [Fact]
        public void Append_to_element_should_unparent_old_diff_documents() {
            DomDocument doc1 = new DomDocument();
            DomDocument doc2 = new DomDocument();
            var head = doc1.AppendElement("head");
            var body  = doc2.AppendElement("body");
            var e = doc1.CreateElement("lang");

            head.Append(e);
            body.Append(e);
            Assert.Equal(0, head.Elements.Count);
            Assert.Equal(1, body.Elements.Count);
            Assert.Same(doc2, e.OwnerDocument);
            Assert.Same(body, e.ParentElement);
        }

        [Fact]
        public void ParentElement_should_be_null_for_root() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");

            Assert.Null(html.ParentElement);
        }

        [Fact]
        public void ParentElement_should_be_available() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            var p = body.AppendElement("p");

            Assert.Same(body, p.ParentElement);
            Assert.Same(html, body.ParentElement);
        }

        [Fact]
        public void AncestorsAndSelf_element_should_enumerate_ancestors() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            var p = body.AppendElement("p");

            Assert.Equal(new [] { "p", "body", "html" },
                         p.AncestorsAndSelf.Select(t => t.NodeName));
        }

        [Fact]
        public void Ancestors_element_should_enumerate_ancestors() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var body = html.AppendElement("body");
            var p = body.AppendElement("p");

            Assert.Equal(new [] { "body", "html" },
                         p.Ancestors.Select(t => t.NodeName));
        }

        [Fact]
        public void Clone_should_unparent() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("t");

            Assert.Same(doc, e.ParentNode);
            Assert.Null(e.Clone().ParentNode);
        }

        [Fact]
        public void Clone_should_reparent_child_elements_and_nodes() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("t");
            e.AppendText("text");
            e.AppendElement("e");

            Assert.Same(e, e.Elements[0].ParentElement);
            Assert.Same(e, e.LastChildNode.ParentElement);

            var clone = e.Clone();
            Assert.Same(clone, clone.Elements[0].ParentElement);
            Assert.Same(clone, clone.LastChildNode.ParentElement);
        }

        [Fact]
        public void Clone_should_copy_attribute_values_not_instances() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("t");
            e.Attribute("a", "b").Attribute("c", "d");
            var clone = e.Clone();

            Assert.True(clone.Attributes.Contains("a"));
            Assert.True(clone.Attributes.Contains("c"));
            Assert.NotSame(e.Attributes["a"],
                           clone.Attributes["a"]);
        }

        [Fact]
        public void Unwrap_nominal() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("m").AppendElement("t");
            e.Attribute("a", "true");
            e.AppendElement("u");
            e.AppendElement("v");

            e.Unwrap();
            Assert.Equal("<m><u /><v /></m>", doc.ToXml());
        }

        [Fact]
        public void RemoveAttributes_nominal() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("m").Attribute("s", "s")
                .Attribute("t", "t");

            e.RemoveAttributes();
            Assert.Equal("<m />", doc.ToXml());
        }

        [Fact]
        public void Unwrap_returns_first_child() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("t");
            var firstChild = e.AppendElement("u");

            Assert.Same(firstChild, e.Unwrap());
        }

        [Fact]
        public void Unwrap_removes_itself_when_empty() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("t");

            Assert.Null(e.Unwrap());
            Assert.Null(doc.DocumentElement);
        }

        [Fact]
        public void Unwrap_cannot_unwrap_root_with_multiple_children() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("t");
            var firstChild = e.AppendElement("u");
            var secondChild = e.AppendElement("u");

            var ex = Record.Exception(() => e.Unwrap());
            // Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal(DomFailure.CannotUnwrapWouldCreateMalformedDocument().Message,
                         ex.Message);
        }

        [Fact]
        public void Wrap_should_apply_element_name_and_return_new_element() {
            DomDocument doc = new DomDocument();
            doc.LoadXml("<html> <div> <s /> </div> </html>");

            var elem = doc.DocumentElement.FirstChild.Wrap("body");
            Assert.Equal("<html> <body><div> <s /> </div></body> </html>", doc.ToXml());
            Assert.Equal("body", elem.Name);
            Assert.Equal(doc.DocumentElement.FirstChild, elem);
        }

        [Fact]
        public void Wrap_should_apply_specified_element_and_return_it() {
            DomDocument doc = new DomDocument();
            var head = doc.CreateElement("head").Attribute("profile", "s");
            doc.LoadXml("<html> <title> <s /> </title> </html>");

            var elem = doc.DocumentElement.FirstChild.Wrap(head);
            Assert.Equal("<html> <head profile=\"s\"><title> <s /> </title></head> </html>", doc.ToXml());
            Assert.Same(elem, head);
        }

        [Fact]
        public void Wrap_should_apply_to_unlinked_nodes() {
            DomDocument doc = new DomDocument();
            var title = doc.CreateElement("title");
            var head = title.Wrap("head");

            Assert.Equal("head", head.Name);
            Assert.Equal("<head><title /></head>", head.ToXml());
        }

        [Fact]
        public void NodeName_is_element_name() {
            DomDocument doc = new DomDocument();
            var title = doc.CreateElement("title");

            Assert.Equal("title", title.NodeName);
        }

        [Fact]
        public void NodeValue_is_null() {
            DomDocument doc = new DomDocument();
            var dd = doc.CreateElement("html");
            Assert.Null(dd.NodeValue);
        }
    }

}
