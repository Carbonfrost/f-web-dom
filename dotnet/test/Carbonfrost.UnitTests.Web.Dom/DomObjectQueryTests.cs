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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                doc.ToXmlString());
        }

        [Fact]
        public void Remove_should_delete_nodes() {
            var xml = "<html> <body><s /><s /><s /></body> </html>";
            var doc = new DomDocument();
            doc.LoadXml(xml);
            doc.Select("s").Remove();

            Assert.Equal("<html> <body /> </html>",
                doc.ToXmlString());
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
                doc.CompressWhitespace().ToXmlString());
        }

        public IEnumerable<PropertyInfo> DomNodeProperties {
            get {
                return typeof(DomNode).GetProperties()
                    .Where(p => p.PropertyType == typeof(DomNode) || p.PropertyType == typeof(IEnumerable<DomNode>));
            }
        }

        public IEnumerable<MethodInfo> DomAppenderMethods {
            get {
                return typeof(IDomNodeAppendApiConventions).GetMethods();
            }
        }

        [Theory]
        [PropertyData(nameof(DomNodeProperties))]
        public void Properties_should_correspond_to_DomNode(PropertyInfo property) {
            // Because there is a property FollowingSiblingNodes on DomNode, the DomObjectQuery must
            // have a method FollowingSiblingNodes() that does the same thing
            var actual = typeof(DomObjectQuery).GetMethod(
                property.Name
            );
            Assert.NotNull(actual);
            Assert.Equal(typeof(DomObjectQuery), actual.ReturnType);
        }

        [Theory]
        [PropertyData(nameof(DomAppenderMethods))]
        public void Append_method_should_exist_matching_nodes(MethodInfo method) {
            // All the appender methods (like AppendText) should exist on DomNode and DomObjectQuery
            MethodInfo actual = typeof(DomObjectQuery).GetMethod(
                method.Name,
                method.GetParameters().Select(t => t.ParameterType).ToArray()
            );
            Assert.NotNull(actual);
            Assert.Equal(typeof(DomObjectQuery), actual.ReturnType);
        }
    }
}
