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
using Carbonfrost.Commons.Web.Dom;
using Carbonfrost.Commons.Web.Dom.Query;

namespace Carbonfrost.Commons.Web.Dom.Query {

    static class Collector {

        public static DomObjectQuery Collect(Evaluator eval, DomContainer root) {
            var elements = new List<DomNode>();
            new AccumulatorVisitor(root, elements, eval).Visit(root);
            return new DomObjectQuery(elements);
        }

        private class AccumulatorVisitor : DomNodeVisitor {

            private readonly List<DomNode> _elements;
            private readonly Evaluator _eval;
            private readonly DomContainer _root;

            public AccumulatorVisitor(DomContainer root, List<DomNode> elements, Evaluator eval) {
                _root = root;
                _elements = elements;
                _eval = eval;
            }

            protected override void DefaultVisit(DomObject node) {
                DomElement el = node as DomElement;
                if (el != null) {
                    if (_eval.Matches(_root, el)) {
                        _elements.Add(el);
                    }
                }
                base.DefaultVisit(node);
            }
        }
    }
}

