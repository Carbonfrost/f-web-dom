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

    // Provides a multi-map that is optimized for a small number of values.
    class FrugalMultimap<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
        where TValue : class
    {

        private readonly Dictionary<TKey, FrugalList<TValue>> _items;

        public int Count {
            get {
                return _items.Values.Sum(s => s.Count);
            }
        }

        public IReadOnlyCollection<TValue> this[TKey key] {
            get {
                return _items.GetValueOrDefault(key);
            }
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> All {
            get {
                return _items.SelectMany(
                    s => s.Value.Select(t => new KeyValuePair<TKey, TValue>(s.Key, t))
                );
            }
        }

        public ICollection<TKey> Keys {
            get {
                return _items.Keys;
            }
        }

        public IReadOnlyCollection<KeyValuePair<TKey, FrugalList<TValue>>> Items {
            get {
                return _items;
            }
        }

        public IEnumerable<object> Values {
            get {
                return _items.Values.SelectMany(s => s);
            }
        }

        public IReadOnlyCollection<FrugalList<TValue>> Groupings {
            get {
                return _items.Values;
            }
        }

        public FrugalMultimap(IEqualityComparer<TKey> keyComparer) {
            _items = new Dictionary<TKey, FrugalList<TValue>>(keyComparer);
        }

        public void Add(TKey key, TValue value) {
            var list = _items.GetValueOrDefault(key, FrugalList<TValue>.Empty);
            _items[key] = list.Add(value);
        }

        public bool Contains(TKey key) {
            return _items.ContainsKey(key);
        }

        public void Clear() {
            _items.Clear();
        }

        public bool Remove(TKey key, TValue value) {
            FrugalList<TValue> list;
            if (_items.TryGetValue(key, out list) && list.TryRemove(value, out list)) {
                if (list.Count == 0) {
                    _items.Remove(key);
                } else {
                    _items[key] = list;
                }
                return true;
            }
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return All.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
