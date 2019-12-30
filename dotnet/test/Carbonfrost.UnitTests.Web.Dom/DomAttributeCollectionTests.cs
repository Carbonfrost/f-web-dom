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
    }
}
