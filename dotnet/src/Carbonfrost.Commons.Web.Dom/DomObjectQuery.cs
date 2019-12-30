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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomObjectQuery : IDomNodeQuery<DomObjectQuery>, IEnumerable<DomNode> {

        internal static readonly DomObjectQuery _Empty = new DomObjectQuery();

        private readonly List<DomNode> _nodes = new List<DomNode>();

        public DomNode this[int index] {
            get {
                return _nodes[index];
            }
        }

        public DomObjectQuery() {}

        public DomObjectQuery(IEnumerable<DomNode> nodes) {
            if (nodes != null) {
                _nodes.AddRange(nodes);
            }
        }

        public DomObjectQuery(DomNode node) {
            if (node != null) {
                _nodes.Add(node);
            }
        }

        public DomObjectQuery Add(DomNode node) {
            if (node == null) {
                return this;
            }
            var nodes = new List<DomNode>(_nodes.Count + 1);
            nodes.AddRange(_nodes);
            nodes.Add(node);
            return new DomObjectQuery(nodes);
        }

        public DomObjectQuery Concat(DomObjectQuery other) {
            if (other == null) {
                throw new ArgumentNullException("other");
            }

            return this;
        }

        public string Attribute(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrEmpty(name)) {
                throw Failure.EmptyString("name");
            }

            var node = this.FirstOrDefault();
            if (node == null) {
                return null;
            }
            else {
                return node.Attribute(name);
            }
        }

        public DomObjectQuery Attribute(string name, object value) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrEmpty(name)) {
                throw Failure.EmptyString("name");
            }
            foreach (var m in _nodes) {
                m.Attribute(name, value);
            }

            return this;
        }

        public DomObjectQuery Closest(string selector) {
            DomSelector s;
            if (!DomSelector.TryParse(selector, out s)) {
                throw Failure.NotParsable("selector", typeof(DomSelector));
            }
            return Closest(s);
        }

        public DomObjectQuery Closest(DomSelector selector) {
            if (selector == null) {
                throw new ArgumentNullException("selector");
            }
            return new DomObjectQuery(this.Select(e => e.Closest(selector)).NonNull());
        }

        public DomObjectQuery RemoveSelf() {
            return Remove();
        }

        public DomObjectQuery Empty() {
            return Remove();
        }

        public DomObjectQuery Remove() {
            foreach (var m in _nodes) {
                m.RemoveSelf();
            }
            return _Empty;
        }

        public DomObjectQuery ChildNode(int index) {
            throw new NotImplementedException();
        }

        public bool HasAttribute(string name) {
            throw new NotImplementedException();
        }

        public bool HasClass(string name) {
            throw new NotImplementedException();
        }

        public DomObjectQuery QuerySelectorAll(string selector) {
            return new DomObjectQuery(_nodes.SelectMany(t => t.QuerySelectorAll(selector)));
        }

        public DomNode QuerySelector(string selector) {
            return _nodes.Select(t => t.QuerySelector(selector)).FirstOrDefault();
        }

        public IEnumerator<DomNode> GetEnumerator() {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }

}
