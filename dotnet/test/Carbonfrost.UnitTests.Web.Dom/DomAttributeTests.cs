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

    public class DomAttributeTests {

        [Fact]
        public void After_sets_attributes_after() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr1 = html.AppendAttribute("lang", "en");
            var attr2 = doc.CreateAttribute("profile", "dotcom");

            attr1.After(attr2);
            Assert.Equal("<html lang=\"en\" profile=\"dotcom\"/>", doc.OuterXml);
        }

        [Fact]
        public void After_should_preserve_order_of_list() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = html.AppendAttribute("first", "");
            var attr1 = doc.CreateAttribute("lang", "en");
            var attr2 = doc.CreateAttribute("profile", "dotcom");
            attr.After(attr1, attr2);
            Assert.Equal("<html first=\"\" lang=\"en\" profile=\"dotcom\"/>", doc.OuterXml);
        }

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
            Assert.Equal("lang", attr.LocalName);
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
            Assert.False(doc.UnlinkedNodes.Contains(attr));
        }

        [Fact]
        public void AppendAttribute_implies_parent_and_owner() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = html.AppendAttribute("lang", "en");

            Assert.Same(doc, attr.OwnerDocument);
            Assert.Same(html, attr.OwnerElement);
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
        public void Constructor_requires_name_argument() {
            Assert.Throws<ArgumentNullException>(() => new DomAttribute((DomName) null));
        }

        [Fact]
        public void Constructor_requires_name_argument_nonempty() {
            DomDocument doc = new DomDocument();
            Assert.Throws<ArgumentException>(() => new DomAttribute(""));
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
        public void ChildNodes_is_a_empty_collection() {
            DomDocument doc = new DomDocument();
            var attr = doc.CreateAttribute("lang", "en");

            Assert.True(attr.ChildNodes.IsReadOnly);
            Assert.Empty(attr.ChildNodes);
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
        public void ReplaceWith_attribute_replace_with_attribute_and_unlinks() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = html.AppendAttribute("lang", "en");
            var attr2 = doc.CreateAttribute("dir", "ltr");

            Assert.Same(attr2, attr.ReplaceWith(attr2));
            Assert.Equal(1, html.Attributes.Count);
            Assert.Equal(attr2, html.Attributes[0]);
            Assert.DoesNotContain(attr2, doc.UnlinkedNodes);
            Assert.Contains(attr, doc.UnlinkedNodes);
        }

        [Fact]
        public void ReplaceWith_attribute_same_is_nop() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = html.AppendAttribute("lang", "en");

            Assert.Same(attr, attr.ReplaceWith(attr));
            Assert.Equal(1, html.Attributes.Count);
        }

        [Fact]
        public void ReplaceWith_using_null_deletes_the_attribute() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html").Attribute("lang", "en");
            var attr = html.Attributes[0];
            attr.ReplaceWith(null);

            Assert.Equal(0, html.Attributes.Count);
        }

        [Fact]
        public void ReplaceWith_updates_attribute_at_same_index() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.Attribute("lang", "en");
            html.Attribute("dir", "rtl");
            var attr = html.Attributes[0];
            attr.ReplaceWith(doc.CreateAttribute("lang", "fr"));

            Assert.Equal(2, html.Attributes.Count);
            Assert.Equal("lang", html.Attributes[0].LocalName);
            Assert.Equal("fr", html.Attributes[0].Value);
        }

        [Fact]
        public void ReplaceWith_works_if_parent_is_unlinked() {
            DomDocument doc = new DomDocument();
            var html = doc.CreateElement("html");
            html.Attribute("lang", "en");

            var alt = doc.CreateAttribute("dir", "rtl");
            var old = html.Attributes[0];
            old.ReplaceWith(alt);

            Assert.Equal(1, html.Attributes.Count);
            Assert.Equal("dir", html.Attributes[0].LocalName);
            Assert.Equal("rtl", html.Attributes[0].Value);
            Assert.True(old.IsUnlinked);
        }

        [Theory]
        [InlineData("exists other1")]
        [InlineData("other1 exists")]
        public void ReplaceWith_when_name_already_exists_preserves_uniqueness(string attributes) {
            var doc = new DomDocument();
            var attr = doc.CreateAttribute("exists", "expected value");
            var e = doc.AppendElement("e");
            foreach (var attrName in attributes.Split()) {
                e.Attribute(attrName, 2);
            }

            // Replacing the second attribute collides with the first by name,
            // so the first by name is removed
            e.Attributes["other1"].ReplaceWith(attr);
            Assert.HasCount(1, e.Attributes);
            Assert.Equal("expected value", e.Attributes[0].Value);
        }

        [Fact]
        public void GetValue_will_apply_type_conversion() {
            DomDocument doc = new DomDocument();
            var time = doc.AppendElement("time");
            var attr = time.AppendAttribute("stamp", "PT3.3S");

            Assert.Equal(TimeSpan.FromSeconds(3.3), attr.GetValue<TimeSpan>());
        }

        [Fact]
        public void GetValue_on_type_conversion_error_throws_FormatException() {
            DomDocument doc = new DomDocument();
            var time = doc.AppendElement("time");
            var attr = time.AppendAttribute("stamp", "00:00:03.3000000");

            Assert.Throws<FormatException>(() => attr.GetValue<TimeSpan>());
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
            Assert.Equal("data-e", attr.NextAttribute.LocalName);
            Assert.Equal("lang", attr.PreviousAttribute.LocalName);
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
        public void NextAttribute_and_PreviousAttribute_adjacent_unlinkd() {
            DomDocument doc = new DomDocument();
            var attr = doc.CreateAttribute("att");

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

        [Fact]
        public void ToString_nominal() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html").Attribute("lang", "en");

            Assert.Equal("lang", html.Attributes[0].ToString());
        }

        [Fact]
        public void TextContent_gets_and_sets_text() {
            DomDocument doc = new DomDocument();
            var attr = doc.CreateAttribute("s");
            attr.TextContent = "text";

            Assert.Equal("text", attr.TextContent);
        }

        [Fact]
        public void OwnerElement_is_the_containing_element() {
            DomDocument doc = new DomDocument();
            var attr = doc.AppendElement("root").AppendAttribute("s");

            Assert.Same(doc.DocumentElement, attr.OwnerElement);
        }

        [Fact]
        public void ParentNode_and_ParentElement_is_null() {
            DomDocument doc = new DomDocument();
            var attr = doc.AppendElement("root").AppendAttribute("s");

            Assert.Null(attr.ParentNode);
            Assert.Null(attr.ParentElement);
        }

        [Fact]
        public void RootNode_is_null() {
            DomDocument doc = new DomDocument();
            var attr = doc.AppendElement("root").AppendAttribute("s");

            Assert.Null(attr.RootNode);
        }

    }
}
