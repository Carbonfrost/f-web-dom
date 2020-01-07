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
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    internal class Buffer<TElement> : IEnumerable<TElement>, IReadOnlyList<TElement> {

        private List<TElement> cache;
        private readonly IEnumerable<TElement> source;
        private IEnumerator<TElement> current;

        public Buffer(IEnumerable<TElement> e) {
            this.source = e;
            this.current = source.GetEnumerator();
            this.cache = new List<TElement>();
        }

        public TElement this[int index] {
            get {
                Ensure(index);
                return cache[index];
            }
        }

        public int Count {
            get {
                while (MoveNext()) {
                }

                return cache.Count;
            }
        }

        public IEnumerator<TElement> GetEnumerator() {
            return new Enumerator(this);
        }

        private bool MoveNext() {
            bool any;
            try {
                any = current.MoveNext();

            } catch (InvalidOperationException) {
                // Clear cache on concurrent modification
                this.cache.Clear();
                this.current = source.GetEnumerator();
                any = current.MoveNext();
            }

            if (any) {
                this.cache.Add(this.current.Current);
                return true;
            }

            return false;
        }

        private void Ensure(int index) {
            // Invoke MoveNext() first so that we always check whether the
            // underlying enumerator was concurrently modified (even if this
            // causes more items to be cached than necessary)
            while (MoveNext() && this.Count <= index) {
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        private struct Enumerator : IEnumerator<TElement> {

            private readonly Buffer<TElement> source;
            private int index;

            public Enumerator(Buffer<TElement> source) {
                this.source = source;
                this.index = -1;
            }

            public TElement Current {
                get {
                    if (this.index < 0 || this.index >= this.source.cache.Count)
                        throw Failure.OutsideEnumeration();

                    return this.source.cache[this.index];
                }
            }

            object System.Collections.IEnumerator.Current {
                get {
                    return this.Current;
                }
            }

            public void Dispose() {}

            public bool MoveNext() {
                this.index++;

                if (this.source.cache.Count == this.index) {
                    return this.source.MoveNext();
                }

                return true;
            }

            public void Reset() {
                throw new NotSupportedException();
            }

        }
    }
}
