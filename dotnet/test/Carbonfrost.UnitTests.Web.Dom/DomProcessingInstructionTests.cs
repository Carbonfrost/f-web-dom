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

using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomProcessingInstructionTests {

        [Fact]
        public void Clone_will_clone_annotations_copied_to_new_node() {
            var doc = new DomDocument();

            var node = doc.CreateProcessingInstruction("a").AddAnnotation(new object());
            var clone = node.Clone();

            Assert.HasCount(1, clone.AnnotationList.OfType<object>());
        }

        [Fact]
        public void CreateProcessingInstruction_nominal() {
            DomDocument doc = new DomDocument();
            var pi = doc.CreateProcessingInstruction("hello", "world");
            Assert.Equal("world", pi.TextContent);
            Assert.Equal("world", pi.Data);
            Assert.Equal("hello", pi.Target);
            Assert.Equal("world", pi.NodeValue);
            Assert.Null(pi.Prefix);
            Assert.Null(pi.LocalName);
            Assert.Null(pi.NamespaceUri);
            Assert.Null(pi.Name);
            Assert.Equal("hello", pi.NodeName);
            Assert.Equal(0, pi.ChildNodes.Count);
            Assert.Equal((DomNodeType) 7, pi.NodeType);
            Assert.Null(pi.InnerText);
        }

        [Fact]
        public void CreateProcessingInstruction_node_not_in_document_has_owner() {
            DomDocument doc = new DomDocument();
            var pi = doc.CreateProcessingInstruction("hello", "world");
            Assert.Null(pi.ParentNode);
            Assert.Null(pi.ParentElement);
            Assert.Equal(doc, pi.OwnerDocument);
        }

        [Fact]
        public void NodeName_equals_target() {
            DomDocument doc = new DomDocument();
            var pi = doc.CreateProcessingInstruction("hello", "world");
            Assert.Equal("hello", pi.NodeName);
        }

        [Fact]
        public void NodeValue_equals_entire_content_excluding_target() {
            DomDocument doc = new DomDocument();
            var pi = doc.CreateProcessingInstruction("hello", "world");
            Assert.Equal("world", pi.NodeValue);
        }
    }
}
