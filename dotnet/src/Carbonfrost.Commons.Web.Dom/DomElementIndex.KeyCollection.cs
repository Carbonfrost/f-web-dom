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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomElementIndex<TKey> {

        public struct KeyCollection : IReadOnlyCollection<TKey>, ICollection<TKey> {

            private readonly DomElementIndex<TKey> _parent;

            public KeyCollection(DomElementIndex<TKey> parent) {
                _parent = parent;
            }

            public int Count {
                get {
                    return _parent._items.Keys.Count;
                }
            }

            bool ICollection<TKey>.IsReadOnly {
                get {
                    return true;
                }
            }

            public IEnumerator<TKey> GetEnumerator() {
                return _parent._items.Keys.GetEnumerator();
            }

            void ICollection<TKey>.Add(TKey item) {
                throw Failure.ReadOnlyCollection();
            }

            void ICollection<TKey>.Clear() {
                throw Failure.ReadOnlyCollection();
            }

            public bool Contains(TKey item) {
                return _parent._items.Keys.Contains(item);
            }

            void ICollection<TKey>.CopyTo(TKey[] array, int arrayIndex) {
                _parent._items.Keys.CopyTo(array, arrayIndex);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            bool ICollection<TKey>.Remove(TKey item) {
                throw Failure.ReadOnlyCollection();
            }
        }
    }
}
