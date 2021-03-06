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
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomDocumentFragmentTests {

        [Fact]
        public void Clone_will_clone_annotations_copied_to_new_node() {
            var doc = new DomDocument();

            var node = doc.CreateDocumentFragment().AddAnnotation(new object());
            var clone = node.Clone();

            Assert.HasCount(1, clone.AnnotationList.OfType<object>());
        }

        [Fact]
        public void CreateAttribute_uses_primitive_value_specified_by_schema() {
            var schema = new DomSchema("custom");
            var attrDef = schema.AttributeDefinitions.AddNew("lcid");
            attrDef.ValueType = typeof(int);

            var doc = new DomDocumentFragment().WithSchema(schema);
            Assert.IsInstanceOf<DomValue<int>>(
                doc.CreateAttribute("lcid").DomValue
            );
        }


        [Fact]
        public void NodeName_equals_special_name() {
            var doc = new DomDocumentFragment();
            Assert.Equal("#document-fragment", doc.NodeName);
        }

        [Fact]
        public void NodeValue_is_null() {
            var doc = new DomDocumentFragment();
            Assert.Null(doc.NodeValue);
        }

        [Fact]
        public void ParentElement_should_be_null() {
            var doc = new DomDocumentFragment();
            Assert.Null(doc.ParentElement);
        }

        [Fact]
        public void ParentNode_should_be_null() {
            var doc = new DomDocumentFragment();
            Assert.Null(doc.ParentElement);
        }

        [Fact]
        public void Append_should_append_fragment_contents_not_fragment_itself() {
            var frag = new DomDocument().CreateDocumentFragment();
            frag.LoadXml(@"<a /> <b /> <c />");

            var doc = new DomDocument();
            doc.AppendElement("html").Append(frag);
            Assert.Equal("<html><a /> <b /> <c /></html>", doc.ToXmlString());
            Assert.Equal("", frag.ToXmlString());
        }

        [Fact]
        public void Wrap_with_fragment_should_remove_children_from_document() {
            var doc = new DomDocument();
            doc.LoadXml(@"<html> <a /> <b /> <c /> </html>");
            var frag = doc.CreateDocumentFragment();
            doc.QuerySelectorAll("html > *").Wrap(frag);

            Assert.Equal("<html>    </html>", doc.ToXmlString());
            Assert.Equal("<a /><b /><c />", frag.ToXmlString());
        }

        [Fact]
        public void Constructor_can_create_document_fragment_with_implicit_internal_document() {
            var frag = new DomDocumentFragment();
            Assert.NotNull(frag.OwnerDocument);
        }

        [Fact]
        public void Constructor_can_create_document_fragment_with_implicit_internal_document_using_specialized_type() {
            var frag = new RDocumentFragment();
            Assert.NotNull(frag.OwnerDocument);
            Assert.IsInstanceOf<RDocument>(frag.OwnerDocument);
        }

    }
}
