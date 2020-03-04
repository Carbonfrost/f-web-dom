//
// - DomWriter.cs -
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
using System.Collections.Generic;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomWriter : DisposableObject, IDomNodeVisitor {

        public void Close() {
            Dispose(true);
        }

        public void Write(IEnumerable<DomObject> nodes) {
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            Visit(nodes);
        }

        public void Write(DomObject node) {
            if (node == null)
                throw new ArgumentNullException("node");

            node.AcceptVisitor(this);
        }

        public abstract void WriteStartElement(string name, string namespaceUri);
        public abstract void WriteStartAttribute(string name, string namespaceUri);
        public abstract void WriteEndAttribute();

        public abstract void WriteValue(string value);

        public abstract void WriteEndDocument();

        public abstract void WriteDocumentType(string name, string publicId, string systemId);

        public abstract void WriteEntityReference(string name);
        public abstract void WriteProcessingInstruction(string target, string data);

        public abstract void WriteNotation();

        public abstract void WriteComment(string data);
        public abstract void WriteCDataSection(string data);
        public abstract void WriteText(string data);

        public abstract void WriteStartDocument();

        public abstract void WriteEndElement();

        public virtual void WriteElement(DomElement element) {
            if (element == null)
                throw new ArgumentNullException("element");

            WriteStartElement(element.Name, element.NamespaceUri);
            Visit(element.Attributes);
            Visit(element.ChildNodes);
            WriteEndElement();
        }

        public virtual void WriteAttribute(DomAttribute attribute) {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            WriteStartAttribute(attribute.Name, attribute.NamespaceUri);
            WriteValue(attribute.Value);
            WriteEndAttribute();
        }

        public virtual void WriteDocument(DomDocument document) {
            if (document == null)
                throw new ArgumentNullException("document");

            // Don't generate an empty document
            if (document.ChildNodes.Count == 0) {
                return;
            }
            WriteStartDocument();
            Visit(document.ChildNodes);
            WriteEndDocument();
        }

        public virtual void WriteCDataSection(DomCDataSection section) {
            if (section == null)
                throw new ArgumentNullException("section");

            WriteCDataSection(section.Data);
        }

        public virtual void WriteComment(DomComment comment) {
            if (comment == null)
                throw new ArgumentNullException("comment");

            WriteComment(comment.Data);
        }

        public virtual void WriteText(DomText text) {
            if (text == null)
                throw new ArgumentNullException("text");

            WriteText(text.Data);
        }

        public virtual void WriteProcessingInstruction(DomProcessingInstruction instruction) {
            if (instruction == null)
                throw new ArgumentNullException("instruction");

            WriteProcessingInstruction(instruction.Target, instruction.Data);
        }

        public virtual void WriteNotation(DomNotation notation) {
            if (notation == null)
                throw new ArgumentNullException("notation");

            // TODO Overloads for writing notations
        }

        public virtual void WriteDocumentType(DomDocumentType documentType) {
            if (documentType == null)
                throw new ArgumentNullException("documentType");

            WriteDocumentType(documentType.Name, documentType.PublicId, documentType.SystemId);
        }

        public virtual void WriteEntityReference(DomEntityReference entityReference) {
            if (entityReference == null)
                throw new ArgumentNullException("entityReference");

            WriteEntityReference(entityReference.Data);
        }


        public virtual void WriteEntity(DomEntity entity) {
            if (entity == null)
                throw new ArgumentNullException("entity");

            // TODO Overloads for writing entities
        }

        public virtual void WriteDocumentFragment(DomDocumentFragment fragment) {
            if (fragment == null)
                throw new ArgumentNullException("fragment");

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
            foreach (var node in nodes) {
                node.AcceptVisitor(this);
            }
        }
    }

}
