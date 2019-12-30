//
// Copyright 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomObjectQueryTests {

        [Fact]
        public void Closest_should_return_closest_element() {
            var doc = new DomDocument();
            var self = doc.AppendElement("a").Attribute("class", "clear").AppendElement("a").AppendElement("span");
            Assert.Same(doc.DocumentElement, new DomObjectQuery(self).Closest("a.clear").Single());
        }

        [Fact]
        public void SetName_should_set_names() {
            var xml = "<html> <body a=\"false\"> <s /> <b /> </body> </html>";
            var doc = new DomDocument();
            doc.LoadXml(xml);
            doc.Select("body").SetName("head");

            Assert.Equal("<html> <head a=\"false\"> <s /> <b /> </head> </html>",
                doc.ToXml());
        }

        [Fact]
        public void Remove_should_delete_nodes() {
            var xml = "<html> <body><s /><s /><s /></body> </html>";
            var doc = new DomDocument();
            doc.LoadXml(xml);
            doc.Select("s").Remove();

            Assert.Equal("<html> <body /> </html>",
                doc.ToXml());
        }

        [Fact]
        public void Unwrap_should_unwrap_nodes() {
            const string xml = @"<section>
  <dl>
  <dd class='f'><dl></dl>
  </dd>
  <dd class='e'><dl></dl></dd>
  </dl>
  </section>";
            var doc = new DomDocument();
            doc.LoadXml(xml);
            doc.QuerySelectorAll("dl").QuerySelectorAll("> dd").Unwrap();
            Assert.Equal("<section>\n<dl>\n<dl />\n<dl />\n</dl>\n</section>",
                doc.CompressWhitespace().ToXml());
        }
    }
}
