//
// - OuterTextVisitor.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using System.Text;

namespace Carbonfrost.Commons.Web.Dom {

    class OuterTextVisitor : DomNodeVisitor {

        private readonly StringBuilder sb = new StringBuilder();
        private readonly bool _markup;

        public OuterTextVisitor(bool markup) {
            _markup = markup;
        }

        public string ConvertToString(DomNode node) {
            node.AcceptVisitor(this);
            return sb.ToString();
        }

        public string ConvertToString(IEnumerable<DomNode> nodes) {
            foreach (var node in nodes) {
                node.AcceptVisitor(this);
            }
            return sb.ToString();
        }

        private void Visit(IEnumerable<DomObject> nodes, string between) {
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            bool comma = false;
            foreach (var node in nodes) {
                if (comma)
                    sb.Append(between);

                Visit(node);
                comma = true;
            }
        }

        protected override void VisitElement(DomElement element) {
            if (element == null) {
                throw new ArgumentNullException("element");
            }

            // TODO Respect Tag's rules for how to encapsulate SGML notation
            // element.Tag
            if (_markup) {
                sb.Append("<");
                sb.Append(element.Name);

                if (element.Attributes.Any()) {
                    sb.Append(" ");
                }

                Visit(element.Attributes, " ");
                sb.Append(">");
            }

            Visit(element.ChildNodes);

            if (_markup) {
                sb.Append("</");
                sb.Append(element.Name);
                sb.Append(">");
            }
        }

        protected override void VisitAttribute(DomAttribute attribute) {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            sb.Append(attribute.Name);
            sb.Append("=");
            sb.Append("\"");
            sb.Append(EscapeText(attribute.Value));
            sb.Append("\"");
        }

        protected override void VisitDocument(DomDocument document) {
            if (document == null)
                throw new ArgumentNullException("document");
            Visit(document.ChildNodes);
        }

        protected override void VisitCDataSection(DomCDataSection section) {
            if (section == null)
                throw new ArgumentNullException("section");

            sb.Append("<![CDATA[");
            sb.Append(section.Data);
            sb.Append("]]>");
        }

        protected override void VisitComment(DomComment comment) {
            if (comment == null)
                throw new ArgumentNullException("comment");

            sb.Append("<!--");
            sb.Append(comment.Data);
            sb.Append("-->");
        }

        protected override void VisitText(DomText text) {
            if (text == null)
                throw new ArgumentNullException("text");

            sb.Append(EscapeText(text.Data));
        }

        protected override void VisitProcessingInstruction(DomProcessingInstruction instruction) {
            if (instruction == null)
                throw new ArgumentNullException("instruction");

            sb.Append("<?");
            sb.Append(instruction.Target);
            sb.Append(" ");
            sb.Append(instruction.Data);
            sb.Append("-->");
        }

        protected override void VisitNotation(DomNotation notation) {
            if (notation == null)
                throw new ArgumentNullException("notation");

            DefaultVisit(notation);
        }

        protected override void VisitDocumentType(DomDocumentType documentType) {
            if (documentType == null)
                throw new ArgumentNullException("documentType");

            DefaultVisit(documentType);
        }

        private static string EscapeText(string str) {
            if (str == null) {
                return null;
            }
            var s = new StringBuilder(str);
            return s.Replace("&", "&amp;")
                    .Replace("\"", "&quot;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("'", "&apos;")
                    .ToString();
        }
    }
}
