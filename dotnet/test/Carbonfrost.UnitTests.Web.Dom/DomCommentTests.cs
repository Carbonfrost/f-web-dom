//
// Copyright 2016, 2019 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Text.RegularExpressions;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomCommentTests {

        [Fact]
        public void Attributes_is_null() {;
            var comment = new DomDocument().CreateComment("");
            Assert.Null(comment.Attributes);
        }

        [Fact]
        public void HasAttributes_is_false() {
            var comment = new DomDocument().CreateComment("");
            Assert.False(comment.HasAttributes);
        }

        [Fact]
        public void Data_contains_text() {
            var comment = new DomDocument().CreateComment("data");
            Assert.Equal("data", comment.Data);
        }

        [Fact]
        public void Text_contains_text() {
            var comment = new DomDocument().CreateComment("data");
            Assert.Equal("data", comment.Text);
        }

        [Fact]
        public void TextContent_contains_text() {
            var comment = new DomDocument().CreateComment("data");
            Assert.Equal("data", comment.TextContent);
        }

        [Fact]
        public void NodeName_equals_attribute_name() {
            var doc = new DomDocument();
            var comment = doc.CreateComment("");
            Assert.Equal("#comment", comment.NodeName);
        }

        [Fact]
        public void NodeValue_equals_text() {
            DomDocument doc = new DomDocument();
            var comment = doc.CreateComment("hello");
            Assert.Equal("hello", comment.NodeValue);
        }

        [Fact]
        public void SplitText_should_split_on_index_follower_is_rest_of_text() {
            const string xml = @"<e><!--this is some text--></e>";
            DomDocument doc = new DomDocument();
            doc.LoadXml(xml);
            var text = ((DomComment) doc.DocumentElement.ChildNodes[0]);
            text.SplitText(4);
            Assert.Equal(text.NodeValue, "this");
            Assert.Equal(text.NextSiblingNode.NodeValue, " is some text");
        }

        [Fact]
        public void SplitText_should_split_on_index_and_length_follower_is_rest_of_text() {
            const string xml = @"<e><!--this is some text--></e>";
            DomDocument doc = new DomDocument();
            doc.LoadXml(xml);
            var text = ((DomComment) doc.DocumentElement.ChildNodes[0]);
            text.SplitText(4, 3);
            Assert.Equal(text.NodeValue, "this");
            Assert.Equal(text.NextSiblingNode.NodeValue, " is");
            Assert.Equal(text.NextSiblingNode.NextSiblingNode.NodeValue, " some text");
        }

        [Fact]
        public void SplitText_should_split_on_regular_expression_remove_zero_length_texts() {
            const string xml = @"<e><!--this null false and true splits--></e>";
            DomDocument doc = new DomDocument();
            doc.LoadXml(xml);
            var text = ((DomComment) doc.DocumentElement.ChildNodes[0]);
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
