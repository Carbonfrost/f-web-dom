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
    }
}
