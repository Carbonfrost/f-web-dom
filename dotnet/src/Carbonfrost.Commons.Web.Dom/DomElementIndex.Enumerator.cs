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

    public abstract partial class DomElementIndex<TKey> {

        public struct Enumerator : IEnumerator<Grouping>, IEnumerator<IReadOnlyCollection<DomElement>>, IEnumerator<KeyValuePair<TKey, IReadOnlyCollection<DomElement>>> {

            private readonly IEnumerator<Grouping> _inner;

            public Grouping Current {
                get {
                    return _inner.Current;
                }
            }

            object IEnumerator.Current {
                get {
                    return Current;
                }
            }

            IReadOnlyCollection<DomElement> IEnumerator<IReadOnlyCollection<DomElement>>.Current {
                get {
                    return Current;
                }
            }

            KeyValuePair<TKey, IReadOnlyCollection<DomElement>> IEnumerator<KeyValuePair<TKey, IReadOnlyCollection<DomElement>>>.Current {
                get {
                    return Current.AsKeyValuePair();
                }
            }

            internal Enumerator(DomElementIndex<TKey> parent) {
                _inner = parent._items.Items.Select(g => new Grouping(g.Key, g.Value)).GetEnumerator();
            }

            public void Dispose() {
                _inner.Dispose();
            }

            public bool MoveNext() {
                return _inner.MoveNext();
            }

            public void Reset() {
                _inner.Reset();
            }
        }
    }
}
