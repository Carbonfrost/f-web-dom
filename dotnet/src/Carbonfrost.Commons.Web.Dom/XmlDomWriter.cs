//
// Copyright 2013, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Xml;

namespace Carbonfrost.Commons.Web.Dom {

    class XmlDomWriter : DomWriter {

        private readonly XmlWriter _writer;

        public XmlDomWriter(XmlWriter writer) {
            _writer = writer;
        }

        public override DomWriteState WriteState {
            get {
                return (DomWriteState) _writer.WriteState;
            }
        }

        public override void WriteDocumentType(string name, string publicId, string systemId) {
            _writer.WriteDocType(name, publicId, systemId, null);
        }

        public override void WriteStartAttribute(DomName name) {
            _writer.WriteStartAttribute(name.LocalName, name.NamespaceUri);
        }

        public override void WriteEndAttribute() {
            _writer.WriteEndAttribute();
        }

        public override void WriteStartElement(DomName name) {
            _writer.WriteStartElement(name.LocalName, name.NamespaceUri);
        }

        public override void WriteProcessingInstruction(string target, string data) {
            _writer.WriteProcessingInstruction(target, data);
        }

        public override void WriteValue(string value) {
            _writer.WriteValue(value);
        }

        public override void WriteEndDocument() {
            _writer.WriteEndDocument();
        }

        public override void WriteStartDocument() {
            _writer.WriteStartDocument();
        }

        public override void WriteEntityReference(string name) {
            _writer.WriteEntityRef(name);
        }

        public override void WriteEndElement() {
            _writer.WriteEndElement();
        }

        public override void WriteNotation() {
            throw new NotImplementedException();
        }

        public override void WriteCDataSection(string data) {
            _writer.WriteCData(data);
        }

        public override void WriteComment(string data) {
            _writer.WriteComment(data);
        }

        public override void WriteText(string data) {
            if (string.IsNullOrWhiteSpace(data)) {
                _writer.WriteWhitespace(data);
            } else {
                _writer.WriteValue(data);
            }
        }
    }
}
