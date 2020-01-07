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
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomAttributeTests {

        [Fact]
        public void Attribute_implies_new_instance() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.Attribute("lang", "en");
            DomAttribute attr = html.Attributes[0];
            Assert.Equal(1, html.Attributes.Count);
            Assert.Equal(0, html.ChildNodes.Count);

            // Excludes attributes
            Assert.Equal("en", attr.Value);
            Assert.Equal("lang", attr.Name);
            Assert.False(doc.UnlinkedNodes.Contains(attr));
        }

        [Fact]
        public void Attribute_implies_parent_and_owner() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.Attribute("lang", "en");

            DomAttribute attr = html.Attributes[0];
            Assert.Same(doc, attr.OwnerDocument);
            Assert.Same(html, attr.OwnerElement);
            Assert.Null(attr.ParentNode); // per spec
            Assert.False(doc.UnlinkedNodes.Contains(attr));
        }

        [Fact]
        public void AppendAttribute_implies_parent_and_owner() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = html.AppendAttribute("lang", "en");

            Assert.Same(doc, attr.OwnerDocument);
            Assert.Same(html, attr.OwnerElement);
            Assert.Null(attr.ParentNode); // per spec
        }

        [Fact]
        public void Attribute_set_dom_value_getter() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var classList = new DomStringTokenList();
            html.Attribute("class", classList);

            Assert.Equal(string.Empty, html.Attribute("class"));
            classList.Add("cool");
            classList.Add("down");
            Assert.Equal("cool down", html.Attribute("class"));
        }

        [Fact]
        public void Attribute_set_dom_value_setter() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var classList = new DomStringTokenList();
            html.Attribute("class", classList);
            html.Attribute("class", "heat up");
            Assert.Contains("heat", classList);
            Assert.Contains("up", classList);
        }

        [Fact]
        public void RemoveSelf_implies_parent_and_owner() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.Attribute("lang", "en");

            DomAttribute attr = html.Attributes[0].RemoveSelf();
            Assert.Equal(0, html.Attributes.Count);
            Assert.Same(doc, attr.OwnerDocument);
            Assert.Null(attr.ParentNode);
        }

        // TODO Similar RemoveSelf tests with DomElement

        [Fact]
        public void Append_new_attributes_implies_add() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = doc.CreateAttribute("lang", "en");

            html.Append(attr);

            Assert.Equal(1, html.Attributes.Count);
            Assert.Equal("en", html.Attribute("lang"));
            Assert.Same(doc, attr.OwnerDocument);
        }

        [Fact]
        public void CreateAttribute_is_unlinked() {
            DomDocument doc = new DomDocument();
            var attr = doc.CreateAttribute("lang", "en");

            Assert.Same(doc, attr.OwnerDocument);
        }

        [Fact]
        public void AppendAttribute_duplicate_name_error() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = html.AppendAttribute("lang", "en");

            Assert.Throws<ArgumentException>(() => {
                html.AppendAttribute("lang", "fr");
            });

            Assert.Throws<ArgumentException>(() => {
                html.Append(doc.CreateAttribute("lang", "fr"));
            });

            Assert.Throws<ArgumentException>(() => {
                html.Attributes.Add(doc.CreateAttribute("lang", "fr"));
            });
        }

        [Fact]
        public void AppendAttribute_duplicate_name_same_instance() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = html.AppendAttribute("lang", "en");
            Assert.Equal(1, html.Attributes.Count);

            // TODO Should this move attribute to end of collection?

            html.Append(attr); // legal
            Assert.Equal(1, html.Attributes.Count);
        }

        [Fact]
        public void RemoveSelf_implies_attribute_count() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = html.AppendAttribute("lang", "en");

            Assert.Equal(1, html.Attributes.Count);
            attr.RemoveSelf();

            Assert.Equal(0, html.Attributes.Count);
        }

        [Fact]
        public void ReplaceWith_attribute_replace_with_attribute() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = html.AppendAttribute("lang", "en");
            var attr2 = doc.CreateAttribute("dir", "ltr");

            Assert.Same(attr2, attr.ReplaceWith(attr2));
            Assert.Equal(1, html.Attributes.Count);
            Assert.Same(attr2, html.Attributes[0]);
            Assert.True(doc.UnlinkedNodes.Contains(attr));
            Assert.False(doc.UnlinkedNodes.Contains(attr2));
        }

        [Fact]
        public void NextAttribute_attribute_adjacent_nominal() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.Attribute("lang", "en");
            html.Attribute("dir", "ltr");
            html.Attribute("data-e", "e");

            DomAttribute attr = html.Attributes[1];
            Assert.Equal(1, attr.AttributePosition);
            Assert.Equal("data-e", attr.NextAttribute.Name);
            Assert.Equal("lang", attr.PreviousAttribute.Name);
            Assert.Null(attr.PreviousSiblingNode); // per spec
            Assert.Null(attr.NextSiblingNode);
        }

        [Fact]
        public void NextAttribute_and_PreviousAttribute_adjacent_singleton() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.Attribute("lang", "en");

            DomAttribute attr = html.Attributes[0];
            Assert.Null(attr.NextAttribute);
            Assert.Null(attr.PreviousAttribute);
        }

        [Fact]
        public void Append_to_element_should_add_attribute() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = doc.CreateAttribute("lang");

            html.Append(attr);
            Assert.Equal(1, html.Attributes.Count);
        }

        [Fact]
        public void Append_to_element_should_unparent_old() {
            DomDocument doc = new DomDocument();
            var root = doc.AppendElement("html");
            var head = root.AppendElement("head");
            var body  = root.AppendElement("body");
            var attr = doc.CreateAttribute("lang");

            head.Append(attr);
            body.Append(attr);
            Assert.Equal(0, head.Attributes.Count);
            Assert.Equal(1, body.Attributes.Count);
            Assert.Same(body, attr.OwnerElement);
        }

        [Fact]
        public void Append_to_element_should_unparent_old_diff_documents() {
            DomDocument doc1 = new DomDocument();
            DomDocument doc2 = new DomDocument();
            var head = doc1.AppendElement("head");
            var body  = doc2.AppendElement("body");
            var attr = doc1.CreateAttribute("lang");

            head.Append(attr);
            body.Append(attr);
            Assert.Equal(0, head.Attributes.Count);
            Assert.Equal(1, body.Attributes.Count);
            Assert.Same(body, attr.OwnerElement);
            Assert.Same(doc2, attr.OwnerDocument);
        }

        [Fact]
        public void NodeName_equals_attribute_name() {
            var doc = new DomDocument();
            var attr = doc.CreateAttribute("lang");
            Assert.Equal("lang", attr.NodeName);
        }

        [Fact]
        public void NodeName_equals_attribute_value() {
            var doc = new DomDocument();
            var attr = doc.CreateAttribute("lang");
            attr.Value = "en-us";
            Assert.Equal("en-us", attr.NodeValue);
        }
    }
}
