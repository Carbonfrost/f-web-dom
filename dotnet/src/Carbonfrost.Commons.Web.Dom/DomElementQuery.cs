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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomElementQuery : IDomQuery<DomElement, DomElementQuery> {

        private readonly QueryValues<DomElement> _items;

        public DomProviderFactory ProviderFactory {
            get {
                return DomProviderFactory;
            }
        }

        protected virtual DomProviderFactory DomProviderFactory {
            get {
                return DomProviderFactory.Default;
            }
        }

        public int Count {
            get {
                return _items.Count;
            }
        }

        public DomElement this[int index] {
            get {
                return _items[index];
            }
        }

        private DomElementQuery _Empty {
            get {
                return ProviderFactory.EmptyElementQuery;
            }
        }

        public DomElementQuery() {
            _items = QueryValues<DomElement>.Empty;
        }

        public DomElementQuery(IEnumerable<DomElement> items) {
            _items = QueryValues.Create(items);
        }

        public DomElementQuery(DomElement item) {
            _items = QueryValues.Create(item);
        }

        private DomElementQuery CreateNew(IEnumerable<DomElement> items) {
            return ProviderFactory.CreateElementQuery(items);
        }

        public DomElementQuery Add(DomElement item) {
            if (item == null) {
                return this;
            }
            return CreateNew(_items.AddNonDuplicate(item));
        }

        public DomElementQuery AddRange(IEnumerable<DomElement> items) {
            if (items == null) {
                return this;
            }
            return CreateNew(_items.ConcatItemsDistinct(items));
        }

        public DomElementQuery Concat(DomElementQuery other) {
            if (other == null) {
                return this;
            }
            return CreateNew(_items.ConcatItemsDistinct(other._items));
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

        public string Attribute(DomName name) {
            return QueryFirstOrDefault(e => e.Attribute(name));
        }

        public TValue Attribute<TValue>(DomName name) {
            return QueryFirstOrDefault(e => e.Attribute<TValue>(name));
        }

        public DomElementQuery Attribute(DomName name, object value) {
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

        public bool HasAttribute(DomName name) {
            return QueryFirstOrDefault(e => e.HasAttribute(name));
        }

        public bool HasClass(string name) {
            return QueryFirstOrDefault(e => e.HasClass(name));
        }

        public DomElement QuerySelector(string selector) {
            var s = ParseSelector(selector);
            return _items.FirstNonNull(e => e.QuerySelector(s));
        }

        public DomElementQuery QuerySelectorAll(string selector) {
            var s = ParseSelector(selector);
            return QueryMany(e => e.QuerySelectorAll(s));
        }

        public DomElementQuery RemoveAttribute(string name) {
            Each(e => e.RemoveAttribute(name));
            return this;
        }

        public DomElementQuery RemoveAttribute(DomName name) {
            Each(e => e.RemoveAttribute(name));
            return this;
        }

        public DomObjectQuery Select(string selector) {
            return Select(ParseSelector(selector));
        }

        public DomObjectQuery Select(DomSelector selector) {
            return ProviderFactory.CreateObjectQuery(
                _items.SelectMany(n => n.Select(selector))
            );
        }

        public DomElementQuery QuerySelectorAll(DomSelector selector) {
            return CreateNew(_items.SelectMany(t => t.QuerySelectorAll(selector)));
        }

        public DomElement QuerySelector(DomSelector selector) {
            return _items.Select(t => t.QuerySelector(selector)).FirstOrDefault();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private DomSelector ParseSelector(string selector) {
            return ProviderFactory.CreateSelector(selector);
        }
    }
}
