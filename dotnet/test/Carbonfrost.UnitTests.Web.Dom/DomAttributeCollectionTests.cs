//
// Copyright 2013, 2016, 2019 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomAttributeCollectionTests {

        [Fact]
        public void Clear_collection_implies_no_parent() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr1 = html.AppendAttribute("lang", "en");
            var attr2 = html.AppendAttribute("dir", "ltr");
            var attr3 = html.AppendAttribute("class", "y");

            Assert.Same(doc, attr1.OwnerDocument);
            Assert.Same(html, attr1.OwnerElement);
            Assert.Null(attr1.ParentElement); // spec
            html.Attributes.Clear();

            Assert.Equal(0, html.Attributes.Count);
            Assert.Same(doc, attr1.OwnerDocument);
            Assert.Null(attr1.OwnerElement); // spec
            Assert.Null(attr1.ParentElement);
        }

        [Fact]
        public void Remove_from_collection_implies_no_parent() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr1 = html.AppendAttribute("lang", "en");

            Assert.Same(doc, attr1.OwnerDocument);
            Assert.Same(html, attr1.OwnerElement);
            Assert.Null(attr1.ParentElement); // spec
            Assert.Equal(0, attr1.AttributePosition);
            Assert.Equal(1, html.Attributes.Count);
            Assert.Equal(0, html.Attributes.IndexOf(attr1));
            Assert.True(html.Attributes.Remove(attr1));
            Assert.Equal(0, html.Attributes.Count);
            Assert.Equal(-1, html.Attributes.IndexOf(attr1));

            Assert.Same(doc, attr1.OwnerDocument);
            Assert.Null(attr1.OwnerElement); // spec
            Assert.Null(attr1.ParentElement);
        }

        [Fact]
        public void ReadOnly_is_read_only() {
            Assert.True(DomAttributeCollection.ReadOnly.IsReadOnly);
        }

        [Fact]
        public void ReadOnly_cannot_add_or_clear() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = html.AppendAttribute("lang", "en");

            Assert.Throws<NotSupportedException>(() => {
                DomAttributeCollection.ReadOnly.Add(attr);
            });

            Assert.Throws<NotSupportedException>(() => {
                DomAttributeCollection.ReadOnly.Clear();
            });
        }

        [Fact]
        public void Indexer_can_set_by_name_and_retrieve_by_name() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.Attributes["class"] = "hello";

            Assert.Equal("hello", html.Attributes["class"]);
        }

        [Fact]
        public void Indexer_can_set_by_name_and_create_attribute() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.Attributes["class"] = "hello";

            Assert.HasCount(1, html.Attributes);
            Assert.Equal("class", html.Attributes[0].Name);
        }

        [Fact]
        public void Remove_supports_removing_by_name() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.Attribute("class", "a").Attribute("id", "b").Attribute("lang", "en");

            Assert.True(html.Attributes.Remove("class"));
            Assert.Equal("id", html.Attributes[0].Name);
            Assert.Equal("lang", html.Attributes[1].Name);
        }

        [Fact]
        public void RemoveAt_supports_removing_by_index() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.Attribute("class", "a").Attribute("id", "b").Attribute("lang", "en");

            html.Attributes.RemoveAt(1);
            Assert.Equal("class", html.Attributes[0].Name);
            Assert.Equal("lang", html.Attributes[1].Name);
        }
    }
}
