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

using System.Linq;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomPathTests {

        private DomDocument _document;

        private DomElement BElement {
            get {
                return Document.DocumentElement.FirstChild.FirstChild;
            }
        }

        private DomDocument Document {
            get {
                return _document ?? (_document = new DomDocument().LoadXml(@"
                <root>
                    <a>
                        <b one='1' two='2'>
                            <c>
                                <d />
                            </c>
                            <c />
                            <c attr='present' />
                            <c attr='matching value' />
                        </b>
                    </a>
                    <f id='hello' />
                </root>
".Replace("'", "\"")));
            }
        }

        [Fact]
        public void Id_should_get_element_by_Id() {
            var path = DomPath.Root.Id("hello");
            var expected = Document.GetElementById("hello");
            Assert.Equal("//*[@id=\"hello\"]", path.ToString());
            Assert.Same(expected, path.Select(Document.DocumentElement).Elements[0]);
        }

        [Fact]
        public void Element_should_get_elements_by_name() {
            var path = DomPath.Root.Element("root");
            Assert.Same(Document.DocumentElement, path.Select(Document).Elements[0]);
            Assert.Equal("/root", path.ToString());
        }

        [Fact]
        public void Element_should_get_element_by_element_and_index() {
            var path = DomPath.Root.Element("c", 2);
            Assert.Equal("present", path.Select(BElement).Elements[0].Attribute("attr"));
            Assert.Equal("/c[2]", path.ToString());
        }

        [Fact]
        public void DescendantHasAttribute_should_get_elements_with_attribute_present() {
            var path = DomPath.Root.DescendantHasAttribute("attr");
            Assert.HasCount(2, path.Select(BElement).Elements);
            Assert.Equal("c", path.Select(BElement).Elements[0].LocalName);
            Assert.Equal("//*[@attr]", path.ToString());
        }

        [Fact]
        public void DescendantHasAttributeValue_should_get_elements_with_attribute_and_value_present() {
            var path = DomPath.Root.DescendantHasAttributeValue("attr", "matching value");
            Assert.HasCount(1, path.Select(BElement).Elements);
            Assert.Equal("c", path.Select(BElement).Elements[0].LocalName);
            Assert.Equal("matching value", path.Select(BElement).Elements[0].Attribute("attr"));
        }

        [Fact]
        public void Root_should_get_document() {
            var path = DomPath.Root;
            Assert.HasCount(1, Document.Select(path));
            Assert.Same(Document, path.Select(Document)[0]);
        }

        [Fact]
        public void Attribute_should_get_matching_attribute() {
            var path = DomPath.Root.Attribute("one");
            Assert.HasCount(1, BElement.Select(path));
            Assert.Same(BElement.Attributes["one"], path.Select(BElement)[0]);
        }

        [Fact]
        public void Parse_should_create_expressions_and_match() {
            var path = DomPath.Parse("/root/a/b/c");
            Assert.Equal("/root/a/b/c", path.ToString());
            Assert.Same(BElement.FirstChild, path.Select(Document)[0]);
        }

        [Fact]
        public void Parse_should_create_attribute_expressions() {
            var path = DomPath.Parse("/root/a/b@one");
            Assert.Equal("/root/a/b@one", path.ToString());
            Assert.Same(BElement.Attributes["one"], path.Select(Document)[0]);
        }

        [Fact]
        public void Parse_should_match_index_expression() {
            var path = DomPath.Parse("/root/a/b/c[2]");
            Assert.Equal("/root/a/b/c[2]", path.ToString());
            var actual = path.Select(Document)[0] as DomElement;

            Assert.Equal("c", actual.LocalName);
            Assert.Equal("present", actual.Attribute("attr"));
        }

        [Fact]
        public void Parse_should_create_index_expressions() {
            var path = DomPath.Parse("/html/body/div[4]/main");
            Assert.Equal("/html/body/div[4]/main", path.ToString());
            Assert.Equal(new [] {
                DomPathExpressionType.Root,
                DomPathExpressionType.Element,
                DomPathExpressionType.Element,
                DomPathExpressionType.Element,
                DomPathExpressionType.Element,
            }, path.Expressions.Select(e => e.Type));
            Assert.Equal(new [] {
                "html",
                "body",
                "div",
                "main",
            }, path.Expressions.Skip(1).Select(e => e.LocalName));
            Assert.Equal(new [] {
                -1,
                -1,
                4,
                -1,
            }, path.Expressions.Skip(1).Select(e => e.Index));
        }

        [Fact]
        public void Parse_should_create_ID_expression_nominal() {
            var path = DomPath.Parse("//*[@id=\"hello\"]");
            Assert.Equal("//*[@id=\"hello\"]", path.ToString());
            Assert.Equal("f", path.Select(Document)[0].LocalName);
        }
    }
}
