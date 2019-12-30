//
// Copyright 2014, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

    public class DomCDataSectionTests {

        [Fact]
        public void HasAttributes_should_be_false() {
            DomContainer doc = new DomDocument().AppendElement("s");
            var text = doc.AppendCDataSection(" ");
            Assert.False(text.HasAttributes);
        }

        [Fact]
        public void HasAttribute_should_be_false() {
            DomContainer doc = new DomDocument().AppendElement("s");
            var text = doc.AppendCDataSection(" ");
            Assert.False(text.HasAttribute("anything"));
        }

        [Fact]
        public void RemoveAttribute_should_be_nop() {
            DomContainer doc = new DomDocument().AppendElement("s");
            var text = doc.AppendCDataSection(" ");

            Assert.Same(text, text.RemoveAttribute("anything"));
        }

        [Fact]
        public void NodeName_equals_attribute_name() {
            var doc = new DomDocument();
            var attr = doc.CreateCDataSection("");
            Assert.Equal("#cdata-section", attr.NodeName);
        }

        [Fact]
        public void NodeValue_equals_text() {
            DomDocument doc = new DomDocument();
            var t = doc.CreateCDataSection("hello");
            Assert.Equal("hello", t.NodeValue);
        }
    }
}
