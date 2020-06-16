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

        private readonly QueryValues<DomObject> _items;
        private DomNode[] _nodesCache;
        private DomAttribute[] _attributesCache;

        public DomProviderFactory ProviderFactory {
            get {
                return DomProviderFactory;
            }
        }

        protected virtual DomProviderFactory DomProviderFactory {
            get {
                return DomProviderFactory.ForProviderObject(this);
            }
        }

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
                return _items.Count;
            }
        }

        private DomObjectQuery _Empty {
            get {
                return ProviderFactory.EmptyObjectQuery;
            }
        }

        public DomObjectQuery() {
            _items = QueryValues<DomObject>.Empty;
        }

        public DomObjectQuery(IEnumerable<DomObject> objs) {
            _items = QueryValues.Create(objs);
        }

        public DomObjectQuery(DomObject item) {
            _items = QueryValues.Create(item);
        }

        private DomObjectQuery CreateNew(IEnumerable<DomObject> items) {
            return ProviderFactory.CreateObjectQuery(items);
        }

        public DomObjectQuery Add(DomObject item) {
            if (item == null) {
                return this;
            }
            return CreateNew(_items.AddNonDuplicate(item));
        }

        public DomObjectQuery AddRange(IEnumerable<DomObject> items) {
            if (items == null) {
                return this;
            }
            return CreateNew(_items.ConcatItemsDistinct(items));
        }

        public DomObjectQuery Concat(DomObjectQuery other) {
            if (other == null) {
                return this;
            }

            return CreateNew(_items.ConcatItemsDistinct(other._items));
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

        public string Attribute(DomName name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
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

        public DomObjectQuery Attribute(DomName name, object value) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
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

        public TValue Attribute<TValue>(DomName name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            var node = Nodes().FirstOrDefault();
            if (node == null) {
                return default(TValue);
            }
            return node.Attribute<TValue>(name);
        }

        public DomObjectQuery Closest(string selector) {
            DomSelector s = ParseSelector(selector);
            return Closest(s);
        }

        public DomObjectQuery Closest(DomSelector selector) {
            if (selector == null) {
                throw new ArgumentNullException(nameof(selector));
            }
            return CreateNew(Nodes().Select(e => e.Closest(selector)).NonNull());
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

        public bool HasAttribute(DomName name) {
            return QueryFirstOrDefault(n => n.HasAttribute(name));
        }

        public bool HasClass(string name) {
            return QueryFirstOrDefault(n => n.HasAttribute(name));
        }

        public DomObjectQuery Select(string selector) {
            var ds = ParseSelector(selector);
            return CreateNew(
                Nodes().SelectMany(n => n.Select(ds))
            );
        }

        public DomElementQuery QuerySelectorAll(string selector) {
            var ds = ParseSelector(selector);
            return new DomElementQuery(Nodes().SelectMany(t => t.QuerySelectorAll(ds)));
        }

        public DomElement QuerySelector(string selector) {
            var ds = ParseSelector(selector);
            return Nodes().Select(t => t.QuerySelector(ds)).FirstOrDefault();
        }

        public DomObjectQuery Select(DomSelector selector) {
            return CreateNew(
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

        private DomSelector ParseSelector(string selector) {
            return ProviderFactory.CreateSelector(selector);
        }
    }

}
