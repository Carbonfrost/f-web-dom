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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomElementQuery : IDomQuery<DomElement, DomElementQuery> {

        internal static readonly DomElementQuery _Empty = new DomElementQuery();
        private readonly DomElement[] _items = Array.Empty<DomElement>();

        public int Count {
            get {
                return _items.Length;
            }
        }

        public DomElement this[int index] {
            get {
                return _items[index];
            }
        }

        public DomElementQuery() {}

        public DomElementQuery(IEnumerable<DomElement> objs) {
            if (objs != null) {
                _items = objs.Distinct().ToArray();
            }
        }

        public DomElementQuery(DomElement item) {
            if (item != null) {
                _items = new [] { item };
            }
        }

        public DomElementQuery Add(DomElement item) {
            if (item == null) {
                return this;
            }
            return new DomElementQuery(DomObjectQuery.AddNonDuplicate(_items, item));
        }

        public DomElementQuery AddRange(IEnumerable<DomElement> items) {
            if (items == null) {
                return this;
            }
            return new DomElementQuery(DomObjectQuery.ConcatItemsDistinct(_items, items));
        }

        public DomElementQuery Concat(DomElementQuery other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return new DomElementQuery(DomObjectQuery.ConcatItemsDistinct(_items, other._items));
        }

        public string Attribute(string name) {
            return QueryFirstOrDefault(e => e.Attribute(name));
        }

        public TValue Attribute<TValue>(string name) {
            return QueryFirstOrDefault(e => e.Attribute<TValue>(name));
        }

        public DomElementQuery Attribute(string name, object value) {
            return Each(e => e.Attribute(name, value));
        }

        public DomObjectQuery ChildNode(int index) {
            return Query(e => e.ChildNode(index));
        }

        public DomElementQuery Closest(string selector) {
            return Closest(ParseSelector(selector));
        }

        public DomElementQuery Closest(DomSelector selector) {
            return Query(e => e.Closest(selector) as DomElement);
        }

        public IEnumerator<DomElement> GetEnumerator() {
            return ((IEnumerable<DomElement>) _items).GetEnumerator();
        }

        public bool HasAttribute(string name) {
            return QueryFirstOrDefault(e => e.HasAttribute(name));
        }

        public bool HasClass(string name) {
            return QueryFirstOrDefault(e => e.HasClass(name));
        }

        public DomElement QuerySelector(string selector) {
            return _items.FirstNonNull(e => e.QuerySelector(selector));
        }

        public DomElementQuery QuerySelectorAll(string selector) {
            return QueryMany(e => e.QuerySelectorAll(selector));
        }

        public DomElementQuery RemoveAttribute(string name) {
            Each(e => e.RemoveAttribute(name));
            return this;
        }

        public DomObjectQuery Select(string selector) {
            return Select(ParseSelector(selector));
        }

        public DomObjectQuery Select(DomSelector selector) {
            return new DomObjectQuery(
                _items.SelectMany(n => n.Select(selector))
            );
        }

        public DomElementQuery QuerySelectorAll(DomSelector selector) {
            return new DomElementQuery(_items.SelectMany(t => t.QuerySelectorAll(selector)));
        }

        public DomElement QuerySelector(DomSelector selector) {
            return _items.Select(t => t.QuerySelector(selector)).FirstOrDefault();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private DomSelector ParseSelector(string selector) {
            // TODO Should be provider-specific
            return DomSelector.Parse(selector);
        }
    }
}
