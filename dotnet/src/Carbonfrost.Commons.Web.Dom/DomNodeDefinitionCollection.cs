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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    class DomNodeDefinitionCollection<T> : IDomNodeDefinitionCollection<T> where T : DomNodeDefinition {

        private readonly Dictionary<DomName, T> _items = new Dictionary<DomName, T>();
        private readonly Func<DomName, T> _ctor;

        public DomNodeDefinitionCollection(Func<DomName, T> ctor) {
            _ctor = ctor;
        }

        public T this[DomName name] {
            get {
                return _items.GetValueOrDefault(name);
            }
        }

        public T this[string name] {
            get {
                return this[DomName.Create(name)];
            }
        }

        public int Count {
            get {
                return _items.Count;
            }
        }

        public bool IsReadOnly {
            get;
            private set;
        }

        public void Add(T item) {
            ThrowIfReadOnly();
            _items.Add(item.Name, item);
        }

        public T AddNew(string name) {
            return AddNew(DomName.Create(name));
        }

        public T AddNew(DomName name) {
            ThrowIfReadOnly();
            var result = _ctor(name);
            Add(result);
            return result;
        }

        public void Clear() {
            ThrowIfReadOnly();
            _items.Clear();
        }

        public bool Contains(T item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            T actual;
            return _items.TryGetValue(item.Name, out actual) && actual == item;
        }

        public bool Contains(string name) {
            return Contains(DomName.Create(name));
        }

        public bool Contains(DomName name) {
            return _items.ContainsKey(name);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _items.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            if (Contains(item)) {
                return _items.Remove(item.Name);
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator() {
            return _items.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        internal void MakeReadOnly() {
            IsReadOnly = true;
            foreach (var item in _items.Values) {
                item.MakeReadOnly();
            }
        }

        private void ThrowIfReadOnly() {
            if (IsReadOnly) {
                throw Failure.Sealed();
            }
        }
    }
}
