//
// Copyright 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomObjectQuery : IDomNodeManipulation<DomObjectQuery> {

        private DomObjectQuery Select(Func<DomNode, DomNode> f) {
            return new DomObjectQuery(Enumerable.Select(this, f).NonNull());
        }

        private DomObjectQuery SelectMany(Func<DomNode, IEnumerable<DomNode>> f) {
            return new DomObjectQuery(Enumerable.SelectMany(this, f).NonNull());
        }

        private DomObjectQuery Each(Action<DomNode> f) {
            foreach (var n in this) {
                f(n);
            }
            return this;
        }

        public DomObjectQuery Unwrap() {
            return Select(m => m.Unwrap());
        }

        public DomObjectQuery RemoveAttributes() {
            return Select(m => m.RemoveAttributes());
        }

        public DomObjectQuery RemoveAttribute(string name) {
            return Select(m => m.RemoveAttribute(name));
        }

        public DomObjectQuery AddClass(string className) {
            return Select(m => m.AddClass(className));
        }

        public DomObjectQuery RemoveClass(string className) {
            return Select(m => m.RemoveClass(className));
        }

        public DomObjectQuery SetName(string name) {
            return new DomObjectQuery(SetNameInternal(name));
        }

        public DomObjectQuery Wrap(string element) {
            return Select(m => m.Wrap(element));
        }

        public DomObjectQuery Wrap(DomNode newParent) {
            return Select(m => m.Wrap(newParent));
        }

        public DomObjectQuery After(DomNode nextSibling) {
            throw new NotImplementedException();
        }

        public DomObjectQuery After(params DomNode[] nextSiblings) {
            throw new NotImplementedException();
        }

        public DomObjectQuery After(string text) {
            throw new NotImplementedException();
        }

        public DomObjectQuery InsertAfter(DomNode other) {
            throw new NotImplementedException();
        }

        public DomObjectQuery Before(DomNode previousSibling) {
            throw new NotImplementedException();

        }
        public DomObjectQuery Before(params DomNode[] previousSiblings) {
            throw new NotImplementedException();
        }

        public DomObjectQuery Before(string text) {
            throw new NotImplementedException();
        }

        public DomObjectQuery InsertBefore(DomNode other) {
            throw new NotImplementedException();
        }

        public DomWriter Append() {
            throw new NotImplementedException();
        }

        public DomObjectQuery Append(DomNode child) {
            throw new NotImplementedException();
        }

        public DomObjectQuery Append(string text) {
            throw new NotImplementedException();
        }

        public DomObjectQuery AppendTo(DomNode parent) {
            throw new NotImplementedException();
        }

        public DomWriter Prepend() {
            throw new NotImplementedException();
        }

        public DomObjectQuery Prepend(DomNode child) {
            throw new NotImplementedException();
        }

        public DomObjectQuery Prepend(string text) {
            throw new NotImplementedException();
        }

        public DomObjectQuery PrependTo(DomNode parent) {
            throw new NotImplementedException();
        }

        public DomObjectQuery ReplaceWith(DomNode other) {
            throw new NotImplementedException();
        }

        public DomObjectQuery ReplaceWith(params DomNode[] others) {
            throw new NotImplementedException();
        }

        public DomObjectQuery ReplaceWith(string text) {
            throw new NotImplementedException();
        }

        private IEnumerable<DomNode> SetNameInternal(string name) {
            foreach (var m in this) {
                if (m.IsAttribute || m.IsElement) {
                    yield return m.SetName(name);
                } else {
                    yield return m;
                }
            }
        }

        public DomObjectQuery AncestorNodes() {
            return SelectMany(n => n.AncestorNodes);
        }

        public DomObjectQuery AncestorNodesAndSelf() {
            return SelectMany(n => n.AncestorNodesAndSelf);
        }

        public DomObjectQuery DescendantNodes() {
            return SelectMany(n => n.DescendantNodes);
        }

        public DomObjectQuery DescendantNodesAndSelf() {
            return SelectMany(n => n.DescendantNodesAndSelf);
        }

        public DomObjectQuery FirstChildNode() {
            return Select(n => n.FirstChildNode);
        }

        public DomObjectQuery FollowingNodes() {
            return SelectMany(n => n.FollowingNodes);
        }

        public DomObjectQuery FollowingSiblingNodes() {
            return SelectMany(n => n.FollowingSiblingNodes);
        }

        public DomObjectQuery LastChildNode() {
            return Select(n => n.LastChildNode);
        }

        public DomObjectQuery NextSiblingNode() {
            return Select(n => n.NextSiblingNode);
        }

        public DomObjectQuery ParentNode() {
            return Select(n => n.ParentNode);
        }

        public DomObjectQuery PrecedingNodes() {
            return SelectMany(n => n.PrecedingNodes);
        }

        public DomObjectQuery PrecedingSiblingNodes() {
            return SelectMany(n => n.PrecedingSiblingNodes);
        }

        public DomObjectQuery PreviousSiblingNode() {
            return Select(n => n.PreviousSiblingNode);
        }

        // These methods provide the operations that match IDomNodeAppendApiConventions

        public DomObjectQuery AppendElement(string name) {
            return Each(s => s.AppendElement(name));
        }

        public DomObjectQuery AppendAttribute(string name, object value) {
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

        public DomObjectQuery PrependAttribute(string name, object value) {
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
    }

}
