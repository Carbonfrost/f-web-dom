//
// - DomNode.Writer.cs -
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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomNode : IDomStream<DomNode>, IXmlSerializable {

        // TODO Support cloning

        public void CopyTo(DomNode parent) {
            if (parent == null)
                throw new ArgumentNullException("parent");

            parent.Append(this.Clone());
        }

        public void CopyContentsTo(DomNode parent) {
            if (parent == null)
                throw new ArgumentNullException("parent");

            foreach (var child in this.ChildNodes)
                parent.Append(child.Clone());
        }

        public DomNodeReader CreateReader() {
            return CreateReader(null);
        }

        public virtual DomNodeReader CreateReader(DomNodeReaderSettings settings) {
            return new DomNodeReader(this, settings);
        }

        public string ToXml(XmlWriterSettings settings) {
            StringWriter sw = new StringWriter();
            using (XmlWriter xw = XmlWriter.Create(sw, settings))
                WriteTo(xw);
            return sw.ToString();
        }

        public string ToXml() {
            XmlWriterSettings s = new XmlWriterSettings();
            s.OmitXmlDeclaration = true;
            return ToXml(s);
        }

        public virtual DomWriter Prepend() {
            if (PreviousSiblingNode == null)
                return RequireParent().Append();
            else
                return PreviousSiblingNode.AppendAfter();
        }

        public virtual DomWriter Append() {
            return OwnerDocument.ProviderFactory.CreateWriter(this, null);
        }

        public virtual DomNodeWriter AppendAfter() {
            return OwnerDocument.ProviderFactory.CreateWriter(this, null);
        }

        public virtual void WriteTo(TextWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            WriteTo(XmlWriter.Create(writer));
        }

        public void WriteTo(XmlWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            WriteTo(new XmlDomWriter(writer));
        }

        public void WriteTo(DomWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(this);
        }

        public void WriteContentsTo(TextWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            WriteContentsTo(XmlWriter.Create(writer));
        }

        public void WriteContentsTo(XmlWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            WriteContentsTo(new XmlDomWriter(writer));
        }

        public void WriteContentsTo(DomWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(this.ChildNodes);
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
    }

}
