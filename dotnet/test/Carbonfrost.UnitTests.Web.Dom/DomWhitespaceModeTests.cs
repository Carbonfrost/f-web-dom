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

using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomWhitespaceModeTests {

        public IEnumerable<string> ExpectedOperators {
            get {
                return new [] {
                    "op_Equality",
                    "op_Inequality",
                };
            }
        }

        public IEnumerable<string> Names {
            get {
                return new [] {
                    "None",
                    "Preserve",
                    "Default"
                };
            }
        }

        public DomWhitespaceMode[] Values {
            get {
                return new [] {
                    DomWhitespaceMode.None,
                    DomWhitespaceMode.Preserve,
                    DomWhitespaceMode.Default
                };
            }
        }

        [Theory]
        [PropertyData(nameof(ExpectedOperators))]
        public void Equals_operators_should_be_present(string name) {
            Assert.NotNull(
                typeof(DomWhitespaceMode).GetMethod(name)
            );
        }

        [Fact]
        public void Values_each_are_unique() {
            Assert.HasCount(
                Values.Length,
                DomWhitespaceMode.GetValues().Select(t => t.ToInt32()).Distinct()
            );
        }

        [Fact]
        public void GetNames_should_have_correct_values() {
            Assert.SetEqual(
                Names,
                DomWhitespaceMode.GetNames()
            );
        }

        [Fact]
        public void GetValues_should_have_correct_values() {
            Assert.SetEqual(
                Values,
                DomWhitespaceMode.GetValues()
            );
        }

        [Theory]
        [PropertyData(nameof(Names))]
        public void GetName_should_produce_the_right_names(string name) {
            var value = (DomWhitespaceMode) typeof(DomWhitespaceMode).GetField(name).GetValue(null);
            Assert.Equal(
                name,
                DomWhitespaceMode.GetName(value)
            );
        }

        [Theory]
        [PropertyData(nameof(Names))]
        public void Parse_should_roundtrip_on_values(string name) {
            var expected = typeof(DomWhitespaceMode).GetField(name).GetValue(null);
            Assert.Equal(
                expected,
                DomWhitespaceMode.Parse(name)
            );
        }

        [Fact]
        public void TryParse_should_return_false_on_unknown_value() {
            Assert.False(
                DomWhitespaceMode.TryParse("Unknown", out _)
            );
        }

        [Fact]
        public void Default_is_the_default_value() {
            Assert.Equal(
                DomWhitespaceMode.Default,
                default(DomWhitespaceMode)
            );
        }
    }

}
