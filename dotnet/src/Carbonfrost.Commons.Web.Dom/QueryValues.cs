//
// Copyright 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

namespace Carbonfrost.Commons.Web.Dom {

    static class QueryValues {

        public static QueryValues<T> Create<T>(T item) {
            if (item == null) {
                return QueryValues<T>.Empty;
            }
            return Optimized(new [] { item });
        }

        public static QueryValues<T> Create<T>(IEnumerable<T> items) {
            if (items == null) {
                return QueryValues<T>.Empty;
            }
            if (items is QueryValues<T> qv) {
                return qv;
            }
            return Optimized(items.Distinct().ToArray());
        }

        public static QueryValues<T> Optimized<T>(T[] items) {
            return new QueryValues<T>(items);
        }
    }

    class QueryValues<T> : IReadOnlyList<T> {
         // An immutable list for use by DomObjectQuery and DomElementQuery

        private readonly T[] _items;

        internal static readonly QueryValues<T> Empty = new QueryValues<T>();

        public T this[int index] {
            get {
                return _items[index];
            }
        }

        public int Count {
            get {
                return _items.Length;
            }
        }

        public QueryValues() {
            _items = Array.Empty<T>();
        }

        public QueryValues(T[] items) {
            _items = items;
        }

        public QueryValues<T> AddNonDuplicate(T item) {
            if (_items.Contains(item)) {
                return this;
            }
            int len = _items.Length;
            var newItems = new T[len + 1];
            Array.Copy(_items, newItems, len);
            newItems[len] = item;
            return QueryValues.Optimized(newItems);
        }

        public QueryValues<T> ConcatItemsDistinct(IEnumerable<T> items2) {
            if (ReferenceEquals(_items, items2) || ReferenceEquals(this, items2)) {
                return this;
            }
            return QueryValues.Optimized(_items.Concat(items2.Except(_items)).ToArray());
        }

        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>) _items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

}
