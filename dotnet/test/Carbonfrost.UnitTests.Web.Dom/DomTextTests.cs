//
// Copyright 2013, 2016, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomTextTests {

        [Fact]
        public void CompressWhitespace_should_delete_extra_spaces() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html").AppendText("  \t\t   ");
            html.CompressWhitespace();
            Assert.Equal(" ", html.Data);
        }

        [Theory]
        [InlineData(" \r\n ", "\r\n")]
        [InlineData(" \n ", "\n")]
        [InlineData(" \r ", "\r")]
        [InlineData(" \r\n\n\r ", "\r\n")]
        [InlineData("        \n", "\n")]
        [InlineData("\tl\n   ", " l\n")]
        public void CompressWhitespace_should_preserve_as_new_line(string input, string expected) {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html").AppendText(input);
            html.CompressWhitespace();
            Assert.Equal(expected, html.Data);
        }

        [Fact]
        public void Data_and_basic_property_access() {
            DomDocument doc = new DomDocument();
            string ws = "leading ws";
            var html = doc.AppendElement("html").AppendText(ws);

            Assert.Equal(ws, html.TextContent);
            Assert.Equal(ws, html.Data);
            Assert.Equal(ws, html.NodeValue);
        }

        [Fact]
        public void IsWhitespace_nominal() {
            DomDocument doc = new DomDocument();
            string ws = "  ";
            var text = doc.AppendElement("html").AppendText(ws);
            Assert.True(text.IsWhitespace);
        }

        [Fact]
        public void ReplaceWith_element() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("leading");
            var head = doc.CreateElement("head");
            var result = html.ReplaceWith(head);

            Assert.Equal("<head />", doc.ToXmlString());
            Assert.Same(head, result);
        }

        [Fact]
        public void AppendText_implies_parent_and_owner() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var text = html.AppendText("ws");
            Assert.Equal(0, text.ChildNodes.Count);
            Assert.Same(doc, text.OwnerDocument);
            Assert.Same(html, text.ParentNode);
            Assert.Same(html, text.ParentElement);
        }

        [Fact]
        public void AppendElement_implies_parent_and_owner() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("e");
            Assert.Equal(0, html.ChildNodes.Count);
            Assert.Same(doc, html.OwnerDocument);
            Assert.Same(doc, html.ParentNode);
            Assert.Null(html.ParentElement);
        }

        [Fact]
        public void SetValue_is_equivalent_to_Data_property() {
            DomContainer doc = new DomDocument().AppendElement("s");
            var html = doc.AppendText(" ").SetData("nyc");

            Assert.IsInstanceOf<DomText>(html);
            Assert.Equal("nyc", html.Data);
        }

        [Fact]
        public void HasAttributes_should_be_false() {
            DomContainer doc = new DomDocument().AppendElement("s");
            var text = doc.AppendText(" ");

            Assert.False(text.HasAttributes);
        }

        [Fact]
        public void HasAttribute_should_be_false() {
            DomContainer doc = new DomDocument().AppendElement("s");
            var text = doc.AppendText(" ");

            Assert.False(text.HasAttribute("anything"));
        }

        [Fact]
        public void RemoveAttribute_should_be_nop() {
            DomContainer doc = new DomDocument().AppendElement("s");
            var text = doc.AppendText(" ");

            Assert.Same(text, text.RemoveAttribute("anything"));
        }

        [Fact]
        public void SplitText_should_split_on_index_keep_original_node() {
            const string xml = @"<e>this is some text</e>";
            DomDocument doc = new DomDocument();
            doc.LoadXml(xml);
            var text = ((DomText) doc.DocumentElement.ChildNodes[0]);
            Assert.Same(text, text.SplitText(4));
        }

        [Fact]
        public void NodeName_equals_special_name() {
            DomDocument doc = new DomDocument();
            var pi = doc.CreateText("hello");
            Assert.Equal("#text", pi.NodeName);
        }

        [Fact]
        public void NodeValue_equals_text() {
            DomDocument doc = new DomDocument();
            var pi = doc.CreateText("hello");
            Assert.Equal("hello", pi.NodeValue);
        }

        [Fact]
        public void SplitText_should_split_on_index_follower_is_rest_of_text() {
            const string xml = @"<e>this is some text</e>";
            DomDocument doc = new DomDocument();
            doc.LoadXml(xml);
            var text = ((DomText) doc.DocumentElement.ChildNodes[0]);
            text.SplitText(4);
            Assert.Equal(text.NodeValue, "this");
            Assert.Equal(text.NextSiblingNode.NodeValue, " is some text");
        }

        [Fact]
        public void SplitText_should_split_on_index_and_length_follower_is_rest_of_text() {
            const string xml = @"<e>this is some text</e>";
            DomDocument doc = new DomDocument();
            doc.LoadXml(xml);
            var text = ((DomText) doc.DocumentElement.ChildNodes[0]);
            text.SplitText(4, 3);
            Assert.Equal(text.NodeValue, "this");
            Assert.Equal(text.NextSiblingNode.NodeValue, " is");
            Assert.Equal(text.NextSiblingNode.NextSiblingNode.NodeValue, " some text");
        }

        [Fact]
        public void SplitText_should_split_on_regular_expression_remove_zero_length_texts() {
            const string xml = @"<e>this null false and true splits</e>";
            DomDocument doc = new DomDocument();
            doc.LoadXml(xml);
            var text = ((DomText) doc.DocumentElement.ChildNodes[0]);
            text.SplitText(new Regex("(this|null|false|true)"));
            var items = new [] { text.NodeValue }.Concat(
                text.FollowingSiblingNodes.Select(t => t.NodeValue).ToArray()
            );
            // N.B.: that regex split would usually lead to "" at beginning and end, but
            // we drop that
            Assert.Equal<string>(new string[] { "this", " ", "null", " ", "false", " and ", "true", " splits" }, items);
        }
    }
}


