//
// Copyright 2013, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Xml;

namespace Carbonfrost.Commons.Web.Dom {

    class XmlDomWriter : DomWriter {

        readonly XmlWriter writer;

        public XmlDomWriter(XmlWriter writer) {
            this.writer = writer;
        }

        public override void WriteDocumentType(string name, string publicId, string systemId) {
            writer.WriteDocType(name, publicId, systemId, null);
        }

        public override void WriteStartAttribute(DomName name) {
            writer.WriteStartAttribute(name.LocalName, name.NamespaceUri);
        }

        public override void WriteEndAttribute() {
            writer.WriteEndAttribute();
        }

        public override void WriteStartElement(DomName name) {
            writer.WriteStartElement(name.LocalName, name.NamespaceUri);
        }

        public override void WriteProcessingInstruction(string target, string data) {
            writer.WriteProcessingInstruction(target, data);
        }

        public override void WriteValue(string value) {
            writer.WriteValue(value);
        }

        public override void WriteEndDocument() {
            writer.WriteEndDocument();
        }

        public override void WriteStartDocument() {
            writer.WriteStartDocument();
        }

        public override void WriteEntityReference(string name) {
            writer.WriteEntityRef(name);
        }

        public override void WriteEndElement() {
            writer.WriteEndElement();
        }

        public override void WriteNotation() {
            throw new NotImplementedException();
        }

        public override void WriteCDataSection(string data) {
            writer.WriteCData(data);
        }

        public override void WriteComment(string data) {
            writer.WriteComment(data);
        }

        public override void WriteText(string data) {
            if (string.IsNullOrWhiteSpace(data)) {
                writer.WriteWhitespace(data);
            } else {
                writer.WriteValue(data);
            }
        }
    }
}
