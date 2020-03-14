//
// Copyright 2013, 2016, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

    public partial class DomObjectQuery : IDomQuery<DomObject, DomObjectQuery> {

        internal static readonly DomObjectQuery _Empty = new DomObjectQuery();

        private readonly DomObject[] _items = Array.Empty<DomObject>();
        private DomNode[] _nodesCache;
        private DomAttribute[] _attributesCache;

        public DomObject this[int index] {
            get {
                return _items[index];
            }
        }

        public DomElementQuery Elements {
            get {
                return new DomElementQuery(_items.OfType<DomElement>());
            }
        }

        public int Count {
            get {
                return _items.Length;
            }
        }

        public DomObjectQuery() {}

        public DomObjectQuery(IEnumerable<DomObject> objs) {
            if (objs == null) {
                throw new ArgumentNullException(nameof(objs));
            }
            if (objs != null) {
                _items = objs.Distinct().ToArray();
            }
        }

        public DomObjectQuery(DomObject item) {
            if (item != null) {
                _items = new [] { item };
            }
        }

        private DomObjectQuery(DomObject[] items) {
            _items = items;
        }

        public DomObjectQuery Add(DomObject item) {
            if (item == null) {
                return this;
            }
            return new DomObjectQuery(AddNonDuplicate(_items, item));
        }

        public DomObjectQuery AddRange(IEnumerable<DomObject> items) {
            if (items == null) {
                return this;
            }
            return new DomObjectQuery(ConcatItemsDistinct(_items, items));
        }

        public DomObjectQuery Concat(DomObjectQuery other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            return new DomObjectQuery(ConcatItemsDistinct(_items, other._items));
        }

        public string Attribute(string name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrEmpty(name)) {
                throw Failure.EmptyString(nameof(name));
            }

            return QueryFirstOrDefault(n => n.Attribute(name));
        }

        public DomObjectQuery Attribute(string name, object value) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrEmpty(name)) {
                throw Failure.EmptyString(nameof(name));
            }
            foreach (var m in Nodes()) {
                m.Attribute(name, value);
            }

            return this;
        }

        public TValue Attribute<TValue>(string name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrEmpty(name)) {
                throw Failure.EmptyString(nameof(name));
            }

            var node = Nodes().FirstOrDefault();
            if (node == null) {
                return default(TValue);
            }

            return node.Attribute<TValue>(name);
        }

        public DomObjectQuery Closest(string selector) {
            DomSelector s;
            if (!DomSelector.TryParse(selector, out s)) {
                throw Failure.NotParsable(nameof(selector), typeof(DomSelector));
            }
            return Closest(s);
        }

        public DomObjectQuery Closest(DomSelector selector) {
            if (selector == null) {
                throw new ArgumentNullException(nameof(selector));
            }
            return new DomObjectQuery(Nodes().Select(e => e.Closest(selector)).NonNull());
        }

        public DomObjectQuery RemoveSelf() {
            return Remove();
        }

        public DomObjectQuery Empty() {
            foreach (var m in Nodes()) {
                m.Empty();
            }
            return this;
        }

        public DomObjectQuery RemoveChildNodes() {
            return Empty();
        }

        public DomObjectQuery Remove() {
            foreach (var m in _items) {
                m.RemoveSelf();
            }
            return _Empty;
        }

        public DomObjectQuery ChildNode(int index) {
            return Query(s => s.ChildNode(index));
        }

        public bool HasAttribute(string name) {
            return QueryFirstOrDefault(n => n.HasAttribute(name));
        }

        public bool HasClass(string name) {
            return QueryFirstOrDefault(n => n.HasAttribute(name));
        }

        public DomObjectQuery Select(string selector) {
            return new DomObjectQuery(
                Nodes().SelectMany(n => n.Select(selector))
            );
        }

        public DomElementQuery QuerySelectorAll(string selector) {
            return new DomElementQuery(Nodes().SelectMany(t => t.QuerySelectorAll(selector)));
        }

        public DomElement QuerySelector(string selector) {
            return Nodes().Select(t => t.QuerySelector(selector)).FirstOrDefault();
        }

        public DomObjectQuery Select(DomSelector selector) {
            return new DomObjectQuery(
                Nodes().SelectMany(n => n.Select(selector))
            );
        }

        public DomElementQuery QuerySelectorAll(DomSelector selector) {
            return new DomElementQuery(Nodes().SelectMany(t => t.QuerySelectorAll(selector)));
        }

        public DomElement QuerySelector(DomSelector selector) {
            return Nodes().Select(t => t.QuerySelector(selector)).FirstOrDefault();
        }

        public IEnumerator<DomObject> GetEnumerator() {
            return ((IEnumerable<DomObject>) _items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private IReadOnlyList<DomNode> Nodes() {
            if (_nodesCache == null) {
                _nodesCache = _items.OfType<DomNode>().ToArray();
            }
            return _nodesCache;
        }

        private IReadOnlyList<DomAttribute> Attributes() {
            if (_attributesCache == null) {
                _attributesCache = _items.OfType<DomAttribute>().ToArray();
            }
            return _attributesCache;
        }

        internal static T[] AddNonDuplicate<T>(T[] items, T item) {
            if (items.Contains(item)) {
                return items;
            }
            int len = items.Length;
            var newItems = new T[len + 1];
            Array.Copy(items, newItems, len);
            newItems[len] = item;
            return newItems;
        }

        internal static T[] ConcatItemsDistinct<T>(T[] items1, IEnumerable<T> items2) {
            if (ReferenceEquals(items1, items2)) {
                return items1;
            }
            return items1.Concat(items2.Except(items1)).ToArray();
        }
    }

}
