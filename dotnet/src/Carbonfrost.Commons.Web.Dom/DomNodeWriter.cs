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

        private DomNode _current;
        private DomAttribute _attribute;
        private readonly DomWriterStateMachine _state;

        public DomNodeWriter(DomNode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            _current = node;
            _state = new DomWriterStateMachine();

            if (node.NodeType == DomNodeType.Element) {
                _state.Transition(DomWriteState.Element);
            }
        }

        public override DomWriteState WriteState {
            get {
                return _state.WriteState;
            }
        }

        public override void WriteStartElement(DomName name) {
            _state.StartElement();
            _current = _current.AppendElement(name);
            _attribute = null;
        }

        public override void WriteStartAttribute(DomName name) {
            _state.StartAttribute();
            _attribute = _current.AppendAttribute(name);
        }

        public override void WriteEndAttribute() {
            _state.EndAttribute();
            _attribute = null;
        }

        public override void WriteValue(string value) {
            _state.Value();
            if (_attribute != null) {
                _attribute.AppendValue(value);
            } else {
                _current.AppendText(value);
            }
        }

        public override void WriteEndDocument() {
            _state.EndDocument();
            _current = null;
            _attribute = null;
        }

        public override void WriteDocumentType(string name, string publicId, string systemId) {
            _state.DocumentType();
            var doc = _current.AppendDocumentType(name);
            doc.PublicId = publicId;
            doc.SystemId = systemId;
        }

        public override void WriteEntityReference(string name) {
            _state.EntityReference();
            throw new NotImplementedException();
        }

        public override void WriteProcessingInstruction(string target, string data) {
            _state.ProcessingInstruction();
            _current.AppendProcessingInstruction(target, data);
        }

        public override void WriteNotation() {
            _state.Notation();
            throw new NotImplementedException();
        }

        public override void WriteComment(string data) {
            _state.Comment();
            _current.AppendComment(data);
        }

        public override void WriteCDataSection(string data) {
            _state.CDataSection();
            _current.AppendCDataSection(data);
        }

        public override void WriteText(string data) {
            _state.Text();
            _current.AppendText(data);
        }

        public override void WriteStartDocument() {
            _state.StartDocument();
        }

        public override void WriteEndElement() {
            _state.EndElement();
            _current = _current.ParentNode;
        }

    }
}
