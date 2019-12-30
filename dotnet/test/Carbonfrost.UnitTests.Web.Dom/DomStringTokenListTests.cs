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

    public class DomStringTokenListTests {

        [Fact]
        public void Add_cannot_contain_whitespace_in_tokens() {
            DomStringTokenList list = new DomStringTokenList();
            Assert.Throws<ArgumentException>(() => list.Add("cannot\tws"));
        }

        [Fact]
        public void Parse_nominal() {
            DomStringTokenList list = DomStringTokenList.Parse(" red \t\tgreen blue \t\r\n");
            Assert.True(list.Contains("red"));
            Assert.True(list.Contains("green"));
            Assert.True(list.Contains("blue"));

            Assert.Equal("red", list[0]);
            Assert.Equal("green", list[1]);
            Assert.Equal("blue", list[2]);
            Assert.Equal("red green blue", list.ToString());
        }

        [Theory]
        [InlineData("")]
        [InlineData((string) null)]
        public void Parse_null_or_empty_string_are_empty(string text) {
            DomStringTokenList list = DomStringTokenList.Parse(text);
            Assert.Equal(DomStringTokenList.Empty, list);
        }

        [Fact]
        public void Add_nominal() {
            DomStringTokenList list = new DomStringTokenList();
            list.Add("nom");

            Assert.True(list.Contains("nom"));
            Assert.Equal(1, list.Count);
            Assert.Equal("nom", list.ToString());
            Assert.Equal(new [] { "nom" }, list.ToArray());
        }

        [Fact]
        public void Contains_and_add_are_case_sensitive() {
            DomStringTokenList list = new DomStringTokenList();
            list.Add("nom");

            Assert.False(list.Contains("NOM"));
        }

        [Fact]
        public void Toggle_implies_Contains() {
            DomStringTokenList list = new DomStringTokenList();
            list.Toggle("nom");
            Assert.True(list.Contains("nom"));

            list.Toggle("nom");
            Assert.False(list.Contains("nom"));
            Assert.Equal("", list.ToString());
        }

        [Fact]
        public void Toggle_explicit_equivalence() {
            DomStringTokenList list = new DomStringTokenList();
            list.Toggle(false, "nom");
            Assert.False(list.Contains("nom"));

            list.Toggle(true, "nom");
            Assert.True(list.Contains("nom"));

            list.Toggle(true, "nom");
            Assert.True(list.Contains("nom"));

            list.Toggle(false, "nom");
            Assert.False(list.Contains("nom"));
        }

        [Fact]
        public void Contains_Add_set_cache_implementation() {
            var list = DomStringTokenList.Parse("1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18");

            Assert.Equal(18, list.Count);
            Assert.Equal("1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18", list.ToString());
            for (int i = 1; i <= 18; i++) {
                Assert.True(list.Contains(i.ToString()));
            }

            Assert.False(list.Add("2"));
            Assert.True(list.Remove("2"));
            Assert.False(list.Contains("2"));
            Assert.True(list.Add("19"));
        }

        [Fact]
        public void AddRange_should_add_multiple_items_nominal() {
            var list = new DomStringTokenList();
            Assert.True(list.AddRange("d", "e"));
            Assert.Equal("d e", list.ToString());
        }

        [Fact]
        public void AddRange_should_return_true_given_new_items() {
            var list = new DomStringTokenList();
            list.AddRange("a", "b", "c");
            Assert.True(list.AddRange("a", "b", "c", "d"));
            Assert.False(list.AddRange("c"));
        }

        [Fact]
        public void Add_should_ignore_empty_or_null() {
            var list = new DomStringTokenList();
            Assert.False(list.Add(string.Empty));
            Assert.False(list.Add(null));
        }

        [Fact]
        public void AddRange_should_ignore_empty_or_null() {
            var list = new DomStringTokenList();
            Assert.False(list.AddRange(string.Empty));
            Assert.False(list.AddRange(null, null));
        }

        [Fact]
        public void Empty_should_be_read_only() {
            Assert.True(DomStringTokenList.Empty.IsReadOnly);
        }
    }
}

