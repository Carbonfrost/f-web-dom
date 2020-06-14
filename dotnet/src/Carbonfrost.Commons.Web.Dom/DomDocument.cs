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
using System.Linq;
using System.Xml;

using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomDocument : DomContainer, IDomNodeFactory, IDomNodeFactoryApiConventions, IDomXmlLoader<DomDocument> {

        private readonly IDomUnlinkedNodeCollection _unlinked;
        private IDomNodeFactory _nodeFactory;

        public DomElement DocumentElement {
            get {
                return (DomElement) this.ChildNodes.FirstOrDefault(t => t.NodeType == DomNodeType.Element);
            }
        }

        public DomProviderFactory ProviderFactory {
            get {
                return DomProviderFactory;
            }
        }

        protected virtual DomProviderFactory DomProviderFactory {
            get {
                return DomProviderFactory.Default;
            }
        }

        public IDomNodeFactory NodeFactory {
            get {
                if (_nodeFactory == null) {
                    _nodeFactory = ProviderFactory.CreateNodeFactory(Schema);
                }
                return _nodeFactory;
            }
        }

        public DomDocumentType Doctype {
            get {
                return ChildNodes.OfType<DomDocumentType>().FirstOrDefault();
            }
        }

        public override string OuterXml {
            get {
                return base.OuterXml;
            }
            set {
                // TODO Should detect malformed document sooner
                // TODO Should use correct provider to parse
                var frag = CreateDocumentFragment();
                frag.LoadXml(value);
                Append(frag.ChildNodes.ToList());
            }
        }

        internal override DomDocument OwnerDocumentOrSelf {
            get {
                return this;
            }
        }

        protected override DomAttributeCollection DomAttributes {
            get {
                return null;
            }
        }

        public override string NodeName {
            get {
                return "#document";
            }
        }

        public override string InnerText {
            get {
                if (DocumentElement == null) {
                    return null;
                }
                return DocumentElement.InnerText;
            }
            set {
                if (DocumentElement == null) {
                    throw DomFailure.RequiresDocumentElementToSetInnerText();
                }
                DocumentElement.InnerText = value;
            }
        }

        public override DomNodeType NodeType {
            get {
                return DomNodeType.Document;
            }
        }

        internal IDomUnlinkedNodeCollection UnlinkedNodes {
            get {
                return _unlinked;
            }
        }

        public DomDocument() {
            _unlinked = new DomUnlinkedNodeCollection(this);
        }

        internal DomDocument(bool useLL) : base(useLL) {
            _unlinked = new DomUnlinkedNodeCollection(this);
        }

        public virtual int GetLinePosition(DomObject node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }
            return -1;
        }

        public virtual int GetLineNumber(DomObject node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }
            return -1;
        }

        internal override void AssertCanAppend(DomNode node, DomNode willReplace) {
            DomNodeType nodeType = node.NodeType;
            switch (nodeType) {
                case DomNodeType.Attribute:
                case DomNodeType.Document:
                case DomNodeType.DocumentFragment:
                case DomNodeType.EntityReference:
                case DomNodeType.CDataSection:
                    throw DomFailure.CannotAppendChildNodeWithType(nodeType);

                case DomNodeType.Text:
                    if (string.IsNullOrWhiteSpace(node.TextContent)) {
                        return;
                    }
                    throw DomFailure.CannotAppendNonWSText();

                case DomNodeType.Element:
                    var root = DocumentElement;
                    if (root == null || root == node || root == willReplace) {
                        return;
                    }
                    throw DomFailure.CannotHaveMultipleRoots();
                case DomNodeType.Unspecified:
                case DomNodeType.Entity:
                case DomNodeType.ProcessingInstruction:
                case DomNodeType.Comment:
                case DomNodeType.DocumentType:
                case DomNodeType.Notation:
                    return;
            }
        }

        protected override DomNode ReplaceWithCore(DomNode other) {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (other == this)
                return this;

            throw DomFailure.CannotReplaceDocument();
        }

        public DomElement GetElementById(string id) {
            // TODO Implement IdDomValue to index these (performance)
            return (DomElement) Select("#" + id).FirstOrDefault();
        }

        public DomDocument Load(string fileName) {
            return Load(StreamContext.FromFile(fileName));
        }

        public DomDocument Load(Uri source) {
            return Load(StreamContext.FromSource(source));
        }

        public DomDocument Load(Stream input) {
            return Load(StreamContext.FromStream(input));
        }

        public DomDocument Load(StreamContext input) {
            if (input == null) {
                throw new ArgumentNullException(nameof(input));
            }

            LoadText(input.OpenText());
            return this;
        }

        protected virtual void LoadText(TextReader input) {
            if (input == null) {
                throw new ArgumentNullException(nameof(input));
            }

            var reader = ProviderFactory.CreateReader(input);
            reader.CopyTo(this);
        }

        public DomDocument LoadXml(string xml) {
            using (var xr = XmlReader.Create(new StringReader(xml))) {
                Load(xr);
            }
            return this;
        }

        public DomDocument Load(XmlReader reader) {
            CoreLoadXml(reader);
            return this;
        }

        public DomComment CreateComment() {
            return AddUnlinked(CreateCommentCore());
        }

        public DomComment CreateComment(string data) {
            var result = CreateComment();
            result.Data = data;
            return AddUnlinked(result);
        }

        protected virtual DomComment CreateCommentCore() {
            return NodeFactory.CreateComment() ?? new DomComment();
        }

        public DomCDataSection CreateCDataSection() {
            return AddUnlinked(CreateCDataSectionCore());
        }

        public DomCDataSection CreateCDataSection(string data) {
            var result = CreateCDataSection();
            result.Data = data;
            return AddUnlinked(result);
        }

        protected virtual DomCDataSection CreateCDataSectionCore() {
            return NodeFactory.CreateCDataSection() ?? new DomCDataSection();
        }

        public DomText CreateText() {
            return AddUnlinked(CreateTextCore());
        }

        public DomText CreateText(string data) {
            var result = CreateText();
            result.Data = data;
            return AddUnlinked(result);
        }

        protected virtual DomText CreateTextCore() {
            return NodeFactory.CreateText() ?? new DomText();
        }

        public DomProcessingInstruction CreateProcessingInstruction(string target) {
            return AddUnlinked(CreateProcessingInstructionCore(target));
        }

        public DomProcessingInstruction CreateProcessingInstruction(string target, string data) {
            var result = CreateProcessingInstruction(target);
            result.Data = data;
            return AddUnlinked(result);
        }

        protected virtual DomProcessingInstruction CreateProcessingInstructionCore(string target) {
            return NodeFactory.CreateProcessingInstruction(target);
        }

        public virtual Type GetAttributeNodeType(DomName name) {
            return NodeFactory.GetAttributeNodeType(name);
        }

        public Type GetAttributeNodeType(string name) {
            return GetAttributeNodeType(DomName.Create(name));
        }

        public virtual Type GetElementNodeType(DomName name) {
            return NodeFactory.GetElementNodeType(name);
        }

        public Type GetElementNodeType(string name) {
            return GetElementNodeType(DomName.Create(name));
        }

        public virtual Type GetProcessingInstructionNodeType(string name) {
            return NodeFactory.GetProcessingInstructionNodeType(name);
        }

        public virtual DomName GetAttributeName(Type attributeType) {
            return NodeFactory.GetAttributeName(attributeType);
        }

        public virtual DomName GetElementName(Type elementType) {
            return NodeFactory.GetElementName(elementType);
        }

        public virtual string GetProcessingInstructionTarget(Type processingInstructionType) {
            return NodeFactory.GetProcessingInstructionTarget(processingInstructionType);
        }

        public DomAttribute CreateAttribute(string name) {
            return CreateAttribute(DomName.Create(name));
        }

        public DomAttribute CreateAttribute(DomName name) {
            return AddUnlinked(CreateAttributeCore(name));
        }

        public DomAttribute CreateAttribute(DomName name, string value) {
            var result = CreateAttribute(name);
            result.Value = value;
            return result;
        }

        public DomAttribute CreateAttribute(DomName name, IDomValue value) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }

            var result = CreateAttribute(name);
            result.DomValue = value;
            return result;
        }

        public DomAttribute CreateAttribute(string name, IDomValue value) {
            return CreateAttribute(DomName.Create(name), value);
        }

        public DomAttribute CreateAttribute(string name, string value) {
            return CreateAttribute(DomName.Create(name), value);
        }

        protected virtual DomAttribute CreateAttributeCore(DomName name) {
            return ApplySchema(NodeFactory.CreateAttribute(name) ?? new DomAttribute(name));
        }

        public DomElement CreateElement(string name) {
            return CreateElement(DomName.Create(name));
        }

        public DomElement CreateElement(DomName name) {
            var e = CreateElementCore(name);
            return AddUnlinked(e);
        }

        protected virtual DomElement CreateElementCore(DomName name) {
            return ApplySchema(NodeFactory.CreateElement(name) ?? new DomElement(name));
        }

        public DomEntityReference CreateEntityReference(string name) {
            var result = CreateEntityReferenceCore(name);
            return AddUnlinked(result);
        }

        protected virtual DomEntityReference CreateEntityReferenceCore(string name) {
            return NodeFactory.CreateEntityReference(name) ?? new DomEntityReference(name);
        }

        public DomDocumentType CreateDocumentType(string name) {
            return AddUnlinked(CreateDocumentTypeCore(name));
        }

        public DomDocumentType CreateDocumentType(string name, string publicId, string systemId) {
            var result = CreateDocumentType(name);
            result.PublicId = publicId;
            result.SystemId = systemId;
            return result;
        }

        protected virtual DomDocumentType CreateDocumentTypeCore(string name) {
            return NodeFactory.CreateDocumentType(name) ?? new DomDocumentType(name);
        }

        public DomDocumentFragment CreateDocumentFragment() {
            var result = CreateDocumentFragmentCore();
            return AddUnlinked(result);
        }

        protected virtual DomDocumentFragment CreateDocumentFragmentCore() {
            return NodeFactory.CreateDocumentFragment();
        }

        public DomEntity CreateEntity(string name) {
            return AddUnlinked(CreateEntityCore(name));
        }

        protected virtual DomEntity CreateEntityCore(string name) {
            return NodeFactory.CreateEntity(name);
        }

        public DomNotation CreateNotation(string name) {
            return AddUnlinked(CreateNotationCore(name));
        }

        protected virtual DomNotation CreateNotationCore(string name) {
            return NodeFactory.CreateNotation(name);
        }

        internal void UpdateElementIndex(string id, DomElement element) {
        }

        private T AddUnlinked<T>(T item) where T : DomObject {
            Track(item);
            return item;
        }

        internal void Track(DomObject item) {
            item.Link(_unlinked);
            _unlinked.UnsafeAdd(item);
        }
    }

}
