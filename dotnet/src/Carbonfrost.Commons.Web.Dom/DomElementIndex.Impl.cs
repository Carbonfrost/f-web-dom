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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomElementIndex<TKey> : DisposableObject, IDomIndex<TKey, DomElement>, IReadOnlyDomIndex<TKey, DomElement> {

        IReadOnlyCollection<DomElement> IDictionary<TKey, IReadOnlyCollection<DomElement>>.this[TKey key] {
            get {
                return this[key];
            }
            set {
                throw Failure.ReadOnlyCollection();
            }
        }

        bool ICollection<KeyValuePair<TKey, IReadOnlyCollection<DomElement>>>.IsReadOnly {
            get {
                return true;
            }
        }

        ICollection<TKey> IDictionary<TKey, IReadOnlyCollection<DomElement>>.Keys {
            get {
                return Keys;
            }
        }

        ICollection<IReadOnlyCollection<DomElement>> IDictionary<TKey, IReadOnlyCollection<DomElement>>.Values {
            get {
                return Values;
            }
        }

        void IDictionary<TKey, IReadOnlyCollection<DomElement>>.Add(TKey key, IReadOnlyCollection<DomElement> value) {
            throw Failure.ReadOnlyCollection();
        }

        void ICollection<KeyValuePair<TKey, IReadOnlyCollection<DomElement>>>.Add(KeyValuePair<TKey, IReadOnlyCollection<DomElement>> item) {
            throw Failure.ReadOnlyCollection();
        }

        void ICollection<KeyValuePair<TKey, IReadOnlyCollection<DomElement>>>.Clear() {
            throw Failure.ReadOnlyCollection();
        }

        bool ICollection<KeyValuePair<TKey, IReadOnlyCollection<DomElement>>>.Contains(KeyValuePair<TKey, IReadOnlyCollection<DomElement>> item) {
            // TODO Implement
            throw new NotImplementedException();
        }

        bool IDictionary<TKey, IReadOnlyCollection<DomElement>>.ContainsKey(TKey key) {
            return Contains(key);
        }

        bool IReadOnlyDictionary<TKey, IReadOnlyCollection<DomElement>>.ContainsKey(TKey key) {
            return Contains(key);
        }

        IEnumerator<KeyValuePair<TKey, IReadOnlyCollection<DomElement>>> IEnumerable<KeyValuePair<TKey, IReadOnlyCollection<DomElement>>>.GetEnumerator() {
            return GetEnumerator();
        }

        bool IDictionary<TKey, IReadOnlyCollection<DomElement>>.Remove(TKey key) {
            throw Failure.ReadOnlyCollection();
        }

        bool ICollection<KeyValuePair<TKey, IReadOnlyCollection<DomElement>>>.Remove(KeyValuePair<TKey, IReadOnlyCollection<DomElement>> item) {
            throw Failure.ReadOnlyCollection();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public Enumerator GetEnumerator() {
            return new Enumerator(this);
        }

    }
}
