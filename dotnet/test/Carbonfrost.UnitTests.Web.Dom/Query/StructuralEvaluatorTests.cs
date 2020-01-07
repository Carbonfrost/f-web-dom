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
using Carbonfrost.Commons.Web.Dom;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.Web.Dom.Query {

    public class StructuralEvaluatorTests {

        [Fact]
        public void ImmediateParent_should_consider_depth() {
            DomDocument d = new DomDocument();
            d.LoadXml(@"<html>
                           <a id='1'>
                                <a id='2'>
                                    <a id='3'> </a>
                                </a>
                           </a>
                        </html>");
            var match = d.QuerySelectorAll("html > a");
            Assert.HasCount(1, match);
            Assert.Equal("1", match.First().Attribute("id"));
        }
    }
}


