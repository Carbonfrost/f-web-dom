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

namespace Carbonfrost.Commons.Web.Dom {

    class CompositeDomWriter : DomWriter {

        private readonly DomWriter[] _writers;

        public CompositeDomWriter(IEnumerable<DomWriter> writers) {
            _writers = writers.ToArray();
        }

        public override DomWriteState WriteState {
            get {
                return _writers[0].WriteState;
            }
        }

        private void Each(Action<DomWriter> action) {
            foreach (var w in _writers) {
                action(w);
            }
        }

        public override void WriteStartElement(DomName name) {
            Each(w => w.WriteStartElement(name));
        }

        public override void WriteStartAttribute(DomName name) {
            Each(w => w.WriteStartAttribute(name));
        }

        public override void WriteEndAttribute() {
            Each(w => w.WriteEndAttribute());
        }

        public override void WriteValue(string value) {
            Each(w => w.WriteValue(value));
        }

        public override void WriteEndDocument() {
            Each(w => w.WriteEndDocument());
        }

        public override void WriteDocumentType(string name, string publicId, string systemId) {
            Each(w => w.WriteDocumentType(name, publicId, systemId));
        }

        public override void WriteEntityReference(string name) {
            Each(w => w.WriteEntityReference(name));
        }

        public override void WriteProcessingInstruction(string target, string data) {
            Each(w => w.WriteProcessingInstruction(target, data));
        }

        public override void WriteNotation() {
            Each(w => w.WriteNotation());
        }

        public override void WriteComment(string data) {
            Each(w => w.WriteComment(data));
        }

        public override void WriteCDataSection(string data) {
            Each(w => w.WriteCDataSection(data));
        }

        public override void WriteText(string data) {
            Each(w => w.WriteText(data));
        }

        public override void WriteStartDocument() {
            Each(w => w.WriteStartDocument());
        }

        public override void WriteEndElement() {
            Each(w => w.WriteEndElement());
        }
    }

}
