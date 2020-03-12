//
// Copyright 2013, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.IO;
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomNode : IDomNodeManipulation<DomNode>, IDomNodeAppendApiConventions, IDomNodeQuery<DomNode> {

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
            if (this.Attributes == null)
                return this;

            var attr = this.Attributes.GetByNameOrCreate(name);
            attr.SetTypedValue(value);
            return this;
        }

        public TValue Attribute<TValue>(string name) {
            var attr = Attributes[name];
            if (attr == null) {
                return default(TValue);
            }
            return attr.GetValue<TValue>();
        }

        public DomNode AddClass(string classNames) {
            ApplyClass(l => l.AddRange(DomStringTokenList.Parse(classNames)));
            return this;
        }

        public DomNode RemoveClass(string classNames) {
            ApplyClass(l => l.RemoveRange(DomStringTokenList.Parse(classNames)));
            return this;
        }

        private void ApplyClass(Action<DomStringTokenList> action) {
            var list = DomStringTokenList.Parse(Attribute("class"));
            action(list);
            Attribute("class", list.ToString());
        }

        public DomNode Before(DomNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            if (this.ParentNode == null)
                throw DomFailure.ParentNodeRequired();

            this.ParentNode.ChildNodes.Insert(this.NodePosition, node);
            return this;
        }

        public DomNode After(DomNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            if (this.ParentNode == null)
                throw DomFailure.ParentNodeRequired();

            this.ParentNode.ChildNodes.Insert(this.NodePosition + 1, node);
            return this;
        }

        public DomNode After(IEnumerable<DomNode> nodes) {
            if (nodes == null) {
                throw new ArgumentNullException("nodes");
            }
            ParentNode.ChildNodes.InsertRange(NodePosition + 1, nodes);
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

        // TODO Before and After implementations

        public DomNode Before(params DomNode[] previousSiblings) {
            if (previousSiblings == null || previousSiblings.Length == 0)
                return this;

            throw new NotImplementedException();
        }

        public DomNode InsertBefore(DomNode other) {
            if (other == null)
                throw new ArgumentNullException("other");

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
            if (childNodes == null)
                return this;

            foreach (var e in childNodes) {
                Append(e);
            }
            return this;
        }

        public DomNode Append(DomNode child) {
            if (child == null)
                return this;

            if (this.ChildNodes.IsReadOnly)
                throw DomFailure.CannotAppendChildNode();

            this.ChildNodes.Add(child);
            return this;
        }

        public DomNode Append(DomObject child) {
            if (child == null)
                return this;

            if (child.NodeType == DomNodeType.Attribute)
                this.Attributes.Add((DomAttribute) child);
            else {
                Append((DomNode) child);
            }

            return this;
        }

        public DomNode Append(string markup) {
            if (string.IsNullOrEmpty(markup))
                return this;

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
                throw new ArgumentNullException("parent");

            parent.Append(this);
            return this;
        }

        public DomAttribute AppendAttribute(string name, object value) {
            return Attributes.GetByNameOrCreate(name).AppendValue(value);
        }

        public DomAttribute PrependAttribute(string name, object value) {
            return Attributes.GetByNameOrCreate(name, true).AppendValue(value);
        }

        public DomElement AppendElement(string name) {
            var e = this.OwnerDocumentOrSelf.CreateElement(name);
            this.Append(e);
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

        public DomNode RemoveAttributes() {
            if (this.Attributes != null)
                this.Attributes.Clear();

            return this;
        }

        // TODO Verify that we can't replace certain types with others

        public DomNode ReplaceWith(DomNode other) {
            if (other == null)
                return this;

            return ReplaceWithCore(other);
        }

        public DomNode ReplaceWith(params DomNode[] others) {
            if (others == null || others.Length == 0) {
                return this;
            }

            var current = this;
            int index = 0;
            foreach (var m in others) {
                if (index == 0) {
                    ReplaceWith(m);
                } else {
                    current.After(m);
                }
                current = m;
                index++;
            }

            return others[0];
        }

        public DomNode ReplaceWith(IEnumerable<DomNode> others) {
            if (others == null)
                throw new ArgumentNullException("others");

            return ReplaceWith(others.ToArray());
        }

        public DomNode ReplaceWith(string markup) {
            AppendMarkup(AppendAfter(), markup);

            var nextSib = NextSiblingNode;
            RemoveSelf();
            return nextSib;
        }

        protected virtual DomNode ReplaceWithCore(DomNode other) {
            RequireParent().ChildNodes[this.NodePosition] = other;
            return other;
        }

        public DomNode Prepend(DomNode child) {
            if (child != null)
                ChildNodes.Insert(0, child);

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
            if (PreviousSiblingNode == null)
                throw new NotImplementedException();
            else
                return PreviousSiblingNode.AppendAfter();
        }

        public DomNode PrependTo(DomNode parent) {
            if (parent == null)
                throw new ArgumentNullException("parent");

            parent.Prepend(this);
            return this;
        }

        public new DomNode SetName(string name) {
            return (DomNode) base.SetName(name);
        }

        public DomNode Wrap(string element) {
            return Wrap(OwnerDocument.CreateElement(element));
        }

        public DomNode Wrap(DomNode newParent) {
            if (newParent == null) {
                throw new ArgumentNullException("newParent");
            }
            if (ParentNode != null && !newParent.IsDocumentFragment) {
                After(newParent);
            }
            newParent.Append(this);
            return newParent;
        }

        public DomNode Unwrap() {
            return UnwrapCore();
        }

        protected virtual DomNode UnwrapCore() {
            RequireParent();

            if (ChildNodes.Count == 0) {
                RemoveSelf();
                return null;
            }

            var firstChild = ChildNodes[0];
            ReplaceWith(ChildNodes.ToList());
            return firstChild;
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
