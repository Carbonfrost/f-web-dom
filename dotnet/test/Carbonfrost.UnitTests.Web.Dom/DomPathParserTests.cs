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

using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomPathParserTests {

        [Fact]
        public void Tokens_splits_on_quotes_internally() {
            var s = "/root//*[@id=\"hello\"]";
            var tokens = DomPathParser.Tokens(s);
            Assert.Equal(
                new [] { "/", "root", "//", "*", "[", "@", "id", "=", "hello", "]", "<eof>" },
                tokens.Select(t => t.Value)
            );
        }

        [Fact]
        public void Token_splits_on_spaces_correctly() {
            var s = "//*[@id=\"hello space\"]";
            var tokens = DomPathParser.Tokens(s);
            Assert.Equal(
                new [] { "//", "*", "[", "@", "id", "=", "hello space", "]", "<eof>" },
                tokens.Select(t => t.Value)
            );
        }
    }
}
