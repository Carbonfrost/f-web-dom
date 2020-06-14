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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public partial class DomObjectQueryTests {

        [Fact]
        public void Add_should_dedupe_existing_nodes() {
            var doc = new DomDocument();
            var root = doc.AppendElement("root");
            var node = root.AppendAttribute("n", 20);

            var q = new DomObjectQuery().Add(node).Add(node);
            Assert.HasCount(1, q);
            Assert.Same(node, q[0]);
        }

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
        public void ReplaceWith_with_multiple_nodes_does_clone() {
            var xml = "<html> <s /><s /><s /> </html>";
            var doc = new DomDocument();
            doc.LoadXml(xml);
            var replacement = doc.CreateElement("t");
            doc.Select("s").ReplaceWith(replacement);

            Assert.Equal("<html> <t /><t /><t /> </html>", doc.ToXmlString());
        }

        [Fact]
        public void ReplaceWith_with_multiple_nodes_keeps_the_replacement() {
            var xml = "<html> <s /><s /><s /> </html>";
            var doc = new DomDocument();
            doc.LoadXml(xml);
            var replacement = doc.CreateElement("t");
            var result = doc.Select("s").ReplaceWith(replacement);

            // We use the actual instance for the first replacement, then clone the rest
            Assert.Same(doc.DocumentElement.FirstChild, replacement);
            Assert.HasCount(3, result);
        }

        [Fact]
        public void ReplaceWith_returns_the_node_set() {
            var xml = "<html><s /><s /></html>";
            var doc = new DomDocument();
            doc.LoadXml(xml);
            var result = doc.Select("s").ReplaceWith("<a /><b /><c />");

            Assert.HasCount(6, result);
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
