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
using System.Text;

namespace Carbonfrost.Commons.Web.Dom {

    class OuterTextWriter : DomWriter {

        private readonly StringBuilder _buffer;

        public override DomWriteState WriteState {
            get {
                throw new NotSupportedException();
            }
        }

        public OuterTextWriter(StringBuilder buffer) {
            _buffer = buffer;
        }

        public static string ConvertToString(IEnumerable<DomObject> objs) {
            var builder = new StringBuilder();
            DomNodeVisitor.VisitAll(objs, new OuterTextWriter(builder));
            return builder.ToString();
        }

        public static string ConvertToString(DomObject obj) {
            var builder = new StringBuilder();
            DomNodeVisitor.Visit(obj, new OuterTextWriter(builder));
            return builder.ToString();
        }

        public override void WriteCDataSection(string data) {
            _buffer.Append(data);
        }

        public override void WriteEntityReference(string name) {
            // TODO Requires DomEscaper to support named conversions; introduction of DomEntitySet
        }

        public override void WriteText(string data) {
            _buffer.Append(data);
        }

        public override void WriteStartElement(DomName name) {
        }

        public override void WriteStartAttribute(DomName name) {
        }

        public override void WriteEndAttribute() {
        }

        public override void WriteValue(string value) {
        }

        public override void WriteEndDocument() {
        }

        public override void WriteDocumentType(string name, string publicId, string systemId) {
        }

        public override void WriteProcessingInstruction(string target, string data) {
        }

        public override void WriteNotation() {
        }

        public override void WriteComment(string data) {
        }

        public override void WriteStartDocument() {
        }

        public override void WriteEndElement() {
        }
    }
}
