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
using System.IO;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public partial class DomWriterTests {

        [Fact]
        public void WriteComment_should_generate_comment_text() {
            AssertResult(
                "<!--comment-->",
                w => w.WriteComment("comment")
            );
        }

        [Fact]
        public void WriteCDataSection_should_generate_text() {
            AssertResult(
                "<![CDATA[possible]]>",
                w => w.WriteCDataSection("possible")
            );
        }

        [Fact]
        public void WriteStartElement_and_WriteEndElement_implicitly_creates_document_element() {
            AssertResult(
                "<a/>",
                w => w.WriteStartElement("a"),
                w => w.WriteEndElement()
            );
        }

        private void AssertResult(string expected, params Action<DomWriter>[] actions) {
            var sw = new StringWriter();
            DomWriter w = DomWriter.Create(sw, DomWriterSettings.Empty);
            foreach (var a in actions) {
                a(w);
            }
            Assert.Equal(expected, sw.ToString());
        }
    }
}
