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

    public class DomNameComparerTests {

        [Theory]
        [InlineData("a", "A", 0)]
        [InlineData("B", "a", 1)]
        [InlineData("b", "a", 1)]
        [InlineData("a", "b", -1)]
        public void CompareTo_should_apply_ignore_case(string x, string y, int expected) {
            Assert.Equal(
                expected, DomNameComparer.IgnoreCase.Compare(x, y)
            );
        }

        [Theory]
        [InlineData("a", "A", true)]
        [InlineData("B", "a", false)]
        [InlineData("a", "a", true)]
        public void Equals_should_apply_ignore_case(string x, string y, bool expected) {
            Assert.Equal(
                expected, DomNameComparer.IgnoreCase.Equals(x, y)
            );
        }

    }
}
