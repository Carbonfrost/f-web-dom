//
// Copyright 2012, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

using System;
using System.Collections.Generic;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Web.Dom.Query;

namespace Carbonfrost.Commons.Web.Dom {

    public class CssSelector : DomSelector {

        private readonly Evaluator _evaluator;

        private CssSelector(Evaluator evaluator) {
            _evaluator = evaluator;
        }

        public static new CssSelector Parse(string text) {
            CssSelector result;
            var ex = _TryParse(text, out result);
            if (ex != null) {
                throw ex;
            }

            return result;
        }

        public static bool TryParse(string text, out CssSelector result) {
            return _TryParse(text, out result) == null;
        }

        private static Exception _TryParse(string text, out CssSelector result) {
            result = null;
            if (text == null) {
                return new ArgumentNullException(nameof(text));
            }

            text = text.Trim();
            if (text.Length == 0) {
                return Failure.AllWhitespace(nameof(text));
            }

            try {
                var opts = new QueryParserOptions();
                var evaluator = QueryParser.Parse(text, opts);
                result = new CssSelector(evaluator);
                return null;
            } catch (Exception ex) {
                return Failure.NotParsable(nameof(text), typeof(CssSelector), ex);
            }
        }

        public override DomObjectQuery Select(DomNode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }
            var e = node as DomContainer;
            if (e == null) {
                return DomObjectQuery._Empty;
            }
            return Collect(_evaluator, e);
        }

        public override bool Matches(DomNode node) {
            var root = node.OwnerDocument.DocumentElement;
            var e = node as DomElement;
            if (e == null) {
                return false;
            }
            return _evaluator.Matches(root, e);
        }

        public override string ToString() {
            return _evaluator.ToString();
        }

        private static DomObjectQuery Collect(Evaluator eval, DomContainer root) {
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
