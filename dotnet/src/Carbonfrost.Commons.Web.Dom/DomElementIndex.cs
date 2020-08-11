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
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract partial class DomElementIndex<TKey> : DisposableObject, IDomIndex<TKey, DomElement>, IReadOnlyDomIndex<TKey, DomElement> {

        private readonly FrugalMultimap<TKey, DomElement> _items;

        public bool IsConnected {
            get;
            private set;
        }

        internal FrugalMultimap<TKey, DomElement> Items {
            get {
                return _items;
            }
        }

        public IReadOnlyCollection<DomElement> this[TKey key] {
            get {
                TryGetValue(key, out var result);
                return result;
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, IReadOnlyCollection<DomElement>>.Keys {
            get {
                return Keys;
            }
        }

        public KeyCollection Keys {
            get {
                ThrowIfDisposed();
                return new KeyCollection(this);
            }
        }

        public ValueCollection Values {
            get {
                ThrowIfDisposed();
                return new ValueCollection(this);
            }
        }

        IEnumerable<IReadOnlyCollection<DomElement>> IReadOnlyDictionary<TKey, IReadOnlyCollection<DomElement>>.Values {
            get {
                return Values;
            }
        }

        public int Count {
            get {
                ThrowIfDisposed();
                return _items.Count;
            }
        }

        protected DomElementIndex(IEqualityComparer<TKey> keyComparer) {
            _items = new FrugalMultimap<TKey, DomElement>(keyComparer);
            IsConnected = true;
        }

        public void Disconnect() {
            DisconnectCore();
            IsConnected = false;
        }

        protected abstract void DisconnectCore();

        protected override void Dispose(bool manualDispose) {
            Disconnect();
            base.Dispose(manualDispose);
        }

        public override string ToString() {
            return string.Join(", ", _items);
        }

        public bool Contains(TKey key) {
            ThrowIfDisposed();
            return Items.Contains(key);
        }

        public void CopyTo(KeyValuePair<TKey, IReadOnlyCollection<DomElement>>[] array, int arrayIndex) {
            ThrowIfDisposed();
            Items.Items.Select(
                s => new KeyValuePair<TKey, IReadOnlyCollection<DomElement>>(s.Key, s.Value)
            ).ToList().CopyTo(array, arrayIndex);
        }

        public bool TryGetValue(TKey key, out IReadOnlyCollection<DomElement> value) {
            ThrowIfDisposed();
            value = Items[key];
            return value != null;
        }
    }
}
