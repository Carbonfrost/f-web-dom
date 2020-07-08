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

    public class FrugalMultimapTests {

        [Fact]
        public void Add_should_store_and_Indexer_should_retrieve_multiple_values() {
            var map = new FrugalMultimap<string, string>(StringComparer.Ordinal);
            map.Add("hello", "a");
            map.Add("hello", "b");

            Assert.HasCount(2, map["hello"]);
            Assert.Equal(new [] { "a", "b" }, map["hello"].Cast<string>());
        }

        [Fact]
        public void Add_should_store_and_Remove_should_remove_values() {
            var map = new FrugalMultimap<string, string>(StringComparer.Ordinal);
            map.Add("hello", "a");
            map.Add("hello", "b");
            map.Add("hello", "c");
            Assert.True(map.Remove("hello", "a"));

            Assert.HasCount(2, map["hello"]);
            Assert.Equal(new [] { "b", "c" }, map["hello"].Cast<string>());
        }

        [Fact]
        public void Add_should_not_store_duplicate_values() {
            var map = new FrugalMultimap<string, string>(StringComparer.Ordinal);
            map.Add("hello", "v");
            map.Add("hello", "v");

            Assert.HasCount(1, map);
        }

        [Fact]
        public void Remove_returns_false_on_missing_key_value_pair() {
            var map = new FrugalMultimap<string, string>(StringComparer.Ordinal);
            map.Add("hello", "a");
            Assert.False(map.Remove("hello", "z"));
        }

        [Fact]
        public void Remove_returns_true_on_key_value_pair_present() {
            var map = new FrugalMultimap<string, string>(StringComparer.Ordinal);
            map.Add("hello", "a");
            Assert.True(map.Remove("hello", "a"));
        }

        [Fact]
        public void Keys_and_whole_list_is_empty_on_removed_all_items() {
            var map = new FrugalMultimap<string, string>(StringComparer.Ordinal);
            map.Add("hello", "a");
            map.Remove("hello", "a");

            Assert.Null(map["hello"]);
            Assert.Empty(map);
        }

        [Fact]
        public void GetEnumerator_should_process_all_key_value_pairs() {
            var map = new FrugalMultimap<string, string>(StringComparer.Ordinal);
            map.Add("a", "a");
            map.Add("a", "b");
            map.Add("b", "b");

            Assert.Equal(new [] {
                new KeyValuePair<string, string>("a", "a"),
                new KeyValuePair<string, string>("a", "b"),
                new KeyValuePair<string, string>("b", "b"),
            }, map.ToArray());
        }
    }
}


