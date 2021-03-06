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
using System.Text;

namespace Carbonfrost.Commons.Web.Dom {

    static class TextVisitor {

        public static string ConvertToString(this ITextVisitor v, DomNode node) {
            var sb = new StringBuilder();
            v.SetBuffer(sb);
            DomNodeVisitor.Visit(node, v);
            return sb.ToString();
        }

        public static string ConvertToString(this ITextVisitor v, IEnumerable<DomNode> nodes) {
            var sb = new StringBuilder();
            v.SetBuffer(sb);
            DomNodeVisitor.VisitAll(nodes, v);
            return sb.ToString();
        }

    }
}
