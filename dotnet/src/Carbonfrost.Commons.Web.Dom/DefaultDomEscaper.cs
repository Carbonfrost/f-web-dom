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
using System.Text;
using System.Text.RegularExpressions;

namespace Carbonfrost.Commons.Web.Dom {

    class DefaultDomEscaper : DomEscaper {

        private static readonly Regex UNESCAPE_PATTERN = new Regex(
            @"&(?<code>
                #(?<hex>x)? (?<num>[0-9a-f]+)
                |[a-z]+\d*
            );?", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

        private static readonly Regex UNESCAPE_PATTERN2  = new Regex(
            @"&(\#x?[0-9a-f]+|[a-z]+\d*);", RegexOptions.IgnoreCase);

        private readonly char[] SPECIAL_CHARS = {
            '&',
            '"',
            '<',
            '>',
            '\'',
        };

        private readonly string[] REPLACEMENTS = {
            "amp",
            "quot",
            "lt",
            "gt",
            "apos",
        };

        public override string Escape(string text) {
            if (string.IsNullOrEmpty(text)) {
                return text;
            }
            int start = text.IndexOfAny(SPECIAL_CHARS);
            if (start < 0) {
                return text;
            }
            var s = new StringBuilder(text.Length);
            for (int i = start; i < text.Length; i++) {
                char c = text[i];
                int repl = Array.IndexOf(SPECIAL_CHARS, c);
                if (repl < 0) {
                    s.Append(c);
                } else {
                    s.Append('&').Append(REPLACEMENTS[repl]).Append(';');
                }
            }
            return s.ToString();
        }

        public override string Unescape(string text) {
            if (string.IsNullOrEmpty(text)) {
                return text;
            }
            if (!text.Contains("&")) {
                return text;
            }

            MatchEvaluator evaluator = (m) => {
                var code = m.Groups[1].Value;
                if (code[0] == '#') {
                    return UnescapeHex(code);
                }
                return UnescapeReference(code);
            };
            return UNESCAPE_PATTERN2.Replace(text, evaluator);
        }

        private string UnescapeReference(string code) {
            int index = Array.IndexOf(REPLACEMENTS, code);
            if (index >= 0) {
                return SPECIAL_CHARS[index].ToString();
            }

            return $"&{code};";
        }

        private string UnescapeHex(string code) {
            try {
                bool hex = code[1] == 'x';
                string num = code.Substring(hex ? 2 : 1);
                int base2 = hex ? 16 : 10;
                return ((char) Convert.ToInt32(num, base2)).ToString();

            } catch (OverflowException) {
            } catch (FormatException) {
            }
            return $"&{code};";
        }
    }
}
