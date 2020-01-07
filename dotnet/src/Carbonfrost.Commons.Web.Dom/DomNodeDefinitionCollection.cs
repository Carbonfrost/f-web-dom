//
// - DomNodeDefinitionCollection.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Collections.Generic;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomNodeDefinitionCollection<T> : ICollection<T>
        where T : DomNodeDefinition {

        private readonly Dictionary<string, T> items = new Dictionary<string, T>();

        public T this[string name] {
            get {
                return items.GetValueOrDefault(name);
            }
        }

        public int Count {
            get {
                return items.Count;
            }
        }

        bool ICollection<T>.IsReadOnly {
            get {
                return false;
            }
        }

        public void Add(T item) {
            items.Add(item.Name, item);
        }

        public void Clear() {
            items.Clear();
        }

        public bool Contains(T item) {
            if (item == null)
                throw new ArgumentNullException("item");

            T actual;
            return items.TryGetValue(item.Name, out actual)
                && actual == item;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            this.items.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            if (item == null)
                throw new ArgumentNullException("item");

            if (Contains(item)) {
                return items.Remove(item.Name);
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator() {
            return items.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
