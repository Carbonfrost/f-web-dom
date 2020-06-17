//
// Copyright 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomPathGeneratorTests {

        [Fact]
        public void Default_provides_element_position_for_DomElement_GetDomPath() {
            DomDocument doc = new DomDocument();
            var dd = doc.AppendElement("a").AppendElement("b");
            Assert.Equal("/a/b[0]", dd.GetDomPath(DomPathGenerator.Default).ToString());
        }

        [Fact]
        public void Minimal_selects_ID_for_DomElement_GetDomPath() {
            DomDocument doc = new DomDocument();
            var dd = doc.AppendElement("a").AppendElement("b").AppendElement("c");
            dd.Attribute("id", "c");
            Assert.Equal("//*[@id=\"c\"]", dd.GetDomPath(DomPathGenerator.Minimal).ToString());
        }

        [Fact]
        public void Minimal_ignores_element_position_when_not_ambiguous_for_DomElement_GetDomPath() {
            DomDocument doc = new DomDocument();
            var dd = doc.AppendElement("a").AppendElement("b");
            Assert.Equal("/a/b", dd.GetDomPath(DomPathGenerator.Minimal).ToString());
        }

        [Fact]
        public void Minimal_ignores_element_position_when_not_ambiguous_by_name_for_DomElement_GetDomPath() {
            var a = new DomDocument().AppendElement("a");
            var b = a.AppendElement("b");
            var dd = a.AppendElement("d");
            Assert.Equal("/a/d", dd.GetDomPath(DomPathGenerator.Minimal).ToString());
        }

        [Fact]
        public void Minimal_includes_element_position_when_it_is_ambiguous_for_DomElement_GetDomPath() {
            var a = new DomDocument().AppendElement("a");
            var dd = a.AppendElement("b");
            a.AppendElement("b");

            Assert.Equal("/a/b[0]", dd.GetDomPath(DomPathGenerator.Minimal).ToString());
        }

        [Fact]
        public void Default_get_DomAttribute_GetDomPath() {
            DomDocument doc = new DomDocument();
            var dd = doc.AppendElement("a")
                .Attribute("attr", "value")
                .Attributes["attr"];

            Assert.Equal(
                "/a@attr",
                dd.GetDomPath(DomPathGenerator.Minimal).ToString()
            );
        }
    }
}
