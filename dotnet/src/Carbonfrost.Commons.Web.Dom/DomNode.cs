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
using System.Linq;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime.Expressions;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract partial class DomNode : DomObject {

        public int NodePosition {
            get {
                return SiblingIndex;
            }
        }

        public int NodeDepth {
            get {
                if (ParentNode == null) {
                    return 0;
                }
                return ParentNode.NodeDepth + 1;
            }
        }

        protected DomNode() {
        }

        public string Attribute(string name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (name.Length == 0) {
                throw Failure.EmptyString(nameof(name));
            }

            if (Attributes == null) {
                Traceables.IgnoredAttributes();
                return null;
            }
            var attr = Attributes[name];
            return attr == null ? null : attr.Value;
        }

        public string Attribute(DomName name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (Attributes == null) {
                Traceables.IgnoredAttributes();
                return null;
            }
            var attr = Attributes[name];
            return attr == null ? null : attr.Value;
        }

        public bool HasClass(string name) {
            var list = Attribute<DomStringTokenList>("class");
            if (list == null) {
                return false;
            }
            var names = DomStringTokenList.Parse(name);
            if (names.Count == 0) {
                return false;
            }
            return names.All(n => list.Contains(n));
        }

        public DomNode ChildNode(int index) {
            return ChildNodes[index];
        }

        public DomNode Empty() {
            DomChildNodes.Clear();
            return this;
        }

        public DomNode RemoveChildNodes() {
            return Empty();
        }

        public bool HasAttribute(string name) {
            return HasAttributes && Attributes.Contains(name);
        }

        public bool HasAttribute(DomName name) {
            return HasAttributes && Attributes.Contains(name);
        }

        public DomNode RemoveAttribute(string name) {
            if (HasAttributes) {
                Attributes.Remove(name);
            }
            return this;
        }

        public DomNode RemoveAttribute(DomName name) {
            if (HasAttributes) {
                Attributes.Remove(name);
            }
            return this;
        }

        public override string ToString() {
            return NodeName;
        }

        public bool HasAttributes {
            get {
                return DomAttributes != null;
            }
        }

        public DomAttributeCollection Attributes {
            get {
                return DomAttributes;
            }
        }

        protected abstract DomAttributeCollection DomAttributes {
            get;
        }

        public DomNodeCollection AncestorNodes {
            get {
                return new DefaultDomNodeCollection(this, e => e._AncestorNodesAndSelf.Skip(1));
            }
        }

        public DomNodeCollection AncestorNodesAndSelf {
            get {
                return new DefaultDomNodeCollection(this, e => e._AncestorNodesAndSelf);
            }
        }

        private IEnumerable<DomNode> _AncestorNodesAndSelf {
            get {
                var item = this;

                while (item != null) {
                    yield return item;
                    item = item.ParentNode;
                }
            }
        }

        public DomNodeCollection DescendantNodes {
            get {
                return new DefaultDomNodeCollection(this, e => e.GetDescendantsNodesCore().Skip(1));
            }
        }

        public DomNodeCollection DescendantNodesAndSelf {
            get {
                return new DefaultDomNodeCollection(this, e => e.GetDescendantsNodesCore());
            }
        }

        [ExpressionSerializationMode(ExpressionSerializationMode.Hidden)]
        public virtual string InnerText {
            get {
                return null;
            }
            set
            {
            }
        }

        [ExpressionSerializationMode(ExpressionSerializationMode.Hidden)]
        public virtual string InnerXml {
            get {
                return null;
            }
            set {
            }
        }

        public sealed override DomNode NextSiblingNode {
            get {
                if (ParentNode == null) {
                    return null;
                }
                return ParentNode.ChildNodes.GetNextSibling(this);
            }
        }

        public DomNode FirstChildNode {
            get {
                return ChildNodes.FirstOrDefault();
            }
        }

        public DomNode LastChildNode {
            get {
                return ChildNodes.LastOrDefault();
            }
        }

        public bool IsLastChildNode {
            get {
                return NextSiblingNode == null;
            }
        }

        public bool IsFirstChildNode {
            get {
                return PreviousSiblingNode == null;
            }
        }

        public virtual string OuterText {
            get {
                return OuterTextWriter.ConvertToString(this);
            }
        }

        public virtual string OuterXml {
            get {
                return DomWriter.GetOuterString(null, this);
            }
            set {
                var frag = OwnerDocument.CreateDocumentFragment();
                frag.LoadXml(value);
                ReplaceWith(frag.ChildNodes);
            }
        }

        public sealed override DomNode PreviousSiblingNode {
            get {
                if (ParentNode == null) {
                    return null;
                }
                return ParentNode.ChildNodes.GetPreviousSibling(this);
            }
        }

        public IEnumerable<DomNode> FollowingNodes {
            get {
                var result = FollowingSiblingNodes.SelectMany(t => t.DescendantNodesAndSelf);
                if (ParentNode != null) {
                    result = result.Concat(ParentNode.FollowingNodes);
                }
                return result;
            }
        }

        public DomNodeCollection FollowingSiblingNodes {
            get {
                return new DefaultDomNodeCollection(this, e => e._FollowingSiblingNodes);
            }
        }

        private IEnumerable<DomNode> _FollowingSiblingNodes {
            get {
                for (var sibling = NextSiblingNode; sibling != null; sibling = sibling.NextSiblingNode) {
                    yield return sibling;
                }
            }
        }

        public DomNodeCollection PrecedingSiblingNodes {
            get {
                return new DefaultDomNodeCollection(this, e => e._PrecedingSiblingNodes);
            }
        }

        private IEnumerable<DomNode> _PrecedingSiblingNodes {
            get {
                if (ParentNode == null) {
                    yield break;
                }
                for (var sibling = ParentNode.FirstChildNode; sibling != this; sibling = sibling.NextSiblingNode) {
                    yield return sibling;
                }
            }
        }

        public IEnumerable<DomNode> PrecedingNodes {
            get {
                var result = Enumerable.Empty<DomNode>();
                if (ParentNode != null) {
                    result = result.Concat(ParentNode.PrecedingNodes);
                    result = result.Concat(new [] { ParentNode });
                }
                result = result.Concat(PrecedingSiblingNodes.SelectMany(t => t.DescendantNodesAndSelf));
                return result;
            }
        }

        [ExpressionSerializationMode(ExpressionSerializationMode.Hidden)]
        public override string TextContent {
            get {
                return null;
            }
            set
            {
            }
        }

        private DomNode RequireParent() {
            if (ParentNode == null) {
                throw DomFailure.ParentNodeRequired();
            }

            return ParentNode;
        }

        private void RequireLinked() {
            if (IsUnlinked) {
                throw DomFailure.ExpectedToBeLinked();
            }
        }

        public DomNode Clone() {
            return CloneCore();
        }

        protected abstract DomNode CloneCore();

        internal static DomNode[] CloneAll(DomNode[] items) {
            return Array.ConvertAll(items, i => i.Clone());
        }

        internal static IEnumerable<DomNode> CloneAll(IEnumerable<DomNode> items) {
            return items.Select(i => i.Clone());
        }

        // One day C# would support covariance here...
        DomNode IDomNodeQuery<DomNode>.Closest(string selector) {
            return Closest(selector);
        }
        DomNode IDomNodeQuery<DomNode>.Closest(DomSelector selector) {
            return Closest(selector);
        }

        public DomContainer Closest(string selector) {
            DomSelector s;
            if (!DomSelector.TryParse(selector, out s)) {
                throw Failure.NotParsable(nameof(selector), typeof(DomSelector));
            }
            return Closest(s);
        }

        public DomContainer Closest(DomSelector selector) {
            if (selector == null) {
                throw new ArgumentNullException(nameof(selector));
            }
            return (DomContainer) ContainerOrSelf.AncestorsAndSelf.FirstOrDefault(t => selector.Matches(t));
        }

        public DomElementQuery QuerySelectorAll(string selector) {
            return Select(selector).Elements;
        }

        public DomElementQuery QuerySelectorAll(DomSelector selector) {
            return Select(selector).Elements;
        }

        public DomElement QuerySelector(string selector) {
            return QuerySelectorAll(selector).FirstOrDefault();
        }

        public DomElement QuerySelector(DomSelector selector) {
            return QuerySelectorAll(selector).FirstOrDefault();
        }

        public DomObjectQuery Select(string selector) {
            var sl = FindProviderFactory().CreateSelector(selector);
            return sl.Select(this);
        }

        public DomObjectQuery Select(DomSelector selector) {
            if (selector == null) {
                throw new ArgumentNullException(nameof(selector));
            }
            return selector.Select(this);
        }

        public DomNode CompressWhitespace() {
            DomNodeVisitor.Visit(this, t => t.MergeAdjacentText());
            DomNodeVisitor.Visit(this, t => t.TryCompressWS());
            return this;
        }

        internal virtual void NotifyParentChanged() {
        }

        private void MergeAdjacentText() {
            if (ChildNodes.Count == 0) {
                return;
            }
            DomNode previous = FirstChildNode;
            for (var node = FirstChildNode.NextSiblingNode; node != null; node = previous.NextSiblingNode) {
                if (previous.IsText && node.IsText) {
                    ((DomCharacterData) previous).AppendData(node.NodeValue);
                    node.RemoveSelf();
                } else {
                    previous = node;
                }
            }
        }

        internal virtual void TryCompressWS() {}

        private IEnumerable<DomNode> GetDescendantsNodesCore() {
            Queue<DomNode> queue = new Queue<DomNode>();
            queue.Enqueue(this);

            while (queue.Count > 0) {
                var result = queue.Dequeue();
                yield return result;

                foreach (var child in result.ChildNodes) {
                    queue.Enqueue(child);
                }
            }
        }
    }
}
