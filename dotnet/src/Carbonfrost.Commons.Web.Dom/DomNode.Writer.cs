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
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomNode : IDomStream<DomNode>, IXmlSerializable {

        // TODO Support cloning

        public void CopyTo(DomNode parent) {
            if (parent == null) {
                throw new ArgumentNullException(nameof(parent));
            }

            parent.Append(this.Clone());
        }

        public void CopyContentsTo(DomNode parent) {
            if (parent == null) {
                throw new ArgumentNullException(nameof(parent));
            }

            foreach (var child in ChildNodes) {
                parent.Append(child.Clone());
            }
        }

        public DomNodeReader CreateReader() {
            return CreateReader(null);
        }

        public virtual DomNodeReader CreateReader(DomNodeReaderSettings settings) {
            return new DomNodeReader(this, settings);
        }

        public string ToXmlString(XmlWriterSettings settings) {
            StringWriter sw = new StringWriter();
            using (XmlWriter xw = XmlWriter.Create(sw, settings)) {
                WriteTo(xw);
            }
            return sw.ToString();
        }

        public string ToXmlString() {
            return ToXmlString(new XmlWriterSettings {
                OmitXmlDeclaration = true
            });
        }

        public virtual DomWriter Prepend() {
            if (PreviousSiblingNode == null) {
                return RequireParent().Append();
            }
            return PreviousSiblingNode.AppendAfter();
        }

        public virtual DomWriter Append() {
            return OwnerDocument.ProviderFactory.CreateWriter(this, null);
        }

        public virtual DomNodeWriter AppendAfter() {
            return OwnerDocument.ProviderFactory.CreateWriter(this, null);
        }

        public void WriteTo(TextWriter writer) {
            if (writer == null){
                throw new ArgumentNullException(nameof(writer));
            }

            WriteTo(
                CreateDefaultTextWriter(writer)
            );
        }

        public void WriteTo(XmlWriter writer) {
            if (writer == null){
                throw new ArgumentNullException(nameof(writer));
            }

            WriteTo(new XmlDomWriter(writer));
        }

        public void WriteTo(DomWriter writer) {
            if (writer == null){
                throw new ArgumentNullException(nameof(writer));
            }

            writer.Write(this);
        }

        public void WriteContentsTo(TextWriter writer) {
            if (writer == null) {
                throw new ArgumentNullException(nameof(writer));
            }

            WriteContentsTo(
                CreateDefaultTextWriter(writer)
            );
        }

        public void WriteContentsTo(XmlWriter writer) {
            if (writer == null){
                throw new ArgumentNullException(nameof(writer));
            }

            WriteContentsTo(new XmlDomWriter(writer));
        }

        public void WriteContentsTo(DomWriter writer) {
            if (writer == null){
                throw new ArgumentNullException(nameof(writer));
            }

            writer.Write(ChildNodes);
        }

        XmlSchema IXmlSerializable.GetSchema() {
            // TODO XmlSchema adapter for dom semantics (rare)
            return null;
        }

        // TODO Consider whether each node can read (only document really makes sense)
        void IXmlSerializable.ReadXml(XmlReader reader) {
        }

        void IXmlSerializable.WriteXml(XmlWriter writer) {
            WriteTo(writer);
        }

        private DomWriter CreateDefaultTextWriter(TextWriter writer) {
            return FindProviderFactory().CreateWriter(writer);
        }
    }

}
