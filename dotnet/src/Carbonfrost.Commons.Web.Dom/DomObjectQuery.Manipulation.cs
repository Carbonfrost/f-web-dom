//
// Copyright 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

    partial class DomObjectQuery
        : IDomNodeManipulation<DomObjectQuery>,
          IDomAttributeManipulation<DomObjectQuery>,
          IDomNodeQueryManipulation {

        private DomObjectQuery Query(Func<DomNode, DomNode> f) {
            return new DomObjectQuery(Nodes().Select(f).NonNull());
        }

        private TValue QueryFirstOrDefault<TValue>(Func<DomNode, TValue> f) {
            var node = Nodes().FirstOrDefault();
            if (node == null) {
                return default(TValue);
            }
            else {
                return f(node);
            }
        }

        private DomObjectQuery QueryMany(Func<DomNode, IEnumerable<DomNode>> f) {
            return new DomObjectQuery(Nodes().SelectMany(f).NonNull());
        }

        private DomObjectQuery QueryMany(Func<DomNode, IEnumerable<DomNode>> first, Func<DomNode, IEnumerable<DomNode>> rest) {
            var nodes = Nodes();
            var firstNode = nodes.FirstOrDefault();
            if (firstNode == null) {
                return this;
            }
            return new DomObjectQuery(Enumerable.Concat(
                first(firstNode),
                nodes.Skip(1).Cast<DomNode>().SelectMany(n => rest(n))
            ));
        }

        private DomObjectQuery Each(Action<DomNode> f) {
            foreach (var n in Nodes()) {
                f(n);
            }
            return this;
        }

        private DomObjectQuery Each(Action<DomNode> first, Action<DomNode> rest) {
            var e = Nodes().FirstOrDefault();
            if (e == null) {
                return this;
            }
            first(e);
            foreach (var n in Nodes().Skip(1)) {
                rest(n);
            }
            return this;
        }

        private DomObjectQuery Each(Action<DomAttribute> f) {
            foreach (var n in Attributes()) {
                f(n);
            }
            return this;
        }

        private DomObjectQuery Each(Action<DomAttribute> first, Action<DomAttribute> rest) {
            var e = Attributes().FirstOrDefault();
            if (e == null) {
                return this;
            }
            first(e);
            foreach (var n in Attributes().Skip(1)) {
                rest(n);
            }
            return this;
        }

        public DomObjectQuery Unwrap() {
            return QueryMany(m => m.UnwrapCore());
        }

        public DomObjectQuery RemoveAttributes() {
            return Query(m => m.RemoveAttributes());
        }

        public DomObjectQuery RemoveAttribute(string name) {
            return Query(m => m.RemoveAttribute(name));
        }

        public DomObjectQuery RemoveAttribute(DomName name) {
            return Query(m => m.RemoveAttribute(name));
        }

        public DomObjectQuery AddClass(string className) {
            return Query(m => m.AddClass(className));
        }

        public DomObjectQuery AddClass(params string[] classNames) {
            return Query(m => m.AddClass(classNames));
        }

        public DomObjectQuery RemoveClass(string className) {
            return Query(m => m.RemoveClass(className));
        }

        public DomObjectQuery RemoveClass(params string[] classNames) {
            return Query(m => m.RemoveClass(classNames));
        }

        public DomObjectQuery SetName(string name) {
            return new DomObjectQuery(SetNameInternal(name));
        }

        public DomObjectQuery SetName(DomName name) {
            return new DomObjectQuery(SetNameInternal(name));
        }

        public DomObjectQuery Wrap(string element) {
            return Each(m => m.Wrap(element));
        }

        public DomObjectQuery Wrap(DomName element) {
            return Each(m => m.Wrap(element));
        }

        public DomObjectQuery Wrap(DomContainer newParent) {
            if (newParent == null) {
                throw new ArgumentNullException(nameof(newParent));
            }
            newParent.RemoveSelf();
            return Each(m => m.Wrap(newParent.Clone()), m => m.Wrap(newParent.Clone()));
        }

        public DomObjectQuery After(DomNode nextSibling) {
            return Each(n => n.After(nextSibling));
        }

        public DomObjectQuery After(params DomNode[] nextSiblings) {
            return Each(n => n.After(nextSiblings), n => n.After(DomNode.CloneAll(nextSiblings)));
        }

        public DomObjectQuery After(string markup) {
            return After(Parse(markup));
        }

        public DomObjectQuery InsertAfter(DomNode other) {
            return Each(n => n.InsertAfter(other));
        }

        public DomObjectQuery Before(DomNode previousSibling) {
            return Each(n => n.Before(previousSibling));
        }

        public DomObjectQuery Before(params DomNode[] previousSiblings) {
            return Each(n => n.Before(previousSiblings), n => n.Before(DomNode.CloneAll(previousSiblings)));
        }

        public DomObjectQuery Before(string markup) {
            return Before(Parse(markup));
        }

        public DomObjectQuery InsertBefore(DomNode other) {
            return Each(n => n.InsertBefore(other));
        }

        public DomWriter Append() {
            throw new NotImplementedException();
        }

        public DomObjectQuery Append(DomNode child) {
            return Each(n => n.Append(child), n => n.Append(child.Clone()));
        }

        public DomObjectQuery Append(string markup) {
            return Append(Parse(markup));
        }

        public DomObjectQuery Append(params DomNode[] nodes) {
            return Each(n => n.Append(nodes), n => n.Append(DomNode.CloneAll(nodes)));
        }

        public DomObjectQuery AppendTo(DomNode parent) {
            return Each(n => n.AppendTo(parent));
        }

        public DomWriter Prepend() {
            throw new NotImplementedException();
        }

        public DomObjectQuery Prepend(DomNode child) {
            return Each(n => n.Prepend(child), n => n.Prepend(child.Clone()));
        }

        public DomObjectQuery Prepend(params DomNode[] nodes) {
            return Each(n => n.Prepend(nodes), n => n.Prepend(DomNode.CloneAll(nodes)));
        }

        public DomObjectQuery Prepend(string markup) {
            return Prepend(Parse(markup));
        }

        public DomObjectQuery PrependTo(DomNode parent) {
            return Each(n => n.PrependTo(parent));
        }

        public DomObjectQuery ReplaceWith(DomNode other) {
            if (other == null) {
                return RemoveSelf();
            }
            return ReplaceWith(new [] { other });
        }

        public DomObjectQuery ReplaceWith(params DomNode[] others) {
            if (others == null) {
                throw new ArgumentNullException(nameof(others));
            }

            if (others.Length == 0) {
                return RemoveSelf();
            }

            return QueryMany(n => _ReplaceWith(n, others), n => _ReplaceWith(n, DomNode.CloneAll(others)));
        }

        public DomObjectQuery ReplaceWith(IEnumerable<DomNode> others) {
            if (others == null) {
                throw new ArgumentNullException(nameof(others));
            }
            return ReplaceWith(others.ToArray());
        }

        internal static IEnumerable<DomNode> _ReplaceWith(DomNode node, DomNode[] others) {
            var current = node;
            int index = 0;
            foreach (var m in others) {
                if (index == 0) {
                    node.ReplaceWith(m);
                } else {
                    current.After(m);
                }
                current = m;
                index++;
            }
            return others;
        }

        public DomObjectQuery ReplaceWith(string markup) {
            return ReplaceWith(Parse(markup));
        }

        private IEnumerable<DomNode> SetNameInternal(string name) {
            foreach (var m in Nodes()) {
                if (m.IsAttribute || m.IsElement) {
                    yield return m.SetName(name);
                } else {
                    yield return m;
                }
            }
        }

        private IEnumerable<DomNode> SetNameInternal(DomName name) {
            foreach (var m in Nodes()) {
                if (m.IsAttribute || m.IsElement) {
                    yield return m.SetName(name);
                } else {
                    yield return m;
                }
            }
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

        public DomObjectQuery AppendElement(string name) {
            return Each(s => s.AppendElement(name));
        }

        public DomObjectQuery AppendElement(DomName name) {
            return Each(s => s.AppendElement(name));
        }

        public DomObjectQuery AppendAttribute(string name, object value) {
            return Each(s => s.AppendAttribute(name, value));
        }

        public DomObjectQuery AppendAttribute(DomName name, object value) {
            return Each(s => s.AppendAttribute(name, value));
        }

        public DomObjectQuery AppendText(string data) {
            return Each(s => s.AppendText(data));
        }

        public DomObjectQuery AppendCDataSection(string data) {
            return Each(s => s.AppendCDataSection(data));
        }

        public DomObjectQuery AppendProcessingInstruction(string target, string data) {
            return Each(s => s.AppendProcessingInstruction(target, data));
        }

        public DomObjectQuery AppendComment(string data) {
            return Each(s => s.AppendComment(data));
        }

        public DomObjectQuery AppendDocumentType(string name) {
            return Each(s => s.AppendDocumentType(name));
        }

        public DomObjectQuery PrependElement(string name) {
            return Each(s => s.PrependElement(name));
        }

        public DomObjectQuery PrependElement(DomName name) {
            return Each(s => s.PrependElement(name));
        }

        public DomObjectQuery PrependAttribute(string name, object value) {
            return Each(s => s.PrependAttribute(name, value));
        }

        public DomObjectQuery PrependAttribute(DomName name, object value) {
            return Each(s => s.PrependAttribute(name, value));
        }

        public DomObjectQuery PrependText(string data) {
            return Each(s => s.PrependText(data));
        }

        public DomObjectQuery PrependCDataSection(string data) {
            return Each(s => s.PrependCDataSection(data));
        }

        public DomObjectQuery PrependProcessingInstruction(string target, string data) {
            return Each(s => s.PrependProcessingInstruction(target, data));
        }

        public DomObjectQuery PrependComment(string data) {
            return Each(s => s.PrependComment(data));
        }

        public DomObjectQuery PrependDocumentType(string name) {
            return Each(s => s.PrependDocumentType(name));
        }

        private DomNode[] Parse(string markup) {
            // TODO Depends on provider factory
            var doc = new DomDocument();
            var frag = doc.CreateDocumentFragment();
            frag.LoadXml(markup);
            return frag.ChildNodes.ToArray();
        }

        public DomObjectQuery After(IEnumerable<DomNode> nextSiblings) {
            if (nextSiblings == null) {
                throw new ArgumentNullException(nameof(nextSiblings));
            }
            return After(nextSiblings.ToArray());
        }

        public DomObjectQuery Before(IEnumerable<DomNode> previousSiblings) {
            if (previousSiblings == null) {
                throw new ArgumentNullException(nameof(previousSiblings));
            }
            return Before(previousSiblings.ToArray());
        }

        public DomObjectQuery Append(IEnumerable<DomNode> children) {
            if (children == null) {
                throw new ArgumentNullException(nameof(children));
            }
            return Append(children.ToArray());
        }

        public DomObjectQuery Prepend(IEnumerable<DomNode> children) {
            if (children == null) {
                throw new ArgumentNullException(nameof(children));
            }
            return Prepend(children.ToArray());
        }

        public DomObjectQuery Before(DomAttribute previousAttribute) {
            if (previousAttribute == null) {
                throw new ArgumentNullException(nameof(previousAttribute));
            }
            return Each(n => n.Before(previousAttribute));
        }

        public DomObjectQuery Before(params DomAttribute[] previousAttributes) {
            if (previousAttributes == null) {
                throw new ArgumentNullException(nameof(previousAttributes));
            }
            return Each(n => n.Before(previousAttributes), n => n.Before(DomAttribute.CloneAll(previousAttributes)));
        }

        public DomObjectQuery Before(IEnumerable<DomAttribute> previousAttributes) {
            if (previousAttributes == null) {
                throw new ArgumentNullException(nameof(previousAttributes));
            }
            return Before(previousAttributes.ToArray());
        }

        public DomObjectQuery InsertBefore(DomAttribute other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return Each(n => n.InsertBefore(other));
        }

        public DomObjectQuery After(DomAttribute nextAttribute) {
            if (nextAttribute == null) {
                throw new ArgumentNullException(nameof(nextAttribute));
            }
            return After(new [] { nextAttribute });
        }

        public DomObjectQuery After(params DomAttribute[] nextAttributes) {
            if (nextAttributes == null) {
                throw new ArgumentNullException(nameof(nextAttributes));
            }
            return Each(n => n.After(nextAttributes), n => n.After(DomAttribute.CloneAll(nextAttributes)));
        }

        public DomObjectQuery After(IEnumerable<DomAttribute> nextAttributes) {
            if (nextAttributes == null) {
                throw new ArgumentNullException(nameof(nextAttributes));
            }
            return After(nextAttributes.ToArray());
        }

        public DomObjectQuery InsertAfter(DomAttribute other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return Each(n => n.InsertBefore(other));
        }
    }
}
