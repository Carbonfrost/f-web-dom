//
// Copyright 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

    public class CompositeDomWriterTests {

        [Fact]
        public void Append_generates_composite_write_output() {
            var html = new DomDocument().LoadXml(@"
                <a>
                    <b a='100' />
                    <c>
                        <b />
                    </c>
                </a>".Replace('\'', '"'));

            var w = html.QuerySelectorAll("b").Append();
            w.WriteStartElement("new");
            w.WriteStartAttribute("attr");
            w.WriteValue("value");
            w.WriteEndAttribute();
            w.WriteEndElement();
            w.WriteValue("significant text");
            Assert.Equal(@"
                <a>
                    <b a=""100""><new attr=""value""/>significant text</b>
                    <c>
                        <b><new attr=""value""/>significant text</b>
                    </c>
                </a>".Replace('\'', '"'),
                html.OuterXml
            );
        }

        [Fact]
        public void Append_can_add_attributes_to_elements() {
            var html = new DomDocument().LoadXml(@"
                <a>
                    <b a='100' />
                    <b />
                </a>".Replace('\'', '"'));

            var w = html.QuerySelectorAll("b").Append();
            w.WriteStartAttribute("a");
            w.WriteValue("value");
            w.WriteEndAttribute();
            Assert.Equal(@"
                <a>
                    <b a='100value'/>
                    <b a='value'/>
                </a>".Replace('\'', '"'),
                html.OuterXml
            );
        }

    }
}
