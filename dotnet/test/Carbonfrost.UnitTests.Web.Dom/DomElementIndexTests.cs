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
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomElementIndexTests {

        [Fact]
        public void Items_will_contain_item_by_name_when_an_attribute_is_added() {
            var doc = new DomDocument();
            var index = doc.CreateAttributeIndex("id");
            var a = doc.AppendElement("root").AppendElement("a");
            a.Id = "id";

            Assert.True(index.IsConnected);
            Assert.SetEqual(new [] { a }, index["id"]);
        }

        [Fact]
        public void Items_will_contain_item_by_name_when_an_element_is_added() {
            var doc = new DomDocument();
            var e = doc.CreateElement("hello").Attribute("id", "id");

            var index = doc.CreateAttributeIndex("id");
            Assume.Empty(index);

            var a = doc.AppendElement("root").Append(e);

            Assert.SetEqual(new [] { e }, index["id"]);
        }

        [Fact]
        public void Items_will_contain_items_by_name_when_index_is_created_after_the_fact() {
            var doc = new DomDocument();
            var a = doc.AppendElement("root").AppendElement("a");
            a.Id = "id";

            var index = doc.CreateAttributeIndex("id");
            Assert.SetEqual(new [] { a }, index["id"]);
        }

        [Fact]
        public void Items_will_remove_item_by_name_when_attribute_is_removed() {
            var doc = new DomDocument();
            var index = doc.CreateAttributeIndex("id");
            var a = doc.AppendElement("root").AppendElement("a").Attribute("id", "a");
            Assume.NotEmpty(index);

            a.RemoveAttributes();
            Assert.Empty(index);
        }

        [Fact]
        public void Items_will_remove_item_by_name_when_element_is_removed() {
            var doc = new DomDocument();
            var index = doc.CreateAttributeIndex("id");
            var a = doc.AppendElement("root").AppendElement("a").Attribute("id", "a");
            Assume.NotEmpty(index);

            a.RemoveSelf();
            Assert.Empty(index);
        }

        [Fact]
        public void Items_will_not_update_after_disconnected() {
            var doc = new DomDocument();
            var index = doc.CreateAttributeIndex("id");
            var a = doc.AppendElement("root").AppendElement("a");
            index.Disconnect();
            a.Id = "id";

            Assert.False(index.IsConnected);
            Assert.DoesNotContainKey("a", index);
        }

        [Fact]
        public void Items_will_throw_if_disposed() {
            var doc = new DomDocument();
            var index = doc.CreateAttributeIndex("id");
            index.Dispose();

            Assert.Throws<ObjectDisposedException>(
                () => index.Count
            );
            Assert.Throws<ObjectDisposedException>(
                () => index["anything"]
            );
        }
    }
}
