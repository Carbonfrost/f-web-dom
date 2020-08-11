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
using System.IO;
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    class DefaultDomWriter : DomWriter {

        private readonly TextWriter _writer;
        private readonly Stack<DomElementState> _elements = new Stack<DomElementState>();
        private readonly Dictionary<int, string> _indentStringCache = new Dictionary<int, string>();

        private bool _needIndent;
        private readonly string _spaceAroundAttributeEqual;
        private readonly string _quoteChar;
        private readonly DomWriterStateMachine _state;

        private int Depth {
            get {
                return _elements.Count - 1;
            }
        }

        public override DomWriteState WriteState {
            get {
                return _state.WriteState;
            }
        }

        private DomElementState Element {
            get {
                if (_elements.Count == 0) {
                    return null;
                }
                return _elements.Peek();
            }
        }

        public DefaultDomWriter(TextWriter writer, DomWriterSettings settings) : base(settings) {
            _writer = writer;

            _spaceAroundAttributeEqual = WriterSettings.Indent ? " " : "";
            _quoteChar = WriterSettings.QuoteAttributesCharacter.ToChar();
            _state = new DomWriterStateMachine();
        }

        public override void WriteStartElement(DomName name) {
            _state.StartElement();
            ImpliesElementWithChildNodes();

            var element = new DomElementState(name.LocalName);
            _elements.Push(element);
        }

        public override void WriteStartAttribute(DomName name) {
            _state.StartAttribute();
            Element.Attributes.Add(new DomAttributeState(name.LocalName, ""));
        }

        private void ImpliesElementWithChildNodes() {
            if (Element == null) {
                return;
            }
            if (Element.HasChildNodes) {
                return;
            }
            Element.HasChildNodes = true;
            StartTag(Element, false);
        }

        public override void WriteEndAttribute() {
            _state.EndAttribute();
        }

        public override void WriteValue(string value) {
            _state.Value();
            int last = Element.Attributes.Count - 1;
            Element.Attributes[last] = new DomAttributeState(
                Element.Attributes[last].Name,
                value
            );
        }

        public override void WriteEndDocument() {
            _state.EndDocument();
        }

        public override void WriteDocumentType(string name, string publicId, string systemId) {
            _state.DocumentType();
            Append("<!DOCTYPE ");
            Append(name);

            if (!string.IsNullOrWhiteSpace(publicId)) {
                Append(" PUBLIC \"");
                Append(publicId);
                Append("\"");
            }

            if (!string.IsNullOrWhiteSpace(systemId)) {
                Append(" \"");
                Append(systemId);
                Append("\"");
            }

            Append('>');
        }

        public override void WriteEntityReference(string name) {
            _state.EntityReference();
        }

        public override void WriteProcessingInstruction(string target, string data) {
            _state.ProcessingInstruction();
            Append("<?");
            Append(target);
            if (!string.IsNullOrEmpty(data)) {
                Append(" ");
                Append(data);
            }
            Append("?>");
        }

        public override void WriteNotation() {
            _state.Notation();
        }

        public override void WriteComment(string data) {
            _state.Comment();
            Append("<!--");
            Append(data);
            Append("-->");
        }

        public override void WriteCDataSection(string data) {
            _state.CDataSection();
            Append("<![CDATA[");
            Append(data);
            Append("]]>");
        }

        public override void WriteText(string data) {
            _state.Text();
            ImpliesElementWithChildNodes();
            Append(EscapeText(data));
            _needIndent = true;
        }

        public override void WriteStartDocument() {
            _state.StartDocument();
        }

        public override void WriteEndElement() {
            _state.EndElement();
            if (!Element.HasChildNodes) {
                StartTag(Element, true);
                _elements.Pop();
                return;
            }
            EndTag(Element);
            _elements.Pop();
        }

        private void Append(string s) {
            _writer.Write(s);
        }

        private void Append(char s) {
            _writer.Write(s);
        }

        private void AppendLine(string s) {
            _writer.WriteLine(s);
        }

        private void AppendLine() {
            _writer.WriteLine();
        }

        private string IndentString {
            get {
                if (!WriterSettings.Indent) {
                    return null;
                }
                return _indentStringCache.GetValueOrCache(
                    Depth,
                    _ => new string(WriterSettings.IndentCharacter.ToChar(), WriterSettings.IndentWidth * Depth)
                );
            }
        }

        private void EndTag(DomElementState element) {
            WriteIndent();
            Append("</");
            Append(element.Name);
            Append(">");
            _needIndent = true;
        }

        private void AfterEndTag() {
            _needIndent = true;
        }

        private void StartTag(DomElementState element, bool close) {
            WriteIndent();
            Append("<");
            Append(element.Name);
            if (element.Attributes.Any()) {
                Append(" ");
            }

            DoWriteAttributes(element);

            // Add a space unless attributes are using alignment
            bool addlSpace = WriterSettings.Indent && WriterSettings.AlignAttributes != DomAttributeAlignment.Unaligned;
            if (addlSpace && close) {
                Append(" ");
            }
            Append(close ? "/>" : ">");
            _needIndent = true;
        }

        private void DoWriteAttributes(DomElementState element) {
            if (element.Attributes.Count == 0) {
                return;
            }

            if (WriterSettings.AlignAttributes == DomAttributeAlignment.Unaligned) {
                DoSimpleWriteAttributes(element);
                return;
            }

            // Align attributes and pad either left or right
            var attributes = element.Attributes;

            int padding = attributes.Max(a => a.Name.Length);
            var pad = PadFunc();

            int index = 0;
            foreach (var attribute in attributes) {
                bool last = index == attributes.Count - 1;
                bool first = index == 0;

                string name = attribute.Name;

                if (first) {
                    name = pad(name, padding);
                } else {
                    name = IndentString + pad(
                        name,
                        padding + element.Name.Length + 2
                    );
                }

                Append(name);
                Append(_spaceAroundAttributeEqual);
                Append("=");
                Append(_spaceAroundAttributeEqual);

                Append(_quoteChar);
                Append(EscapeText(attribute.Value));
                Append(_quoteChar);

                if (!last) {
                    AppendLine();
                }
                index++;
            }
        }

        private void DoSimpleWriteAttributes(DomElementState element) {
            bool space = false;
            foreach (var attribute in element.Attributes) {
                if (space) {
                    Append(" ");
                }
                space = true;

                Append(attribute.Name);
                Append(_spaceAroundAttributeEqual);
                Append("=");
                Append(_spaceAroundAttributeEqual);

                Append(_quoteChar);
                Append(EscapeText(attribute.Value));
                Append(_quoteChar);
            }
        }

        private void WriteIndent() {
            if (_needIndent && WriterSettings.Indent) {
                AppendLine();
                Append(IndentString);
            }
            _needIndent = false;
        }

        private static string EscapeText(string str) {
            return DomEscaper.Default.Escape(str);
        }

        private Func<string, int, string> PadFunc() {
            switch (WriterSettings.AlignAttributes) {
                case DomAttributeAlignment.Left:
                    return (name, total) => name.PadRight(total);
                case DomAttributeAlignment.Right:
                    return (name, total) => name.PadLeft(total);
                default:
                    return (name, total) => name;
            }
        }

        class DomElementState {

            public readonly string Name;
            public readonly IList<DomAttributeState> Attributes;
            public bool HasChildNodes { get; internal set; }

            public DomElementState(string name) {
                Name = name;
                Attributes = new List<DomAttributeState>();
            }
        }

        internal struct DomAttributeState {
            public readonly string Name;
            public readonly string Value;

            public DomAttributeState(string name, string value) {
                Name = name;
                Value = value;
            }
        }
    }
}
