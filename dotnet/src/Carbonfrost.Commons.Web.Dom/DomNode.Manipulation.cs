//
// Copyright 2013, 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomNode
        : IDomNodeManipulation<DomNode>,
          IDomNodeQuery<DomNode>,
          IDomNodeAppendApiConventions {

        internal virtual DomDocument OwnerDocumentOrSelf {
            get {
                return this.OwnerDocument;
            }
        }

        public DomNode After(params DomNode[] nextSiblings) {
            if (nextSiblings == null || nextSiblings.Length == 0) {
                return this;
            }
            return After((IEnumerable<DomNode>) nextSiblings);
        }

        public DomNode Attribute(string name, object value) {
            return Attribute(DomName.Create(name), value);
        }

        public DomNode Attribute(DomName name, object value) {
            if (Attributes == null) {
                return this;
            }

            var attr = AttributeByNameOrCreate(name);
            attr.SetValue(value);
            return this;
        }

        public TValue Attribute<TValue>(string name) {
            var attr = Attributes[name];
            if (attr == null) {
                return default(TValue);
            }
            return attr.GetValue<TValue>();
        }

        public TValue Attribute<TValue>(DomName name) {
            var attr = Attributes[name];
            if (attr == null) {
                return default(TValue);
            }
            return attr.GetValue<TValue>();
        }

        public DomNode AddClass(string classNames) {
            return ApplyClass(l => l.AddRange(DomStringTokenList.Parse(classNames)));
        }

        public DomNode AddClass(params string[] classNames) {
            return ApplyClass(l => l.AddRange(classNames));
        }

        public DomNode RemoveClass(string classNames) {
            return ApplyClass(l => l.RemoveRange(DomStringTokenList.Parse(classNames)));
        }

        private DomNode ApplyClass(Action<DomStringTokenList> action) {
            var list = DomStringTokenList.Parse(Attribute("class"));
            action(list);
            return Attribute("class", list.ToString());
        }

        public DomNode Before(DomNode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            RequireParent().ChildNodes.Insert(this.NodePosition, node);
            return this;
        }

        public DomNode After(DomNode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            RequireParent().ChildNodes.Insert(this.NodePosition + 1, node);
            return this;
        }

        public DomNode Before(IEnumerable<DomNode> previousSiblings) {
            if (previousSiblings == null) {
                throw new ArgumentNullException(nameof(previousSiblings));
            }
            return Before(previousSiblings.ToArray());
        }

        public DomNode After(IEnumerable<DomNode> nextSiblings) {
            if (nextSiblings == null) {
                throw new ArgumentNullException(nameof(nextSiblings));
            }
            RequireParent().ChildNodes.InsertRange(NodePosition + 1, nextSiblings);
            return this;
        }

        public DomNode InsertAfter(DomNode other) {
            other.After(this);
            return this;
        }

        public DomNode After(string markup) {
            var writer = AppendAfter();
            return AppendMarkup(writer, markup);
        }

        public DomNode Before(string markup) {
            var writer = PrependBefore();
            return AppendMarkup(writer, markup);
        }

        public DomNode Before(params DomNode[] previousSiblings) {
            if (previousSiblings == null) {
                throw new ArgumentNullException(nameof(previousSiblings));
            }
            if (previousSiblings.Length == 0) {
                return this;
            }

            RequireParent().ChildNodes.InsertRange(NodePosition, previousSiblings);
            return this;
        }

        public DomNode InsertBefore(DomNode other) {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            other.Before(this);
            return this;
        }

        public DomNode Append(DomNode childNode0, DomNode childNode1) {
            return Append(childNode0).Append(childNode1);
        }

        public DomNode Append(DomNode childNode0, DomNode childNode1, DomNode childNode2) {
            return Append(childNode0).Append(childNode1).Append(childNode2);
        }

        public DomNode Append(params DomNode[] childNodes) {
            return Append((IEnumerable<DomNode>) childNodes);
        }

        public DomNode Append(IEnumerable<DomNode> childNodes) {
            if (childNodes == null) {
                return this;
            }

            // TODO We always copy the child nodes because it is possible this is operating on a
            // collection that will be concurrently modified.  We could detect this situation (performance)

            foreach (var e in childNodes.ToList()) {
                Append(e);
            }
            return this;
        }

        public DomNode Append(DomNode child) {
            if (child == null) {
                return this;
            }

            if (ChildNodes.IsReadOnly) {
                throw DomFailure.CannotAppendChildNode();
            }

            ChildNodes.Add(child);
            return this;
        }

        public DomNode Append(DomObject child) {
            if (child == null) {
                return this;
            }

            if (child.NodeType == DomNodeType.Attribute) {
                Attributes.Add((DomAttribute) child);
            } else {
                Append((DomNode) child);
            }

            return this;
        }

        public DomNode Append(string markup) {
            if (string.IsNullOrEmpty(markup)) {
                return this;
            }

            using (var stringReader = new StringReader(markup)) {
                using (DomReader reader = this.OwnerDocument.ProviderFactory.CreateReader(stringReader)) {

                    var writer = this.OwnerDocument.ProviderFactory.CreateWriter(this, null);
                    reader.CopyTo(writer);
                }
            }

            return this;
        }

        public DomNode AppendTo(DomNode parent) {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            parent.Append(this);
            return this;
        }

        public DomAttribute AppendAttribute(string name) {
            return AttributeByNameOrCreate(CreateDomName(name));
        }

        public DomAttribute AppendAttribute(DomName name) {
            return AttributeByNameOrCreate(name);
        }

        public DomAttribute AppendAttribute(string name, object value) {
            return AppendAttribute(DomName.Create(name), value);
        }

        public DomAttribute AppendAttribute(DomName name, object value) {
            return AppendAttribute(name).AppendValue(value);
        }

        public DomAttribute PrependAttribute(string name) {
            return PrependAttribute(CreateDomName(name));
        }

        public DomAttribute PrependAttribute(DomName name) {
            return AttributeByNameOrCreate(name, true);
        }

        public DomAttribute PrependAttribute(string name, object value) {
            return PrependAttribute(DomName.Create(name), value);
        }

        public DomAttribute PrependAttribute(DomName name, object value) {
            return PrependAttribute(name).AppendValue(value);
        }

        public DomElement AppendElement(string name) {
            return AppendElement(CreateDomName(name));
        }

        public DomElement AppendElement(DomName name) {
            var e = OwnerDocumentOrSelf.CreateElement(name);
            Append(e);
            return e;
        }

        public DomText PrependText(string data) {
            var e = this.OwnerDocumentOrSelf.CreateText(data);
            this.Prepend(e);
            return e;
        }

        public DomText AppendText(string data) {
            var e = this.OwnerDocumentOrSelf.CreateText(data);
            this.Append(e);
            return e;
        }

        public DomCDataSection PrependCDataSection(string data) {
            var e = OwnerDocumentOrSelf.CreateCDataSection(data);
            Prepend(e);
            return e;
        }

        public DomCDataSection AppendCDataSection(string data) {
            var e = OwnerDocumentOrSelf.CreateCDataSection(data);
            Append(e);
            return e;
        }

        public DomComment PrependComment(string data) {
            var e = OwnerDocumentOrSelf.CreateComment(data);
            Prepend(e);
            return e;
        }

        public DomComment AppendComment(string data) {
            var e = OwnerDocumentOrSelf.CreateComment(data);
            Append(e);
            return e;
        }

        public DomProcessingInstruction AppendProcessingInstruction(string target, string data) {
            var e = OwnerDocumentOrSelf.CreateProcessingInstruction(target, data);
            Append(e);
            return e;
        }

        public DomDocumentType AppendDocumentType(string name) {
            var e = OwnerDocumentOrSelf.CreateDocumentType(name);
            Append(e);
            return e;
        }

        public DomElement PrependElement(string name) {
            return PrependElement(CreateDomName(name));
        }

        public DomElement PrependElement(DomName name) {
            var e = OwnerDocumentOrSelf.CreateElement(name);
            Prepend(e);
            return e;
        }

        public DomProcessingInstruction PrependProcessingInstruction(string target, string data) {
            var e = OwnerDocumentOrSelf.CreateProcessingInstruction(target, data);
            Prepend(e);
            return e;
        }

        public DomDocumentType PrependDocumentType(string name) {
            var e = OwnerDocumentOrSelf.CreateDocumentType(name);
            Prepend(e);
            return e;
        }

        public new DomNode Remove() {
            return (DomNode) RemoveSelf();
        }

        public new DomNode RemoveSelf() {
            return (DomNode) base.RemoveSelf();
        }

        public DomNode RemoveAttributes() {
            if (Attributes != null) {
                Attributes.Clear();
            }

            return this;
        }

        // TODO Verify that we can't replace certain types with others

        public DomNode ReplaceWith(DomNode other) {
            if (other == null) {
                return this;
            }

            return ReplaceWithCore(other);
        }

        public DomObjectQuery ReplaceWith(params DomNode[] others) {
            if (others == null) {
                throw new ArgumentNullException(nameof(others));
            }
            return new DomObjectQuery(this).ReplaceWith(others);
        }

        public DomObjectQuery ReplaceWith(IEnumerable<DomNode> others) {
            if (others == null) {
                throw new ArgumentNullException(nameof(others));
            }

            return ReplaceWith(others.ToArray());
        }

        public DomObjectQuery ReplaceWith(string markup) {
            return new DomObjectQuery(this).ReplaceWith(markup);
        }

        protected virtual DomNode ReplaceWithCore(DomNode other) {
            RequireParent().ChildNodes[this.NodePosition] = other;
            return other;
        }

        public DomNode Prepend(DomNode child) {
            if (child != null) {
                ChildNodes.Insert(0, child);
            }

            return this;
        }

        public DomNode Prepend(DomNode childNode0, DomNode childNode1) {
            return Prepend(childNode1).Prepend(childNode0);
        }

        public DomNode Prepend(DomNode childNode0, DomNode childNode1, DomNode childNode2) {
            return Prepend(childNode2).Prepend(childNode1).Prepend(childNode0);
        }

        public DomNode Prepend(params DomNode[] childNodes) {
            return Prepend((IEnumerable<DomNode>) childNodes);
        }

        public DomNode Prepend(IEnumerable<DomNode> childNodes) {
            if (childNodes == null)
                return this;

            ChildNodes.InsertRange(0, childNodes);
            return this;
        }

        public DomNode Prepend(string markup) {
            return AppendMarkup(PrependBefore(), markup);
        }

        public DomWriter PrependBefore() {
            if (PreviousSiblingNode == null) {
                return ParentElement.Prepend();
            }

            return PreviousSiblingNode.AppendAfter();
        }

        public DomNode PrependTo(DomNode parent) {
            if (parent == null) {
                throw new ArgumentNullException(nameof(parent));
            }

            parent.Prepend(this);
            return this;
        }

        public new DomNode SetName(string name) {
            return (DomNode) base.SetName(name);
        }

        public new DomNode SetName(DomName name) {
            return (DomNode) base.SetName(name);
        }

        public DomNode Wrap(string element) {
            return Wrap(CreateDomName(element));
        }

        public DomNode Wrap(DomName element) {
            return Wrap(OwnerDocument.CreateElement(element));
        }

        public DomNode Wrap(DomNode newParent) {
            if (newParent == null) {
                throw new ArgumentNullException(nameof(newParent));
            }
            if (ParentNode != null && !newParent.IsDocumentFragment) {
                After(newParent);
            }
            newParent.Append(this);
            return newParent;
        }

        public DomObjectQuery Unwrap() {
            return new DomObjectQuery(UnwrapCore());
        }

        internal virtual void AssertCanUnwrap() {
            RequireParent();
        }

        internal IEnumerable<DomNode> UnwrapCore() {
            AssertCanUnwrap();

            if (ChildNodes.Count == 0) {
                RemoveSelf();
                return Enumerable.Empty<DomNode>();
            }

            var items = ChildNodes.ToList();
            ReplaceWith(items);
            return items;
        }

        public TNode ReplaceWith<TNode>(TNode other) where TNode : DomNode {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            return (TNode) ReplaceWith((DomNode) other);
        }

        internal DomAttribute AttributeByNameOrCreate(DomName name, bool insertFirst = false) {
            var attr = Attributes[name];
            if (attr == null) {
                // Owner doc could be null (rare)
                var doc = OwnerDocument;
                if (doc == null) {
                    attr = DomProviderFactory.ForProviderObject(this).CreateNodeFactory(null).CreateAttribute(name);
                } else {
                    attr = doc.CreateAttribute(name);
                }
                if (insertFirst) {
                    Attributes.Insert(0, attr);
                } else {
                    Attributes.Add(attr);
                }
            }
            return attr;
        }

        private DomNode AppendMarkup(DomWriter writer, string markup) {
            if (string.IsNullOrEmpty(markup))
                return this;

            using (var stringReader = new StringReader(markup)) {
                using (DomReader reader = this.OwnerDocument.ProviderFactory.CreateReader(stringReader)) {
                    reader.CopyTo(writer);
                }
            }

            return this;
        }
    }
}
