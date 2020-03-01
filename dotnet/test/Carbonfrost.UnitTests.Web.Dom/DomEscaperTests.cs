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

    public class DomEscaperTests {

        [Theory]
        [InlineData("&", "&amp;")]
        [InlineData("\"", "&quot;")]
        [InlineData("<", "&lt;")]
        [InlineData(">", "&gt;")]
        [InlineData("'", "&apos;")]
        public void Escape_replaces_known_characters(string c, string expected) {
            Assert.Equal(
                expected,
                DomEscaper.Default.Escape(c)
            );
        }

        [Theory]
        [InlineData("&amp;", "&")]
        [InlineData("&quot;", "\"")]
        [InlineData("&lt;", "<")]
        [InlineData("&gt;", ">")]
        [InlineData("&apos;", "'")]
        public void Unescape_replaces_known_sequences(string seq, string expected) {
            Assert.Equal(
                expected,
                DomEscaper.Default.Unescape(seq)
            );
        }

        [Theory]
        [InlineData("&#32;", " ")]
        [InlineData("&#x20;", " ")]
        public void Unescape_replaces_hex_and_numeric_sequences(string seq, string expected) {
            Assert.Equal(
                expected,
                DomEscaper.Default.Unescape(seq)
            );
        }
    }
}

