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
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomContainerTests {

        [Fact]
        public void AppendElement_implies_parent_relationship() {
            DomDocument doc = new DomDocument();
            var ele = doc.AppendElement("html");
            Assert.Same(doc, ele.ParentNode);
            Assert.Same(ele, doc.DocumentElement);
            Assert.Empty(doc.DocumentElement.ChildNodes);
            Assert.False(doc.UnlinkedNodes.Contains(ele));
        }

        [Fact]
        public void AppendElement_should_throw_on_empty_string() {
            var doc = new DomDocument();
            Assert.Throws<ArgumentException>(() => doc.AppendElement(""));
        }

        [Fact]
        public void AppendElement_should_throw_on_ws() {
            var doc = new DomDocument();
            Assert.Throws<ArgumentException>(() => doc.AppendElement(" s"));
        }

        [Fact]
        public void OuterXml_set_should_replace_content() {
            var doc = new DomDocument();
            var ele = doc.AppendElement("w");
            string xml = "<html><head/><body/></html>";;
            ele.OuterXml = xml;

            Assert.Equal("html", doc.DocumentElement.NodeName);
            Assert.Equal(xml, doc.OuterXml);
            Assert.True(ele.IsUnlinked);
        }

        [Fact]
        public void RemoveChildNodes_should_remove_child_nodes() {
            var doc = new DomDocument();
            var ele = doc.AppendElement("w");
            var x = ele.AppendElement("x");
            var y = ele.AppendElement("y");
            var z = ele.AppendElement("z");

            Assert.Same(ele, ele.RemoveChildNodes());
            Assert.Equal("<w/>", doc.OuterXml);
            Assert.True(x.IsUnlinked);
            Assert.True(y.IsUnlinked);
            Assert.True(z.IsUnlinked);
        }

        [Fact]
        public void RemoveChildNodes_should_remove_child_nodes_already_unlinked() {
            var doc = new DomDocument();
            var ele = doc.CreateElement("w");
            var x = ele.AppendElement("x");
            var y = ele.AppendElement("y");
            var z = ele.AppendElement("z");

            Assert.Same(ele, ele.RemoveChildNodes());
            Assert.HasCount(0, ele.ChildNodes);
            Assert.True(x.IsUnlinked);
            Assert.True(y.IsUnlinked);
            Assert.True(z.IsUnlinked);
        }

        [Fact]
        public void Descendant_should_obtain_element_by_name() {
            var doc = new DomDocument();
            var z = doc.AppendElement("x").AppendElement("y").AppendElement("z");

            Assert.Same(z, doc.Descendant("z"));
        }

        [Fact]
        public void Element_should_obtain_element_by_name() {
            var x = new DomDocument().AppendElement("x");
            var y = x.AppendElement("y");

            Assert.Same(y, x.Element("y"));
        }

        [Fact]
        public void AddRange_appends_elements_by_type() {
            var root = new DomDocument().AppendElement("root");

            root.AddRange(
                "text",
                200,
                root.OwnerDocument.CreateElement("h"),
                root.OwnerDocument.CreateAttribute("l", "a"),
                new [] { "a", "b" , "c"},
                new List<int> { 200 }
            );

            Assert.Equal("<root l=\"a\">text200<h/>abc200</root>", root.OuterXml);
        }
    }
}
