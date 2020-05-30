//
// Copyright 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

    public class DomNodeCollectionTests {

        public IEnumerable<bool> UseLinkedList {
            get {
                return new [] {
                    true,
                    false,
                };
            }
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void Add_should_move_extant_child_to_end(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);
            var head = doc.CreateElement("head");
            var body = doc.CreateText(" ");
            doc.ChildNodes.Add(head);
            doc.ChildNodes.Add(body);
            doc.ChildNodes.Add(head);

            Assert.HasCount(2, doc.ChildNodes);
            Assert.Equal("head", doc.ChildNodes[1].NodeName);
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void NodePosition_should_update_with_add(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);
            var head = doc.CreateElement("head");
            var body = doc.CreateText(" ");
            doc.ChildNodes.Add(head);
            doc.ChildNodes.Add(body);

            Assert.Equal(0, head.NodePosition);
            Assert.Equal(1, body.NodePosition);
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void NodePosition_should_update_with_insert(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);
            var head = doc.CreateElement("head");
            doc.ChildNodes.Add(doc.CreateText(" "));
            doc.ChildNodes.Add(doc.CreateText(" "));
            doc.ChildNodes.Insert(0, head);

            Assert.Equal(0, head.NodePosition);
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void Add_should_add_to_end_nominal(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);
            var a = doc.CreateElement("a");
            doc.ChildNodes.Add(a);

            Assert.Equal(1, doc.ChildNodes.Count);
            Assert.Same(a, doc.ChildNodes[0]);
            Assert.Equal(0, a.NodePosition);
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void Add_should_add_to_end_multiple_times(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);

            var a = doc.CreateElement("a");
            var b = doc.CreateText("  ");
            doc.ChildNodes.Add(a);
            doc.ChildNodes.Add(b);

            Assert.Equal(2, doc.ChildNodes.Count);
            Assert.Same(a, doc.ChildNodes[0]);
            Assert.Same(b, doc.ChildNodes[1]);
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void Insert_should_add_to_end_given_index_empty(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);
            var r = doc.CreateElement("r");
            var a = doc.CreateElement("a");

            r.ChildNodes.Insert(0, a);
            Assert.Equal(1, r.ChildNodes.Count);
            Assert.Same(a, r.ChildNodes[0]);
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void Insert_should_add_to_end_given_index(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);
            var r = doc.AppendElement("r");
            r.AppendElement("a");
            var b = doc.CreateElement("b");

            r.ChildNodes.Insert(1, b);
            Assert.Equal(2, r.ChildNodes.Count);
            Assert.Same(b, r.ChildNodes[1]);
        }

        [Theory]
        [InlineData(0, 1, "a c")]
        [InlineData(0, 0, "a b c")]
        [InlineData(0, 2, "b a")]
        [InlineData(2, 0, "c b")]
        [InlineData(1, 0, "b c")]
        public void Set_should_remove_extant_child_first(int from, int to, string expected) {
            DomDocument doc = new DomDocument();
            var r = doc.CreateElement("r");
            doc.Append(r);
            doc.CreateElement("a").AppendTo(r);
            doc.CreateElement("b").AppendTo(r);
            doc.CreateElement("c").AppendTo(r);

            r.ChildNodes[to] = r.ChildNodes[from];
            Assert.Equal(expected, string.Join(" ", r.Elements.Select(t => t.NodeName)));
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void IndexOf_should_obtain_index_nominal(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);
            var a = doc.CreateElement("a");
            var b = doc.CreateText("  ");
            doc.ChildNodes.Add(a);
            doc.ChildNodes.Add(b);

            Assert.Equal(0, doc.ChildNodes.IndexOf(a));
            Assert.Equal(1, doc.ChildNodes.IndexOf(b));
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void GetEnumerator_should_enumerate_first_and_second(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);
            var a = doc.CreateElement("a");
            var b = doc.CreateText("  ");
            doc.ChildNodes.Add(a);
            doc.ChildNodes.Add(b);

            Assert.Equal(2, doc.ChildNodes.Count);
            Assert.Same(a, doc.ChildNodes.First());
            Assert.Same(b, doc.ChildNodes.Skip(1).First());
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void GetEnumerator_should_use_outside_mechanics(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);
            doc.AppendElement("t");
            var e = doc.ChildNodes.GetEnumerator();
            Assert.Throws<InvalidOperationException>(() => e.Current);
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void GetEnumerator_should_apply_to_empty_list(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);
            var e = doc.ChildNodes.GetEnumerator();

            Assert.False(e.MoveNext());
        }

        [Theory]
        [PropertyData("UseLinkedList")]
        public void Clear_should_reset_siblings(bool useLinkedList) {
            DomDocument doc = new DomDocument(useLinkedList);
            var a = doc.CreateElement("a");
            doc.ChildNodes.Add(a);
            doc.ChildNodes.Clear();

            Assert.Equal(0, doc.ChildNodes.Count);
            Assert.Null(a.PreviousSibling);
            Assert.Null(a.NextSibling);
        }

        [Fact]
        public void AddRange_supports_moving_from_other_node() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.AppendElement("a");
            html.AppendElement("b");
            html.AppendElement("c");

            var element = doc.CreateElement("ok");
            element.AppendElement("d");
            element.ChildNodes.AddRange(html.ChildNodes);

            Assert.Equal("<ok><d/><a/><b/><c/></ok>", element.OuterXml);
        }

        [Fact]
        public void InsertRange_supports_moving_from_other_node() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            html.AppendElement("a");
            html.AppendElement("b");
            html.AppendElement("c");

            var element = doc.CreateElement("ok");
            element.AppendElement("d");
            element.ChildNodes.InsertRange(0, html.ChildNodes);

            Assert.Equal("<ok><a/><b/><c/><d/></ok>", element.OuterXml);
        }
    }
}
