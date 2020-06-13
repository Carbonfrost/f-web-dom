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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomWriter : DisposableObject, IDomNodeVisitor {

        public DomWriterSettings WriterSettings {
            get {
                return DomWriterSettings;
            }
        }

        protected DomWriterSettings DomWriterSettings {
            get;
            private set;
        }

        protected DomWriter(DomWriterSettings writerSettings) {
            DomWriterSettings = writerSettings ?? DomWriterSettings.Empty;
        }

        protected DomWriter() : this(null) {
        }

        public static DomWriter Create(TextWriter writer, DomWriterSettings settings) {
            if (writer == null) {
                throw new ArgumentNullException(nameof(writer));
            }
            if (settings == null) {
                return Create(XmlWriter.Create(writer));
            }

            var pro = DomProviderFactory.ForProviderObject(settings) ?? DomProviderFactory.Default;
            return pro.CreateWriter(writer, settings);
        }

        public static DomWriter Create(XmlWriter writer) {
            if (writer == null) {
                throw new ArgumentNullException(nameof(writer));
            }

            return new XmlDomWriter(writer);
        }

        public static DomWriter Create(StreamContext output) {
            return Create(output, null);
        }

        public static DomWriter Create(string outputUri) {
            return Create(outputUri, null);
        }

        public static DomWriter Create(StreamContext output, DomWriterSettings settings) {
            if (output == null) {
                throw new ArgumentNullException(nameof(output));
            }
            var provider = DomProviderFactory.ForFileName(settings, Utility.LocalPath(output.Uri));
            return provider.CreateWriter(output.AppendText(), settings);
        }

        public static DomWriter Create(string fileName, DomWriterSettings settings) {
            if (fileName == null) {
                throw new ArgumentNullException(nameof(fileName));
            }
            return Create(StreamContext.FromFile(fileName), settings);
        }

        public void Close() {
            Dispose(true);
        }

        internal static string GetOuterString(DomWriterSettings settings, DomNode node) {
            var sw = new StringWriter();
            var writer = new DefaultDomWriter(sw, settings);
            writer.Write(node);
            return sw.ToString();
        }

        internal static string GetInnerString(DomWriterSettings settings, DomNode node) {
            var sw = new StringWriter();
            var writer = new DefaultDomWriter(sw, settings);
            writer.Write(node.ChildNodes);
            return sw.ToString();
        }

        public void Write(IEnumerable<DomObject> objs) {
            if (objs == null) {
                throw new ArgumentNullException(nameof(objs));
            }

            Visit(objs);
        }

        public void Write(DomObject obj) {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            DomNodeVisitor.Visit(obj, this);
        }

        public virtual void WriteStartElement(DomName name) {}
        public virtual void WriteStartAttribute(DomName name) {}
        public virtual void WriteEndAttribute() {}

        public virtual void WriteValue(string value) {}

        public virtual void WriteEndDocument() {}

        public virtual void WriteDocumentType(string name, string publicId, string systemId) {}

        public virtual void WriteEntityReference(string name) {}
        public virtual void WriteProcessingInstruction(string target, string data) {}

        public virtual void WriteNotation() {}

        public virtual void WriteComment(string data) {}
        public virtual void WriteCDataSection(string data) {}
        public virtual void WriteText(string data) {}

        public virtual void WriteStartDocument() {}

        public virtual void WriteEndElement() {}

        protected virtual void WriteElement(DomElement element) {
            if (element == null) {
                throw new ArgumentNullException(nameof(element));
            }

            WriteStartElement(element.Name);
            Visit(element.Attributes);
            Visit(element.ChildNodes);
            WriteEndElement();
        }

        protected virtual void WriteAttribute(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException(nameof(attribute));
            }

            WriteStartAttribute(attribute.Name);
            WriteValue(attribute.Value);
            WriteEndAttribute();
        }

        protected virtual void WriteDocument(DomDocument document) {
            if (document == null) {
                throw new ArgumentNullException(nameof(document));
            }

            // Don't generate an empty document
            if (document.ChildNodes.Count == 0) {
                return;
            }
            WriteStartDocument();
            Visit(document.ChildNodes);
            WriteEndDocument();
        }

        protected virtual void WriteCDataSection(DomCDataSection section) {
            if (section == null) {
                throw new ArgumentNullException(nameof(section));
            }

            WriteCDataSection(section.Data);
        }

        protected virtual void WriteComment(DomComment comment) {
            if (comment == null) {
                throw new ArgumentNullException(nameof(comment));
            }

            WriteComment(comment.Data);
        }

        protected virtual void WriteText(DomText text) {
            if (text == null) {
                throw new ArgumentNullException(nameof(text));
            }

            WriteText(text.Data);
        }

        protected virtual void WriteProcessingInstruction(DomProcessingInstruction instruction) {
            if (instruction == null) {
                throw new ArgumentNullException(nameof(instruction));
            }

            WriteProcessingInstruction(instruction.Target, instruction.Data);
        }

        protected virtual void WriteNotation(DomNotation notation) {
            if (notation == null) {
                throw new ArgumentNullException(nameof(notation));
            }

            // TODO Overloads for writing notations
        }

        protected virtual void WriteDocumentType(DomDocumentType documentType) {
            if (documentType == null) {
                throw new ArgumentNullException(nameof(documentType));
            }

            WriteDocumentType(documentType.Name, documentType.PublicId, documentType.SystemId);
        }

        protected virtual void WriteEntityReference(DomEntityReference entityReference) {
            if (entityReference == null) {
                throw new ArgumentNullException(nameof(entityReference));
            }

            WriteEntityReference(entityReference.Data);
        }

        protected virtual void WriteEntity(DomEntity entity) {
            if (entity == null) {
                throw new ArgumentNullException(nameof(entity));
            }

            // TODO Overloads for writing entities
        }

        protected virtual void WriteDocumentFragment(DomDocumentFragment fragment) {
            if (fragment == null) {
                throw new ArgumentNullException(nameof(fragment));
            }

            Visit(fragment.ChildNodes);
        }

        void IDomNodeVisitor.Visit(DomElement element) {
            WriteElement(element);
        }

        void IDomNodeVisitor.Visit(DomAttribute attribute) {
            WriteAttribute(attribute);
        }

        void IDomNodeVisitor.Visit(DomDocument document) {
            WriteDocument(document);
        }

        void IDomNodeVisitor.Visit(DomCDataSection section) {
            WriteCDataSection(section);
        }

        void IDomNodeVisitor.Visit(DomComment comment) {
            WriteComment(comment);
        }

        void IDomNodeVisitor.Visit(DomText text) {
            WriteText(text);
        }

        void IDomNodeVisitor.Visit(DomProcessingInstruction instruction) {
            WriteProcessingInstruction(instruction);
        }

        void IDomNodeVisitor.Visit(DomNotation notation) {
            WriteNotation(notation);
        }

        void IDomNodeVisitor.Visit(DomDocumentType documentType) {
            WriteDocumentType(documentType);
        }

        void IDomNodeVisitor.Visit(DomEntityReference reference) {
            WriteEntityReference(reference);
        }

        void IDomNodeVisitor.Visit(DomEntity entity) {
            WriteEntity(entity);
        }

        void IDomNodeVisitor.Visit(DomDocumentFragment fragment) {
            WriteDocumentFragment(fragment);
        }

        private void Visit(IEnumerable<DomObject> nodes) {
            DomNodeVisitor.VisitAll(nodes, this);
        }
    }

}
