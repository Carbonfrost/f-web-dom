//
// Copyright 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomElementQuery : IDomNodeManipulation<DomElementQuery> {

        private DomObjectQuery Query(Func<DomElement, DomNode> f) {
            return NewObjectQuery(_items.Select(f).NonNull());
        }

        private DomElementQuery Query(Func<DomElement, DomElement> f) {
            return NewElementQuery(_items.Select(f).NonNull());
        }

        private TValue QueryFirstOrDefault<TValue>(Func<DomElement, TValue> f) {
            var node = _items.FirstOrDefault();
            if (node == null) {
                return default(TValue);
            }
            else {
                return f(node);
            }
        }

        private DomElementQuery QueryMany(Func<DomElement, IEnumerable<DomElement>> f) {
            return NewElementQuery(_items.SelectMany(f).NonNull());
        }

        private DomObjectQuery QueryMany(Func<DomElement, IEnumerable<DomNode>> f) {
            return NewObjectQuery(_items.SelectMany(f).NonNull());
        }

        private DomObjectQuery QueryMany(Func<DomNode, IEnumerable<DomNode>> first, Func<DomNode, IEnumerable<DomNode>> rest) {
            var firstNode = _items.FirstOrDefault();
            if (firstNode == null) {
                return NewObjectQuery(_items);
            }
            return NewObjectQuery(Enumerable.Concat(
                first(firstNode),
                _items.Skip(1).Cast<DomNode>().SelectMany(n => rest(n))
            ));
        }

        private DomElementQuery Each(Action<DomElement> f) {
            foreach (var n in _items) {
                f(n);
            }
            return this;
        }

        private DomElementQuery Each(Action<DomElement> first, Action<DomElement> rest) {
            var e = _items.FirstOrDefault();
            if (e == null) {
                return this;
            }
            first(e);
            foreach (var n in _items.Skip(1)) {
                rest(n);
            }
            return this;
        }

        public DomElementQuery Empty() {
            foreach (var m in this) {
                m.Empty();
            }
            return this;
        }

        public DomElementQuery RemoveChildNodes() {
            return Empty();
        }

        public DomElementQuery RemoveSelf() {
            foreach (var m in this) {
                m.RemoveSelf();
            }
            return _Empty;
        }

        public DomElementQuery Remove() {
            return RemoveSelf();
        }

        public DomElementQuery RemoveAttributes() {
            return Query(m => m.RemoveAttributes());
        }

        public DomElementQuery AddClass(string className) {
            return Query(m => m.AddClass(className));
        }

        public DomElementQuery AddClass(params string[] classNames) {
            return Query(m => m.AddClass(classNames));
        }

        public DomElementQuery RemoveClass(string className) {
            return Query(m => m.RemoveClass(className));
        }

        public DomElementQuery RemoveClass(params string[] classNames) {
            return Query(m => m.RemoveClass(classNames));
        }

        public DomElementQuery SetName(string name) {
            return Query(m => m.SetName(name));
        }

        public DomElementQuery SetName(DomName name) {
            return Query(m => m.SetName(name));
        }

        public DomElementQuery Wrap(string element) {
            return Query(m => m.Wrap(element));
        }

        public DomElementQuery Wrap(DomName element) {
            return Query(m => m.Wrap(element));
        }

        public DomElementQuery Wrap(DomNode newParent) {
            if (newParent == null) {
                throw new ArgumentNullException(nameof(newParent));
            }

            newParent.Append(_items);
            return this;
        }

        public DomElementQuery After(DomNode nextSibling) {
            return Each(n => n.After(nextSibling));
        }

        public DomElementQuery After(params DomNode[] nextSiblings) {
            return Each(n => n.After(nextSiblings), n => n.After(DomNode.CloneAll(nextSiblings)));
        }

        public DomElementQuery After(string markup) {
            return After(Parse(markup));
        }

        public DomElementQuery After(IEnumerable<DomNode> nextSiblings) {
            if (nextSiblings == null) {
                throw new ArgumentNullException(nameof(nextSiblings));
            }
            return After(nextSiblings.ToArray());
        }

        public DomElementQuery InsertAfter(DomNode other) {
            return Each(n => n.InsertAfter(other));
        }

        public DomElementQuery Before(DomNode previousSibling) {
            return Each(n => n.Before(previousSibling));
        }

        public DomElementQuery Before(params DomNode[] previousSiblings) {
            return Each(n => n.Before(previousSiblings), n => n.Before(DomNode.CloneAll(previousSiblings)));
        }

        public DomElementQuery Before(string markup) {
            return Before(Parse(markup));
        }

        public DomElementQuery Before(IEnumerable<DomNode> previousSiblings) {
            if (previousSiblings == null) {
                throw new ArgumentNullException(nameof(previousSiblings));
            }
            return Before(previousSiblings.ToArray());
        }

        public DomElementQuery InsertBefore(DomNode other) {
            return Each(n => n.InsertBefore(other));
        }

        public DomWriter Append() {
            return DomWriter.Compose(this.Select(s => s.Append()));
        }

        public DomElementQuery Append(DomNode child) {
            return Each(n => n.Append(child), n => n.Append(child.Clone()));
        }

        public DomElementQuery Append(string markup) {
            return Append(Parse(markup));
        }

        public DomElementQuery Append(params DomNode[] nodes) {
            return Each(n => n.Append(nodes), n => n.Append(DomNode.CloneAll(nodes)));
        }

        public DomElementQuery AppendTo(DomNode parent) {
            return Each(n => n.AppendTo(parent));
        }

        public DomElementQuery Append(IEnumerable<DomNode> children) {
            if (children == null) {
                throw new ArgumentNullException(nameof(children));
            }
            return Append(children.ToArray());
        }

        public DomElementQuery Prepend(IEnumerable<DomNode> children) {
            if (children == null) {
                throw new ArgumentNullException(nameof(children));
            }
            return Prepend(children.ToArray());
        }

        public DomWriter Prepend() {
            throw new NotImplementedException();
        }

        public DomElementQuery Prepend(DomNode child) {
            return Each(n => n.Prepend(child), n => n.Prepend(child.Clone()));
        }

        public DomElementQuery Prepend(string markup) {
            return Prepend(Parse(markup));
        }

        public DomElementQuery Prepend(params DomNode[] nodes) {
            return Each(n => n.Append(nodes), n => n.Prepend(DomNode.CloneAll(nodes)));
        }

        public DomElementQuery PrependTo(DomNode parent) {
            return Each(n => n.PrependTo(parent));
        }

        public DomObjectQuery ReplaceWith(DomNode other) {
            if (other == null) {
                RemoveSelf();
                return ProviderFactory.EmptyObjectQuery;
            }
            return ReplaceWith(new [] { other });
        }

        public DomObjectQuery ReplaceWith(params DomNode[] others) {
            if (others == null) {
                throw new ArgumentNullException(nameof(others));
            }
            return QueryMany(
                n => DomObjectQuery._ReplaceWith(n, others),
                n => DomObjectQuery._ReplaceWith(n, DomNode.CloneAll(others))
            );
        }

        public DomObjectQuery ReplaceWith(IEnumerable<DomNode> others) {
            if (others == null) {
                throw new ArgumentNullException(nameof(others));
            }
            return ReplaceWith(others.ToArray());
        }

        public DomObjectQuery ReplaceWith(string markup) {
            return ReplaceWith(Parse(markup));
        }

        public DomObjectQuery AncestorNodes() {
            return QueryMany(n => n.AncestorNodes);
        }

        public DomObjectQuery AncestorNodesAndSelf() {
            return QueryMany(n => n.AncestorNodesAndSelf);
        }

        public DomObjectQuery DescendantNodes() {
            return QueryMany(n => n.DescendantNodes);
        }

        public DomObjectQuery DescendantNodesAndSelf() {
            return QueryMany(n => n.DescendantNodesAndSelf);
        }

        public DomObjectQuery FirstChildNode() {
            return Query(n => n.FirstChildNode);
        }

        public DomObjectQuery FollowingNodes() {
            return QueryMany(n => n.FollowingNodes);
        }

        public DomObjectQuery FollowingSiblingNodes() {
            return QueryMany(n => n.FollowingSiblingNodes);
        }

        public DomObjectQuery LastChildNode() {
            return Query(n => n.LastChildNode);
        }

        public DomObjectQuery NextSiblingNode() {
            return Query(n => n.NextSiblingNode);
        }

        public DomObjectQuery ParentNode() {
            return Query(n => n.ParentNode);
        }

        public DomObjectQuery PrecedingNodes() {
            return QueryMany(n => n.PrecedingNodes);
        }

        public DomObjectQuery PrecedingSiblingNodes() {
            return QueryMany(n => n.PrecedingSiblingNodes);
        }

        public DomObjectQuery PreviousSiblingNode() {
            return Query(n => n.PreviousSiblingNode);
        }

        // These methods provide the operations that match IDomNodeAppendApiConventions

        public DomElementQuery AppendElement(string name) {
            return Each(s => s.AppendElement(name));
        }

        public DomElementQuery AppendAttribute(string name, object value) {
            return Each(s => s.AppendAttribute(name, value));
        }

        public DomElementQuery AppendText(string data) {
            return Each(s => s.AppendText(data));
        }

        public DomElementQuery AppendCDataSection(string data) {
            return Each(s => s.AppendCDataSection(data));
        }

        public DomElementQuery AppendProcessingInstruction(string target, string data) {
            return Each(s => s.AppendProcessingInstruction(target, data));
        }

        public DomElementQuery AppendComment(string data) {
            return Each(s => s.AppendComment(data));
        }

        public DomElementQuery AppendDocumentType(string name) {
            return Each(s => s.AppendDocumentType(name));
        }

        public DomElementQuery PrependElement(string name) {
            return Each(s => s.PrependElement(name));
        }

        public DomElementQuery PrependAttribute(string name, object value) {
            return Each(s => s.PrependAttribute(name, value));
        }

        public DomElementQuery PrependText(string data) {
            return Each(s => s.PrependText(data));
        }

        public DomElementQuery PrependCDataSection(string data) {
            return Each(s => s.PrependCDataSection(data));
        }

        public DomElementQuery PrependProcessingInstruction(string target, string data) {
            return Each(s => s.PrependProcessingInstruction(target, data));
        }

        public DomElementQuery PrependComment(string data) {
            return Each(s => s.PrependComment(data));
        }

        public DomElementQuery PrependDocumentType(string name) {
            return Each(s => s.PrependDocumentType(name));
        }

        public DomObjectQuery Unwrap() {
            return NewObjectQuery(_items.SelectMany(i => i.UnwrapCore()));
        }

        private DomObjectQuery NewObjectQuery(IEnumerable<DomObject> items) {
            return ProviderFactory.CreateObjectQuery(items);
        }

        private DomElementQuery NewElementQuery(IEnumerable<DomElement> items) {
            return ProviderFactory.CreateElementQuery(items);
        }

        private DomNode[] Parse(string markup) {
            // TODO Depends on provider factory
            var doc = new DomDocument();
            var frag = doc.CreateDocumentFragment();
            frag.LoadXml(markup);
            return frag.ChildNodes.ToArray();
        }
    }

}
