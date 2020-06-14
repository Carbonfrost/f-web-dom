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

    public class DomNamespaceTests : TestClass {

        [Theory]
        [InlineData("G", "http://www.w3.org/2000/xmlns/")]
        [InlineData("F", "http://www.w3.org/2000/xmlns/")]
        [InlineData("B", "{http://www.w3.org/2000/xmlns/}")]
        public void ToString_should_have_format_strings(string format, string expected) {
            Assert.Equal(expected, DomNamespace.Xmlns.ToString(format));
            Assert.Equal(expected, DomNamespace.Xmlns.ToString(format.ToLowerInvariant()));
        }

        [Theory]
        [InlineData("https://example.com", "http://example.com", Name = "scheme is ignored")]
        [InlineData("https://example.com", "example.com", Name = "implied https")]
        [InlineData("https://example.com/", "https://example.com", Name = "implied trailiing slash")]
        public void Equals_normalizes_in_comparisons(string x, string y) {
            Assert.Equal(DomNamespace.Create(x), DomNamespace.Create(y));

            Assert.True(DomNamespace.Equals(
                DomNamespace.Create(x),
                DomNamespace.Create(y),
                DomNamespaceComparison.Default
            ));

            Assert.False(DomNamespace.Equals(
                DomNamespace.Create(x),
                DomNamespace.Create(y),
                DomNamespaceComparison.Ordinal
            ));
        }
    }
}
