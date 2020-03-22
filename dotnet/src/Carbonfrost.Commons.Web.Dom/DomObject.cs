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
using System.Reflection;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime.Expressions;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomObject  {

        private AnnotationList _annotations = AnnotationList.Empty;
        private IDomNodeCollection _siblingsContent;

        // Purely for the sake of reducing memory required by DomNode
        //   DomAttribute => IDomValue
        //   DomCharacterData => string
        //   DomContainer = > DomNodeCollection corresponding to children
        internal object content;

        // Only for tests
        internal AnnotationList AnnotationList {
            get {
                return _annotations;
            }
        }

        public virtual string LocalName {
            get {
                return NodeName;
            }
        }

        public virtual string NamespaceUri {
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

        internal DomAttributeCollection SiblingAttributes {
            get {
                return _siblingsContent as DomAttributeCollection;
            }
        }

        internal int SiblingIndex {
            get {
                return _siblingsContent.GetSiblingIndex(this);
            }
        }

        internal IDomNodeCollection _Siblings {
            get {
                return _siblingsContent as IDomNodeCollection;
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

        private DomNode OwnerNode {
            get {
                if (SiblingAttributes != null) {
                    return SiblingAttributes.OwnerElement;
                }

                if (_Siblings != null) {
                    return _Siblings.OwnerNode;
                }

                return null;
            }
        }

        internal static string CheckName(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            if (name.Length == 0) {
                throw Failure.EmptyString("name");
            }
            if (name.Any(char.IsWhiteSpace)) {
                throw DomFailure.CannotContainWhitespace("name");
            }
            return name;
        }

        internal static string RequireFactoryGeneratedName(Type type) {
            Type inputType = type;
            while (type != typeof(object)) {
                var fac = DomProviderFactory.ForProviderObject(type);
                if (fac != null) {
                    string name = fac.GenerateDefaultName(inputType);
                    if (name != null) {
                        return name;
                    }
                    if (fac is DefaultDomProviderFactory) {
                        break;
                    }
                }
                type = type.GetTypeInfo().BaseType;
            }

            throw DomFailure.CannotGenerateName(inputType);
        }

        public DomObject AddAnnotation(object annotation) {
            if (annotation == null) {
                throw new ArgumentNullException("annotation");
            }

            _annotations = _annotations.Add(annotation);
            return this;
        }

        public bool HasAnnotation<T>() where T : class {
            return _annotations.OfType<T>().Any();
        }

        public bool HasAnnotation(object instance) {
            return _annotations.Contains(instance);
        }

        public T Annotation<T>() where T : class {
            return _annotations.OfType<T>().FirstOrDefault();
        }

        public object Annotation(Type type) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }

            return _annotations.OfType(type).FirstOrDefault();
        }

        public DomObject AddAnnotations(IEnumerable<object> annotations) {
            if (annotations != null) {
                foreach (var anno in annotations) {
                    AddAnnotation(anno);
                }
            }
            return this;
        }

        public IEnumerable<object> Annotations() {
            return Annotations<object>();
        }

        public IEnumerable<T> Annotations<T>() where T : class {
            return _annotations.OfType<T>();
        }

        public IEnumerable<object> Annotations(Type type) {
            return _annotations.OfType(type);
        }

        public DomObject RemoveAnnotations<T>() where T : class {
            _annotations = _annotations.RemoveOfType(typeof(T));
            return this;
        }

        public DomObject RemoveAnnotations(Type type) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }

            _annotations = _annotations.RemoveOfType(type);
            return this;
        }

        public DomObject RemoveAnnotation(object value) {
            if (value == null) {
                throw new ArgumentNullException("value");
            }

            _annotations = _annotations.Remove(value);
            return this;
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

        internal void SetSiblingNodes(IDomNodeCollection newSiblings) {
            if (_siblingsContent != null && newSiblings != _siblingsContent) {
                var sc = _siblingsContent;
                sc.UnsafeRemove(this);
            }

            _siblingsContent = newSiblings;
        }

        internal void Unlinked() {
            IDomNodeCollection unlinked = OwnerDocument.UnlinkedNodes;
            unlinked.UnsafeAdd(this);
            _siblingsContent = unlinked;
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

        private sealed class BaseUriAnnotation {

            public readonly Uri uri;

            public BaseUriAnnotation(Uri uri) {
                this.uri = uri;
            }

        }
    }
}
