//
// Copyright 2019 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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


using System.Collections.Generic;

namespace Carbonfrost.Commons.Web.Dom.Query {

    static class Collector {

        public static DomObjectQuery Collect(Evaluator eval, DomContainer root) {
            var elements = new List<DomNode>();

            DomNodeVisitor.Visit(root, node => {
                DomElement el = node as DomElement;
                if (el != null) {
                    if (eval.Matches(root, el)) {
                        elements.Add(el);
                    }
                }
            });
            return new DomObjectQuery(elements);
        }
    }
}

