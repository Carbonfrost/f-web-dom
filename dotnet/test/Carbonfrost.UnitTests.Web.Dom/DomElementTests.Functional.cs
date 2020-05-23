//
// Copyright 2013, 2016, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Text.RegularExpressions;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public partial class DomElementTests {

        [Theory]
        [FixtureData("DomElement-{operation}.fixture")]
        public void Applying_operation_should_equal_expected_output(DomElementFixture f) {
            var doc = new DomDocument();
            var input = doc.CreateDocumentFragment();
            var expectedOutput = doc.CreateDocumentFragment();
            input.LoadXml(f.Input);
            expectedOutput.LoadXml(f.Output);

            var op = FindOperation(f);
            op((DomElement) input.Select("target").First());

            // Compress whitespace to simplify comparison
            input.CollapseWS();
            expectedOutput.CollapseWS();

            Assert.Equal(expectedOutput.InnerXml.Trim(), input.InnerXml.Trim());
        }

        private Action<DomElement> FindOperation(DomElementFixture f) {
            switch (f.Operation.ToLowerInvariant()) {
                case "unwrap":
                    return e => e.Unwrap();
                case "removeattributes":
                    return e => e.RemoveAttributes();
                case "prependtext":
                    return e => e.PrependText("#text");
                case "appendtext":
                    return e => e.AppendText("#text");
                default:
                    Assert.Pending("Operation not defined: " + f.Operation);
                    return null;
            }
        }
    }

    public class DomElementFixture {
        public string Case { get; set; }
        public string Operation { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }

        public override string ToString() {
            return Case;
        }
    }

}
