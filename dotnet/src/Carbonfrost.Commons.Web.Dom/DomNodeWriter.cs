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

namespace Carbonfrost.Commons.Web.Dom {

    public class DomNodeWriter : DomWriter {

        // TODO Node reader/writer

        private DomNode node;

        public DomNodeWriter(DomNode node) {
            this.node = node;
        }

        public override void WriteStartElement(DomName name) {
            throw new NotImplementedException();
        }

        public override void WriteStartAttribute(DomName name) {
            throw new NotImplementedException();
        }

        public override void WriteEndAttribute() {
            throw new NotImplementedException();
        }

        public override void WriteValue(string value) {
            throw new NotImplementedException();
        }

        public override void WriteEndDocument() {
            throw new NotImplementedException();
        }

        public override void WriteDocumentType(string name, string publicId, string systemId) {
            throw new NotImplementedException();
        }

        public override void WriteEntityReference(string name) {
            throw new NotImplementedException();
        }

        public override void WriteProcessingInstruction(string target, string data) {
            throw new NotImplementedException();
        }

        public override void WriteNotation() {
            throw new NotImplementedException();
        }

        public override void WriteComment(string data) {
            throw new NotImplementedException();
        }

        public override void WriteCDataSection(string data) {
            throw new NotImplementedException();
        }

        public override void WriteText(string data) {
            throw new NotImplementedException();
        }

        public override void WriteStartDocument() {
            throw new NotImplementedException();
        }

        public override void WriteEndElement() {
            throw new NotImplementedException();
        }

    }
}
