//
// Copyright 2014, 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomValueTests {

        [Fact]
        public void TypedValue_convert_typed_value_to_text() {
            DomValue<long> e = new DomValue<long>();
            Assert.Equal(0, e.TypedValue);
            Assert.Equal("0", e.Value);

            var expr = long.Parse("420");
            e.TypedValue = expr;
            Assert.Equal(expr, e.TypedValue);
            Assert.Equal("420", e.Value);
        }

        [Fact]
        public void TypedValue_convert_text_to_typed() {
            DomValue<long> e = new DomValue<long>();

            e.Value = "421";
            Assert.Equal(421L, e.TypedValue);
            Assert.Equal("421", e.Value);
        }

        [Fact]
        public void AppendValue_will_append_and_convert() {
            DomValue<long> e = new DomValue<long>();

            e.Value = "421";
            Assert.Equal(421L, e.TypedValue);
            Assert.Equal("421", e.Value);
        }

        [Fact]
        public void AppendValue_default_implementation_will_add_objects_when_possible() {
            var e = new StringListDomValue();
            e.Value = "hello";
            e.AppendValue("world");
            Assert.Equal(new List<string> { "hello", "world" }, e.TypedValue);
        }

        class StringListDomValue : DomValue<List<string>> {

            protected override List<string> Convert(string text) {
                return text.Split(' ').ToList();
            }

            protected override string ConvertBack(List<string> value) {
                return string.Join(' ', value);
            }
        }
    }
}
