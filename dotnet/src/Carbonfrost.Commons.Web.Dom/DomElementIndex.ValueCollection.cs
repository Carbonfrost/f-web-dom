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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomElementIndex<TKey> {

        public struct ValueCollection : IReadOnlyCollection<IReadOnlyCollection<DomElement>>, ICollection<IReadOnlyCollection<DomElement>> {

            private readonly IReadOnlyCollection<IReadOnlyCollection<DomElement>> _inner;

            public ValueCollection(DomElementIndex<TKey> parent) {
                _inner = parent._items.Groupings;
            }

            public int Count {
                get {
                    return _inner.Count;
                }
            }

            bool ICollection<IReadOnlyCollection<DomElement>>.IsReadOnly {
                get {
                    return true;
                }
            }

            public IEnumerator<IReadOnlyCollection<DomElement>> GetEnumerator() {
                return _inner.GetEnumerator();
            }

            void ICollection<IReadOnlyCollection<DomElement>>.Add(IReadOnlyCollection<DomElement> item) {
                throw Failure.ReadOnlyCollection();
            }

            void ICollection<IReadOnlyCollection<DomElement>>.Clear() {
                throw Failure.ReadOnlyCollection();
            }

            public bool Contains(IReadOnlyCollection<DomElement> item) {
                return _inner.Contains(item);
            }

            public void CopyTo(IReadOnlyCollection<DomElement>[] array, int arrayIndex) {
                _inner.ToList().CopyTo(array, arrayIndex);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            bool ICollection<IReadOnlyCollection<DomElement>>.Remove(IReadOnlyCollection<DomElement> item) {
                throw Failure.ReadOnlyCollection();
            }
        }

    }
}
