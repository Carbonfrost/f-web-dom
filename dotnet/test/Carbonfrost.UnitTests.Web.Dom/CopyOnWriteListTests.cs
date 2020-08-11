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

    public class CopyOnWriteListTests {

        [Fact]
        public void Add_does_not_cause_concurrent_modifications() {
            var list = new CopyOnWriteList<int> {
                1, 2, 3
            };
            var e = list.GetEnumerator();

            // Adding an item should not cause concurrent modifications
            list.Add(4);
            Assert.DoesNotThrow(() => e.MoveNext());
        }

        [Fact]
        public void Add_does_not_enumerate_added_items() {
            var list = new CopyOnWriteList<int> {
                1, 2, 3
            };
            var e = list.GetEnumerator();
            Assert.True(e.MoveNext());
            Assert.Equal(1, e.Current);

            // Adding an item should not cause concurrent modifications
            list.Add(4);
            Assert.DoesNotThrow(() => e.MoveNext());
            Assert.Equal(2, e.Current);

            Assert.True(e.MoveNext());
            Assert.Equal(3, e.Current);

            Assert.False(e.MoveNext());
        }

        [Fact]
        public void Writes_to_original_are_not_visible_to_clones() {
            var items = new CopyOnWriteList<string>();
            items.Add("test");
            Assert.Equal("test", items[0]);

            var clone = items.Clone();
            var clone2 = items.Clone();

            Assert.True(items.HasSameBacking(clone));
            Assert.True(items.HasSameBacking(clone2));

            items[0] = "no";

            Assert.False(items.HasSameBacking(clone));
            Assert.False(items.HasSameBacking(clone2));
            Assert.True(clone.HasSameBacking(clone2));

            Assert.Equal("test", clone[0]);
            Assert.Equal("test", clone2[0]);
        }

        [Fact]
        public void Writes_to_clones_are_not_visible_to_original() {
            var items = new CopyOnWriteList<string>();
            items.Add("test");
            Assert.Equal("test", items[0]);

            var clone = items.Clone();
            var clone2 = items.Clone();

            Assert.True(items.HasSameBacking(clone));
            Assert.True(items.HasSameBacking(clone2));

            clone[0] = "2";
            Assert.False(items.HasSameBacking(clone));
            Assert.False(clone2.HasSameBacking(clone));
            Assert.True(items.HasSameBacking(clone2));

            clone2[0] = "3";
            Assert.False(items.HasSameBacking(clone2));

            Assert.Equal("test", items[0]);
            Assert.Equal("2", clone[0]);
        }
    }
}
