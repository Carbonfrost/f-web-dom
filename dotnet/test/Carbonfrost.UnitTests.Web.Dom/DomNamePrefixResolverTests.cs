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

    public class DomNamePrefixResolverTests {

        [Fact]
        public void GetPrefix_will_resolve_nominal() {
            var doc = new DomDocument();
            doc.AppendElement("root").Attribute("xmlns:a", "https://example.com/a");

            var resolver = DomNamePrefixResolver.ForElement(doc.DocumentElement);
            Assert.Equal("a", resolver.GetPrefix("https://example.com/a", DomScope.TargetAndAncestors));
        }

        [Fact]
        public void GetNamespace_will_resolve_nominal() {
            var doc = new DomDocument();
            doc.AppendElement("root").Attribute("xmlns:a", "https://example.com/a");

            var resolver = DomNamePrefixResolver.ForElement(doc.DocumentElement);
            Assert.Equal("https://example.com/a", resolver.GetNamespace("a", DomScope.TargetAndAncestors));
        }
    }
}
