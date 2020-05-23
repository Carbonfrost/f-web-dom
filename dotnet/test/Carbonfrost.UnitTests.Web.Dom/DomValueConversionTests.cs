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
using System.Collections.Generic;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomValueConversionTests {

        [Fact]
        public void Value_for_Uri_DomValue_will_apply_Uri_base_uri() {
            var attr = new DomDocument().CreateAttribute("h");
            attr.BaseUri = new Uri("https://example.com");
            attr.SetValue(new Uri("./hello.txt", UriKind.Relative));

            Assert.Equal(
                new Uri("https://example.com/hello.txt"),
                attr.GetValue<Uri>()
            );
            Assert.Equal("./hello.txt", attr.Value);
        }

        [Fact]
        public void Value_for_List_DomValue_will_append_values() {
            var attr = new DomDocument().CreateAttribute("h");
            attr.SetValue(new List<string>());
            attr.Value = "hello";

            attr.AppendValue("world");
            Assert.Equal(new List<string> { "hello", "world" }, attr.GetValue<List<string>>());
        }

        [Fact]
        public void Text_for_List_DomValue_will_separate_tokens() {
            var attr = new DomDocument().CreateAttribute("h");
            attr.SetValue(new List<string> { "hello", "world" });
            Assert.Equal("hello world", attr.Value);
        }
    }
}
