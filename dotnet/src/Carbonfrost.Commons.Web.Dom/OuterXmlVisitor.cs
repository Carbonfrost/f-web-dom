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
using System.Linq;
using System.Text;

namespace Carbonfrost.Commons.Web.Dom {

    class OuterXmlVisitor : DomNodeVisitor, ITextVisitor {

        private StringBuilder _sb;
        private readonly DomWriterSettings _settings;
        private readonly Dictionary<int, string> _indentStringCache = new Dictionary<int, string>();
        private int _depth;

        private readonly string _spaceAfterAttributes = " ";
        private int _paddingAlignedAttributes;
        private readonly string _spaceAroundAttributeEqual;

        private string IndentString {
            get {
                if (!_settings.Indent) {
                    return null;
                }
                return _indentStringCache.GetValueOrCache(
                    _depth,
                    _ => new string(_settings.IndentCharacter.ToChar(), _settings.IndentWidth * _depth)
                );
            }
        }

        public void SetBuffer(StringBuilder stringBuilder) {
            _sb = stringBuilder;
        }

        public OuterXmlVisitor(DomWriterSettings settings) {
            _settings = settings ?? DomWriterSettings.Empty;
            _spaceAfterAttributes = " ";
            // FIXME Should be a different property controlling this variable
            _spaceAroundAttributeEqual = _settings.Indent ? " " : "";
            if (_settings.Indent) {
                // No space after attributes because we break lines
                _spaceAfterAttributes = "";
            }
        }

        protected override void VisitElement(DomElement element) {
            if (element == null) {
                throw new ArgumentNullException(nameof(element));
            }
            if (element.ChildNodes.Count == 0) {
                StartTag(element, true);
                AfterEndTag();
                return;
            }

            StartTag(element, false);
            VisitAll(element.ChildNodes);

            _depth++;
            EndTag(element);
            _depth--;
        }

        private void EndTag(DomElement element) {
            _sb.Append(IndentString);
            _sb.Append("</");
            _sb.Append(element.Name);
            _sb.Append(">");
            AfterEndTag();
        }

        private void AfterEndTag() {
            if (_settings.Indent) {
                _sb.AppendLine();
            }
        }

        private void StartTag(DomElement element, bool close) {
            _sb.Append(IndentString);
            _sb.Append("<");
            _sb.Append(element.Name);

            if (element.Attributes.Any()) {
                _sb.Append(" ");

                if (_settings.AlignAttributes) {
                    _paddingAlignedAttributes = element.Attributes.Max(a => a.Name.Length);
                }
            }

            Visit(element.Attributes, _spaceAfterAttributes);
            if (_settings.AlignAttributes && close) {
                _sb.Append(" ");
            }
            _sb.Append(close ? "/>" : ">");
        }

        protected override void VisitAttribute(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException(nameof(attribute));
            }

            string name = attribute.Name;
            if (_settings.AlignAttributes) {
                name = attribute.Name.PadLeft(_paddingAlignedAttributes);
            }
            // FIXME This has to all migrate to a WRiter
            if (attribute.AttributePosition > 0) {
                name = IndentString + name.PadLeft(attribute.OwnerElement.Name.Length + 1);
            }

            _sb.Append(name);
            _sb.Append(_spaceAroundAttributeEqual);
            _sb.Append("=");
            _sb.Append(_spaceAroundAttributeEqual);

            _sb.Append("\"");
            _sb.Append(EscapeText(attribute.Value));
            _sb.Append("\"");
            if (_settings.AlignAttributes) {
                _sb.AppendLine();
            }
        }

        protected override void VisitDocument(DomDocument document) {
            if (document == null) {
                throw new ArgumentNullException(nameof(document));
            }
            VisitAll(document.ChildNodes);
        }

        protected override void VisitCDataSection(DomCDataSection section) {
            if (section == null) {
                throw new ArgumentNullException(nameof(section));
            }

            _sb.Append("<![CDATA[");
            _sb.Append(section.Data);
            _sb.Append("]]>");
        }

        protected override void VisitComment(DomComment comment) {
            if (comment == null) {
                throw new ArgumentNullException(nameof(comment));
            }

            _sb.Append("<!--");
            _sb.Append(comment.Data);
            _sb.Append("-->");
        }

        protected override void VisitText(DomText text) {
            if (text == null) {
                throw new ArgumentNullException(nameof(text));
            }

            _sb.Append(EscapeText(text.Data));
        }

        protected override void VisitProcessingInstruction(DomProcessingInstruction instruction) {
            if (instruction == null) {
                throw new ArgumentNullException(nameof(instruction));
            }

            _sb.Append("<?");
            _sb.Append(instruction.Target);
            _sb.Append(" ");
            _sb.Append(instruction.Data);
            _sb.Append("?>");
        }

        protected override void VisitNotation(DomNotation notation) {
            if (notation == null) {
                throw new ArgumentNullException(nameof(notation));
            }

            DefaultVisit(notation);
        }

        protected override void VisitDocumentType(DomDocumentType node) {
            _sb.Append("<!DOCTYPE ").Append(node.Name);

            if (!string.IsNullOrWhiteSpace(node.PublicId)) {
                _sb.Append(" PUBLIC \"")
                    .Append(node.PublicId)
                    .Append("\"");
            }

            if (!string.IsNullOrWhiteSpace(node.SystemId)) {
                _sb.Append(" \"")
                    .Append(node.SystemId)
                    .Append("\"");
            }

            _sb.Append('>');
        }

        private static string EscapeText(string str) {
            return DomEscaper.Default.Escape(str);
        }

        private void Visit(IEnumerable<DomObject> nodes, string between) {
            if (nodes == null) {
                throw new ArgumentNullException(nameof(nodes));
            }

            bool sep = false;
            foreach (var node in nodes) {
                if (sep) {
                    _sb.Append(between);
                }

                Visit(node);
                sep = true;
            }
        }
    }
}
