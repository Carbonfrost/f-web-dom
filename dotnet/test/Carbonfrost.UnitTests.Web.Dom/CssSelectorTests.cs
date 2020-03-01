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

using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class CssSelectorTests {

        [Theory]
        [InlineData("tag")]
        [InlineData("tag.class")]
        [InlineData("tag[attr]")]
        [InlineData("tag[attr=pair]")]
        [InlineData("tag[a=b][b=c]")]
        [XInlineData("tag.class>child", Reason = "Ordering in output")]
        [XInlineData("parent>tag>child", Reason = "Ordering in output")]
        [XInlineData("ancestor tag descendant", Reason = "Ordering in output")]
        [XInlineData("tag+next", Reason = "Ordering in output")]
        [XInlineData("tag~sibling", Reason = "Ordering in output")]
        public void Parse_should_roundtrip_string(string query) {
            var css = CssSelector.Parse(query);
            Assert.Equal(query, css.ToString());

        }

        [Theory]
        [InlineData("tag:lt(2)")]
        [InlineData("tag:gt(22)")]
        [InlineData("tag:eq(2)")]
        [InlineData("tag:contains(hello)")]
        [InlineData("tag:containsOwn(hello)")]
        [InlineData("tag:matches(he(l)+o)")]
        [InlineData("tag:matchesOwn(he(l)+o)")]
        public void Parse_should_roundtrip_nonstandard_names(string query) {
            var css = CssSelector.Parse(query);
            Assert.Equal(query, css.ToString());

        }
    }
}
