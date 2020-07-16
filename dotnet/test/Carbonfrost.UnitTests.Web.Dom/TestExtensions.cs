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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    static class TestExtensions {

        public static IEnumerable<string> NodeNames(this DomNodeCollection source) {
            if (source == null) {
                return Enumerable.Empty<string>();
            }
            return source.Select(n => n == null ? "<null>" : n.NodeName);
        }

        public static IEnumerable<string> NodeNames(this DomElementCollection source) {
            if (source == null) {
                return Enumerable.Empty<string>();
            }
            return source.Select(n => n == null ? "<null>" : n.NodeName);
        }

        internal static void CollapseWS(this DomNode node) {
            DomNodeVisitor.Visit(
                node,
                n1 => {
                    n1.CompressWhitespace();
                    DomCharacterData n = n1 as DomCharacterData;
                    if (n == null) {
                        return;
                    }
                    if (n.Data == null) {
                        return;
                    }
                    n.Data = Regex.Replace(n.Data, @"\s+", " ");
                }
            );
        }
    }
}
