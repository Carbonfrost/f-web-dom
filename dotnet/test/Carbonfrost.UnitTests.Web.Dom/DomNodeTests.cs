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

    public class DomNodeTests {

        [Fact]
        public void Append_implies_parent() {

            DomDocument doc = new DomDocument();
            var html = doc.CreateElement("html");
            doc.Append(html);
            var body = doc.CreateElement("body");
            html.Append(body);
            Assert.Same(doc, html.ParentNode);
            Assert.Same(html, body.ParentNode);
            Assert.Same(body, html.ChildNode(0));
            Assert.Same(body, html.ChildNodes[0]);
            Assert.Equal(new DomNode[] { body }, html.ChildNodes);
        }

        [Fact]
        public void Append_should_move_extant_child_to_end() {
            DomDocument doc = new DomDocument();
            var html = doc.CreateElement("html");
            doc.Append(html);
            var head = doc.CreateElement("head").AppendTo(html);
            doc.CreateElement("body").AppendTo(html);

            html.Append(head);
            Assert.HasCount(2, html.ChildNodes);
            Assert.HasCount(2, html.Elements);
            Assert.Equal("head", html.Elements[1].NodeName);
        }

        [Theory]
        [InlineData("")]
        [InlineData((string) null, Name = "null")]
        public void Attribute_illegal_arguments(string attr) {
            DomDocument doc = new DomDocument();
            var html = doc.CreateElement("html");
            Assert.Throws<ArgumentException>(() => {
                html.Attribute(attr);
            });
        }

        [Fact]
        public void BaseUri_should_inherit() {
            DomDocument doc = new DomDocument();
            var html = doc.CreateElement("html");
            var example = new Uri("https://example.com");
            html.BaseUri = example;

            doc.Append(html);
            var body = doc.CreateElement("body");
            html.Append(body);

            Assert.Equal(example, html.BaseUri);
            Assert.Equal(example, body.BaseUri);
        }

        [Fact]
        public void BaseUri_should_inherit_on_base_uri_nulled_out() {
            var example1 = new Uri("https://example.com");
            var example2 = new Uri("https://test.example");

            DomDocument doc = new DomDocument();
            var html = doc.CreateElement("html");
            html.BaseUri = example1;

            var body = doc.CreateElement("body");
            body.BaseUri = example2;

            doc.Append(html);
            html.Append(body);

            // Setting this explicitly show return to the inherited balue
            Assume.Equal(example2, body.BaseUri);
            body.BaseUri = null;

            Assert.Equal(example1, body.BaseUri);
        }

        [Fact]
        public void Closest_should_return_closest_element() {
            var doc = new DomDocument();
            var self = doc.AppendElement("a").Attribute("class", "clear").AppendElement("a").AppendElement("span");
            Assert.Same(doc.DocumentElement, self.Closest("a.clear"));
        }

        [Fact]
        public void Closest_should_return_self_if_it_matches() {
            var doc = new DomDocument();
            var self = doc.AppendElement("a").Attribute("class", "clear").AppendElement("span").AppendElement("span");
            Assert.Same(self, self.Closest("span"));
        }

        [Fact]
        public void FollowingSiblingNodes_should_get_the_siblings_nominal() {
            var doc = new DomDocument();
            var root = doc.AppendElement("root");
            var a = root.AppendElement("a");
            root.AppendElement("b");
            root.AppendElement("c");
            root.AppendText("d");
            Assert.Equal("bc#text", string.Concat(a.FollowingSiblingNodes.Select(n => n.NodeName)));
        }

        [Theory]
        [InlineData("")]
        [InlineData((string) null, Name = "null")]
        public void HasAttribute_illegal_arguments(string attr) {
            DomDocument doc = new DomDocument();
            var html = doc.CreateElement("html");
            Assert.Throws<ArgumentException>(() => {
                html.HasAttribute(attr);
            });
        }

        [Fact]
        public void PrecedingSiblingNodes_should_get_the_siblings_nominal() {
            var doc = new DomDocument();
            var root = doc.AppendElement("root");
            root.AppendElement("a");
            root.AppendElement("b");
            root.AppendElement("c");
            root.AppendText("d");
            var e = root.AppendElement("e");
            Assert.Equal("abc#text", string.Concat(e.PrecedingSiblingNodes.Select(n => n.NodeName)));
        }

        [Fact]
        public void Prepend_should_apply_correct_order() {
            var doc = new DomDocument();
            var element = doc.AppendElement("html");

            element.Prepend(doc.CreateElement("body"));
            element.Prepend(doc.CreateElement("head"));
            Assert.Equal("head", doc.DocumentElement.Elements[0].Name);
            Assert.Equal("body", doc.DocumentElement.Elements[1].Name);
        }

        [Fact]
        public void Prepend_should_apply_correct_order_two() {
            var doc = new DomDocument();
            var element = doc.AppendElement("html");
            element.Prepend(doc.CreateElement("head"), doc.CreateElement("body"));

            Assert.Equal("head", doc.DocumentElement.Elements[0].Name);
            Assert.Equal("body", doc.DocumentElement.Elements[1].Name);
        }

        [Fact]
        public void Prepend_should_apply_correct_order_three() {
            var doc = new DomDocument();
            var element = doc.AppendElement("html");
            element.Prepend(doc.CreateElement("head"), doc.CreateElement("body"), doc.CreateElement("tail"));

            Assert.Equal("head body tail", string.Join(" ", doc.DocumentElement.Elements.Select(t => t.Name)));
        }

        [Fact]
        public void Prepend_should_apply_correct_order_enumeration() {
            var doc = new DomDocument();
            var element = doc.AppendElement("html");
            var items = Enumerable.Range(1, 6).Select(t => doc.CreateElement("x" + t));
            element.Prepend(items);

            Assert.Equal("x1 x2 x3 x4 x5 x6", string.Join(" ", doc.DocumentElement.Elements.Select(t => t.Name)));
        }

        [Fact]
        public void Prepend_should_apply_to_null() {
            DomDocument doc = new DomDocument();
            Assert.DoesNotThrow(() => doc.Prepend((DomNode) null));
            Assert.Same(doc, doc.Prepend((DomNode) null));

            Assert.DoesNotThrow(() => doc.Prepend((IEnumerable<DomNode>) null));
            Assert.Same(doc, doc.Prepend((IEnumerable<DomNode>) null));
        }

        [Fact]
        public void SetName_nominal() {
          var doc = new DomDocument();
          var h4 = doc.AppendElement("h4");
          h4.Attribute("a", "true");
          h4.AppendElement("b");
          h4.AppendElement("i");

          var result = h4.SetName("h5");
          Assert.NotSame(h4, result); // should create a new one
          Assert.Equal("<h5 a=\"true\"><b /><i /></h5>", doc.ToXml());
        }

        [Theory]
        [InlineData("", "", "")]
        [InlineData("", (string) null, "")]
        [InlineData((string) null, "x", "")]
        [InlineData("", "b", "")]
        [InlineData("a b c", "b", "a c")]
        [InlineData("a b c", "b a", "c")]
        [InlineData("b", "b", "")]
        public void RemoveClass_should_remove_from_class_attribute(string start, string removing, string expected) {
            DomDocument doc = new DomDocument();
            var e = doc.CreateElement("s").Attribute("class", start);
            e.RemoveClass(removing);
            Assert.Equal(expected, e.Attribute("class"));
        }

        [Theory]
        [InlineData("", "", "")]
        [InlineData("", "b", "b")]
        [InlineData("", (string) null, "")]
        [InlineData((string) null, "x", "x")]
        [InlineData("a b c", "b", "a b c")]
        [InlineData("a b c", "b a", "a b c")]
        public void AddClass_should_add_to_class_attribute(string start, string adding, string expected) {
            DomDocument doc = new DomDocument();
            var e = doc.CreateElement("s").Attribute("class", start);
            e.AddClass(adding);
            Assert.Equal(expected, e.Attribute("class"));
        }

        [Theory]
        [InlineData("", "", false)]
        [InlineData("a", "", false)]
        [InlineData("a", "a", true)]
        [InlineData("a b", "a", false)]
        [InlineData("a b", "b a c", true)]
        public void HasClass_should_detect_classes(string name, string attr, bool expected) {
            DomDocument doc = new DomDocument();
            var e = doc.CreateElement("s").Attribute("class", attr);
            Assert.Equal(expected, e.HasClass(name));
        }

        [Fact]
        public void FollowingSiblings_should_obtain_inorder_following_elements() {
            string xml = @"<root>
                              <m>
                               <a />
                               <b>
                                <c>
                                  <d />
                                </c>
                               </b><e />
                           </m><f />
                           </root>";
            DomDocument doc = new DomDocument();
            doc.LoadXml(xml);
            var a = doc.DocumentElement.FirstChild.FirstChild;
            Assert.Equal("bcdef", string.Concat(a.FollowingSiblings.Select(t => t.Name)));
        }

        [Fact]
        public void FollowingNodes_should_obtain_inorder_following_nodes() {
            string xml = @"<root>
                              <m>
                               <a />
                               <b>
                                <c>
                                  <d />
                                </c>
                               </b><e />
                           </m><f />
                           </root>";
            DomDocument doc = new DomDocument();
            doc.LoadXml(xml);
            var a = doc.DocumentElement.FirstChild.FirstChild;
            Assert.Equal("#text b #text c #text #text d #text e #text f #text", string.Join(" ", a.FollowingNodes.Select(t => t.NodeName)));
        }

        [Fact]
        public void PrecedingNodes_should_obtain_inorder_preceding_nodes() {
            string xml = @"<root>
                              <m>
                               <a />
                               <b>
                                <c>
                                  <d />
                                </c>
                               </b><e />
                           </m><f />
                           </root>";
            DomDocument doc = new DomDocument();
            doc.LoadXml(xml);
            var e = doc.QuerySelector("e");
            Assert.Equal("#document root #text m #text a #text b #text c #text #text d #text", string.Join(" ", e.PrecedingNodes.Select(t => t.NodeName)));
        }

        [Fact]
        public void NodeDepth_should_be_zero_outside_document() {
            DomDocument doc = new DomDocument();
            var e = doc.CreateElement("e");
            Assert.Equal(0, e.NodeDepth);
        }

        [Fact]
        public void NodeDepth_should_be_correct_value_nominal_case() {
            DomDocument doc = new DomDocument();
            var e = doc.AppendElement("a").AppendElement("b").AppendElement("c");
            Assert.Equal(3, e.NodeDepth);
        }
    }
}

