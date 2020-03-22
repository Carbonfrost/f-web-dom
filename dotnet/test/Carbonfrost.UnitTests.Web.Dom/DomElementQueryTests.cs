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

    public partial class DomElementQueryTests {

        [Fact]
        public void Unwrap_returns_combined_list_of_child_nodes() {
            var doc = new DomDocument();
            doc.OuterXml = @"<root>
                                <p><a /><b /><c /></p>
                                <p></p>
                                <p><d /></p>
                                <p><e /></p>
                             </root>";
            var result = doc.QuerySelectorAll("p").Unwrap();
            var names = doc.QuerySelector("root").Children.NodeNames();
            Assert.Equal(new [] { "a", "b", "c", "d", "e" }, names);
        }

    }
}
