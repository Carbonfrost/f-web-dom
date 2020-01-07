//
// Copyright 2014, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using System.Reflection;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime.Expressions;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomObject {

        private AnnotationList annotations = AnnotationList.Empty;
        private IDomNodeCollection siblingsContent;

        // Purely for the sake of reducing memory required by DomNode
        //   DomAttribute => IDomValue
        //   DomCharacterData => string
        //   DomContainer = > DomNodeCollection corresponding to children
        internal object content;

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
                return this.NodeType == DomNodeType.Text;
            }
        }

        public bool IsCDataSection {
            get {
                return this.NodeType == DomNodeType.CDataSection;
            }
        }

        public bool IsCharacterData {
            get {
                return IsText || IsCDataSection;
            }
        }

        public bool IsElement {
            get {
                return this.NodeType == DomNodeType.Element;
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
                return siblingsContent as DomAttributeCollection;
            }
        }

        internal int SiblingIndex {
            get {
                return siblingsContent.GetSiblingIndex(this);
            }
        }

        internal IDomNodeCollection Siblings {
            get {
                return siblingsContent as IDomNodeCollection;
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

        public int LinePosition {
            get {
                if (OwnerDocument == null)
                    return -1;
                return OwnerDocument.GetLinePosition(this);
            }
        }

        public int LineNumber {
            get {
                if (OwnerDocument == null)
                    return -1;
                return OwnerDocument.GetLineNumber(this);
            }
        }

        public Uri BaseUri {
            get {
                var uc = this.Annotation<BaseUriAnnotation>();
                if (uc == null)
                    return ParentNode == null ? null: ParentNode.BaseUri;
                else
                    return uc.uri;
            }
            set {
                this.RemoveAnnotations<BaseUriAnnotation>();
                this.AddAnnotation(new BaseUriAnnotation(value));
            }
        }

        public DomDocument OwnerDocument {
            get {
                if (this.OwnerNode == null)
                    return null;

                if (this.OwnerNode.NodeType == DomNodeType.Document) {
                    return (DomDocument) this.OwnerNode;
                }

                return this.OwnerNode.OwnerDocument;
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
                if (this.SiblingAttributes != null)
                    return this.SiblingAttributes.OwnerElement;

                else if (this.Siblings != null)
                    return Siblings.OwnerNode;

                else
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
            if (annotation == null)
                throw new ArgumentNullException("annotation");

            this.annotations = this.annotations.Add(annotation);
            return this;
        }

        public bool HasAnnotation<T>() where T : class {
            return annotations.OfType<T>().Any();
        }

        public bool HasAnnotation(object instance) {
            return annotations.Contains(instance);
        }

        public T Annotation<T>() where T : class {
            return annotations.OfType<T>().FirstOrDefault();
        }

        public object Annotation(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            return annotations.OfType(type).FirstOrDefault();
        }

        public DomObject AddAnnotations(IEnumerable<object> annotations) {
            if (annotations != null) {
                foreach (var anno in annotations)
                    AddAnnotation(anno);
            }
            return this;
        }

        public IEnumerable<object> Annotations() {
            return Annotations<object>();
        }

        public IEnumerable<T> Annotations<T>() where T : class {
            return annotations.OfType<T>();
        }

        public IEnumerable<object> Annotations(Type type) {
            return annotations.OfType(type);
        }

        public DomObject RemoveAnnotations<T>() where T : class {
            this.annotations = this.annotations.RemoveOfType(typeof(T));
            return this;
        }

        public DomObject RemoveAnnotations(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            this.annotations = this.annotations.RemoveOfType(type);
            return this;
        }

        public DomObject RemoveAnnotation(object value) {
            if (value == null)
                throw new ArgumentNullException("value");

            this.annotations = this.annotations.Remove(value);
            return this;
        }

        public DomObject SetName(string name) {
            return SetNameCore(name);
        }

        protected virtual DomObject SetNameCore(string name) {
            throw DomFailure.CannotSetName();
        }

        internal void SetSiblingNodes(IDomNodeCollection newSiblings) {
            if (this.siblingsContent != null && newSiblings != this.siblingsContent) {
                var sc = this.siblingsContent;
                sc.UnsafeRemove(this);
            }

            this.siblingsContent = newSiblings;
        }

        internal void Unlinked() {
            IDomNodeCollection unlinked = OwnerDocument.UnlinkedNodes;
            unlinked.UnsafeAdd(this);
            this.siblingsContent = unlinked;
        }

        internal abstract void AcceptVisitor(IDomNodeVisitor visitor);
        internal abstract TResult AcceptVisitor<TArgument, TResult>(IDomNodeVisitor<TArgument, TResult> visitor, TArgument argument);

        public DomObject Remove() {
            return RemoveSelf();
        }

        public DomObject RemoveSelf() {
            if (this.siblingsContent != null) {
                this.siblingsContent.Remove(this);
            }

            return this;
        }

        void AssertSiblings(IDomNodeCollection newSiblings) {
            if (this.siblingsContent != null && newSiblings != null && newSiblings != this.siblingsContent)
                throw new Exception("Already has siblings " + this.siblingsContent);
            this.siblingsContent = newSiblings;
        }

        private sealed class BaseUriAnnotation {

            public readonly Uri uri;

            public BaseUriAnnotation(Uri uri) {
                this.uri = uri;
            }

        }
    }
}
