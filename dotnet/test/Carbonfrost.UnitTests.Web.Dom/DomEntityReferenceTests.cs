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
using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomEntityReferenceTests {

        [Fact]
        public void Clone_will_clone_annotations_copied_to_new_node() {
            var doc = new DomDocument();

            var node = doc.CreateEntityReference("a").AddAnnotation(new object());
            var clone = node.Clone();

            Assert.HasCount(1, clone.AnnotationList.OfType<object>());
        }

        [Fact]
        public void NodeName_equals_name() {
            var doc = new DomDocument();
            var attr = doc.CreateEntityReference("lang");
            Assert.Equal("lang", attr.NodeName);
        }

        [Fact]
        public void NodeValue_is_null() {
            DomDocument doc = new DomDocument();
            var dd = doc.CreateEntityReference("html");
            Assert.Null(dd.NodeValue);
        }
    }
}
