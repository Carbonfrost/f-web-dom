//
// Copyright 2014, 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using Carbonfrost.Commons.Core.Runtime.Expressions;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract partial class DomObject : IDomNameApiConventions {

        private IDomNodeCollection _siblingsContent;

        // Purely for the sake of reducing memory required by DomNode
        //   DomAttribute => IDomValue
        //   DomCharacterData => string
        //   DomContainer = > DomNodeCollection corresponding to children
        internal object content;

        public virtual string LocalName {
            get {
                return Name.LocalName;
            }
        }

        public virtual DomName Name {
            get {
                return DomName.Create(NodeName);
            }
        }

        public string NamespaceUri {
            get {
                if (Namespace == null) {
                    return null;
                }
                return Namespace.NamespaceUri;
            }
        }

        public virtual DomNamespace Namespace {
            get {
                return null;
            }
        }

        public DomElement ParentElement {
            get {
                return ParentNode as DomElement;
            }
        }

        internal DomContainer ContainerOrSelf {
            get {
                return (this as DomContainer) ?? ParentElement;
            }
        }

        public DomNodeCollection ChildNodes {
            get {
                return this.DomChildNodes;
            }
        }

        protected abstract DomNodeCollection DomChildNodes { get; }

        public virtual DomNode PreviousSiblingNode {
            get {
                return null;
            }
        }

        public virtual DomNode NextSiblingNode {
            get {
                return null;
            }
        }

        public bool IsDocument {
            get {
                return NodeType == DomNodeType.Document;
            }
        }

        public bool IsDocumentFragment {
            get {
                return NodeType == DomNodeType.DocumentFragment;
            }
        }

        public bool IsText {
            get {
                return NodeType == DomNodeType.Text;
            }
        }

        public bool IsCDataSection {
            get {
                return NodeType == DomNodeType.CDataSection;
            }
        }

        public bool IsCharacterData {
            get {
                return IsText || IsCDataSection;
            }
        }

        public bool IsElement {
            get {
                return NodeType == DomNodeType.Element;
            }
        }

        public virtual bool IsContainer {
            get {
                return false;
            }
        }

        public bool IsProcessingInstruction {
            get {
                return NodeType == DomNodeType.ProcessingInstruction;
            }
        }

        public bool IsAttribute {
            get {
                return NodeType == DomNodeType.Attribute;
            }
        }

        internal DomAttributeCollectionApi SiblingAttributes {
            get {
                return _siblingsContent as DomAttributeCollectionApi;
            }
        }

        internal int SiblingIndex {
            get {
                return _siblingsContent.GetSiblingIndex(this);
            }
        }

        internal IDomNodeCollection _Siblings {
            get {
                return _siblingsContent;
            }
        }

        public abstract string NodeName { get; }
        public abstract DomNodeType NodeType { get; }
        public abstract string TextContent { get; set; }

        [ExpressionSerializationMode(ExpressionSerializationMode.Hidden)]
        public virtual string NodeValue {
            get {
                return null;
            }
            set
            {
            }
        }

        public bool IsUnlinked {
            get {
                if (OwnerDocument == null) {
                    return true;
                }
                return OwnerDocument.UnlinkedNodes.Contains(this);
            }
        }

        public int LinePosition {
            get {
                if (OwnerDocument == null) {
                    return -1;
                }
                return OwnerDocument.GetLinePosition(this);
            }
        }

        public int LineNumber {
            get {
                if (OwnerDocument == null) {
                    return -1;
                }
                return OwnerDocument.GetLineNumber(this);
            }
        }

        public Uri BaseUri {
            get {
                var uc = Annotation<BaseUriAnnotation>();
                if (uc == null) {
                    return ParentNode == null ? null: ParentNode.BaseUri;
                }
                return uc.uri;
            }
            set {
                RemoveAnnotations<BaseUriAnnotation>();
                if (value != null) {
                    AddAnnotation(new BaseUriAnnotation(value));
                }
            }
        }

        public DomDocument OwnerDocument {
            get {
                return DomOwnerDocument;
            }
        }

        private protected virtual DomDocument DomOwnerDocument {
            get {
                if (OwnerNode == null) {
                    return null;
                }
                if (OwnerNode.NodeType == DomNodeType.Document) {
                    return (DomDocument) OwnerNode;
                }
                return OwnerNode.OwnerDocument;
            }
        }

        public virtual DomNode ParentNode {
            get {
                var owner = OwnerNode;

                if (owner == null)
                    return null;
                else if (owner.NodeType == DomNodeType.Document && ((DomDocument) owner).UnlinkedNodes.Contains(this))
                    return null;
                else
                    return owner;
            }
        }

        public virtual DomNode RootNode {
            get {
                if (ParentNode == null) {
                    return (DomNode) this;
                }
                return ParentNode.RootNode;
            }
        }

        private DomNode OwnerNode {
            get {
                if (_Siblings != null) {
                    return _Siblings.OwnerNode;
                }

                return null;
            }
        }

        internal static DomName CheckName(DomName name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            return name;
        }

        internal static T RequireFactoryGeneratedName<T>(Type type, Func<IDomNodeTypeProvider, Type, T> generator) {
            Type inputType = type;
            while (type != typeof(object)) {
                var fac = DomProviderFactory.ForProviderObject(type);
                if (fac != null) {
                    var name = generator(fac.NodeTypeProvider, inputType);
                    if (name != null) {
                        return name;
                    }
                    if (fac is DefaultDomProviderFactory) {
                        break;
                    }
                }
                type = type.BaseType;
            }

            throw DomFailure.CannotGenerateName(inputType);
        }

        public DomObject SetName(string name) {
            return SetNameCore(name);
        }

        protected virtual DomObject SetNameCore(string name) {
            throw DomFailure.CannotSetName();
        }

        internal DomProviderFactory FindProviderFactory() {
            if (OwnerDocument == null) {
                return DomProviderFactory.Default;
            }
            return OwnerDocument.ProviderFactory;
        }

        internal void Unlink() {
            var unlinked = OwnerDocument.UnlinkedNodes;
            if (_siblingsContent == unlinked) {
                return;
            }
            unlinked.UnsafeAdd(this);
            _siblingsContent = unlinked;
        }

        internal void Link(IDomNodeCollection newSiblings) {
            if (_siblingsContent == newSiblings) {
                return;
            }
            if (_siblingsContent != null) {
                _siblingsContent.Remove(this);
            }
            _siblingsContent = newSiblings;
        }

        public DomObject Remove() {
            return RemoveSelf();
        }

        public DomObject RemoveSelf() {
            if (_siblingsContent != null) {
                _siblingsContent.Remove(this);
            }

            return this;
        }

        internal DomName CreateDomName(string name) {
            return DomName.Create(name);
        }

        private sealed class BaseUriAnnotation {

            public readonly Uri uri;

            public BaseUriAnnotation(Uri uri) {
                this.uri = uri;
            }

        }
    }
}
