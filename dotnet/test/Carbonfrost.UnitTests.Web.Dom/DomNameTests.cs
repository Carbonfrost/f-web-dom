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

using System;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomNameTests {

        const string MUSHROOM_KINGDOM = "http://ns.example.com/mushroom-kingdom";

        [Theory]
        [InlineData("{http://ns.example.com/mushroom-kingdom} Mario", "F")]
        [InlineData("Mario", "S")]
        [InlineData("http://ns.example.com/mushroom-kingdom", "N")]
        [InlineData("Mario", "p")]
        [InlineData("{http://ns.example.com/mushroom-kingdom}", "m")]
        public void ToString_should_format(string expected, string format) {
            DomName qn = DomName.Parse("{http://ns.example.com/mushroom-kingdom} Mario");
            Assert.Equal(expected, qn.ToString(format));
        }

        [Fact]
        public void ToString_should_format_default() {
            DomName qn = DomName.Parse("{http://ns.example.com/mushroom-kingdom} Mario");
            Assert.Equal("{http://ns.example.com/mushroom-kingdom} Mario", qn.ToString());
        }

        [Fact]
        public void ToString_should_format_prefix_binding() {
            DomName qn = DomName.Parse("{http://ns.example.com/mushroom-kingdom} Mario").WithPrefix("mk");
            Assert.Equal("mk:Mario", qn.ToString("P"));
        }

        [Fact]
        public void TryParse_should_detect_non_registered_prefixes() {
            DomName qn;
            Assert.False(DomName.TryParse("nonexistant:a", out qn));
        }

        [Fact]
        public void Parse_should_parse_default_ns() {
            DomName qn = DomName.Parse("Mario");
            Assert.Equal("Mario", qn.LocalName);
        }

        [Fact]
        public void Parse_should_parse_expanded_names() {
            DomName qn = DomName.Parse("{http://ns.example.com/mushroom-kingdom} Mario");
            Assert.Equal("Mario", qn.LocalName);
            Assert.Equal("http://ns.example.com/mushroom-kingdom", qn.NamespaceUri);
        }

        [Fact]
        public void Parse_should_throw_oninvalid_names() {
            Assert.Throws<ArgumentException>(() => { DomName.Parse("*&Ma^^rio"); });
            Assert.Throws<ArgumentException>(() => { DomName.Parse("name whitespace"); });
            Assert.Throws<ArgumentException>(() => { DomName.Parse(""); });
        }

        [Fact]
        public void Create_local_name_is_required() {
            Assert.Throws<ArgumentException>(() => { DomName.Create(DomNamespace.Default, ""); });
            Assert.Throws<ArgumentNullException>(() => { DomName.Create(DomNamespace.Default, null); });
        }

        [Fact]
        public void Equals_operator_should_apply() {
            DomName n = DomName.Create(DomNamespace.Default, "default");
            DomName m = n;
            Assert.False(n == null);
            Assert.True(n != null);
            Assert.False(null == n);
            Assert.True(null != n);

            Assert.True(m == n);
            Assert.False(m != n);
        }

        [Fact]
        public void Equals_qualified_names_equals_equatable() {
            DomName n = DomName.Create(DomNamespace.Default, "default");
            Assert.False(n.Equals(null));
            Assert.True(n.Equals(n));
        }

        [Fact]
        public void WithNamespace_should_change_nominal() {
            DomName n = DomName.Create(DomNamespace.Default, "default");
            DomNamespace nu = DomNamespace.Create("https://example.com");
            n = n.WithNamespace(nu);
            Assert.Same(nu, n.Namespace);
        }

        [Fact]
        public void WithLocalName_should_change_nominal() {
            DomName n = DomName.Create(DomNamespace.Default, "default");
            n = n.WithLocalName("name");
            Assert.Same("name", n.LocalName);
        }

        [Fact]
        public void Create_should_generate_xml_semantics() {
            var dom = DomName.Create("xmlns:a");
            Assert.Equal("xmlns:a", dom.LocalName);
            Assert.Null(dom.Prefix);
            Assert.Empty(dom.NamespaceUri);
            Assert.Equal("xmlns", dom.TryXmlSemantics().Value.Prefix);
            Assert.Equal("a", dom.TryXmlSemantics().Value.LocalName);
        }
    }
}
