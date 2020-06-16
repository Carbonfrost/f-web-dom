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

    public partial class DomNameContextTests : TestClass {

        [Theory]
        [FixtureData("DomNameContext-{operation}.fixture")]
        public void Name_should_be_resolved_based_on_context(DomNameContextFixture f) {
            var doc = new DomDocument().LoadXml(f.Markup);
            if (doc.DocumentElement.Attribute("context") == "xml") {
                doc.NameContext = DomNameContext.Xml;
            }

            var ele = doc.DocumentElement.FirstChild;
            ele = ele.SetName("mem:hello");

            Assert.Equal(f.Prefix, ele.Prefix);
            Assert.Equal(f.Namespace, ele.NamespaceUri);
            Assert.Equal(f.LocalName, ele.LocalName);
        }

        public class DomNameContextFixture {
            public string Markup { get; set; }
            public string LocalName { get; set; }
            public string Prefix { get; set; }
            public string Namespace { get; set; }
        }
    }
}
