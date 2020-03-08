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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Carbonfrost.Commons.Html.Parser;

namespace Carbonfrost.Commons.Web.Dom.Query {

    class QueryParserOptions {

        public class CssExpression {
            public string Token { get; set; }
            public bool HasArguments { get; set; }
            public Func<string, Evaluator> CreateEvaluator { get; set; }
            public string MatchToken {
                get {
                    if (HasArguments) {
                        return Token + "(";
                    }
                    return Token;
                }
            }
        }

        private readonly List<CssExpression> _expressions;

        public List<CssExpression> Expressions {
            get {
                return _expressions;
            }
        }

        public QueryParserOptions() {
            _expressions = new List<CssExpression>() {
                CssFun(":lt", IndexLessThan),
                CssFun(":gt", IndexGreaterThan),
                CssFun(":eq", IndexEquals),
                CssFun(":contains", tq => Contains(false, tq)),
                CssFun(":containsOwn", tq => Contains(true, tq)),
                CssFun(":matches", tq => Matches(false, tq)),
                CssFun(":matchesOwn", tq => Matches(true, tq)),
            };
        }

        private CssExpression CssFun(string v, Func<string, Evaluator> creator) {
            return new CssExpression {
                Token = v,
                HasArguments = true,
                CreateEvaluator = creator,
            };
        }

        private Evaluator IndexLessThan(string tq) {
            return new Evaluator.IndexLessThan(ParseIndex(tq));
        }

        private Evaluator IndexGreaterThan(string tq) {
            return new Evaluator.IndexGreaterThan(ParseIndex(tq));
        }

        private Evaluator IndexEquals(string tq) {
            return new Evaluator.IndexEquals(ParseIndex(tq));
        }

        private int ParseIndex(string tq) {
            return int.Parse(tq.Trim());
        }

        private Evaluator Contains(bool own, string tq) {
            string searchText = TokenQueue.Unescape(tq);

            if (searchText.Length == 0) {
                throw DomFailure.ContainsSelectorCannotBeEmpty();
            }

            if (own) {
                return (new Evaluator.ContainsOwnText(searchText));
            }
            return (new Evaluator.ContainsText(searchText));
        }

        private Evaluator Matches(bool own, string tq) {
            string regex = tq;

            if (string.IsNullOrEmpty(regex)) {
                throw DomFailure.MatchesSelectorCannotBeEmpty();
            }

            if (own) {
                return (new Evaluator.MatchesOwn(new Regex(regex)));
            }
            return (new Evaluator.MatchesImpl(new Regex(regex)));
        }
    }
}
