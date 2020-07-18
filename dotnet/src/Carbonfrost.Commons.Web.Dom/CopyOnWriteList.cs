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

    class CopyOnWriteList<T> : IList<T> {

        private BackingList _backing;

        public T this[int index] {
            get {
                return ReadOperation[index];
            }
            set {
                WriteOperation[index] = value;
            }
        }

        public int Count {
            get {
                return ReadOperation.Count;
            }
        }

        public bool IsReadOnly {
            get {
                return false;
            }
        }

        private BackingList ReadOperation {
            get {
                if (_backing == null) {
                    return BackingList.Empty;
                }

                return _backing;
            }
        }

        private BackingList WriteOperation {
            get {
                if (_backing == null) {
                    _backing = new BackingList();
                } else {
                    lock (((ICollection) _backing).SyncRoot) {
                        _backing = _backing.CloneForWriteIfNecessary();
                    }
                }
                return _backing;
            }
        }

        public CopyOnWriteList() {
        }

        private CopyOnWriteList(CopyOnWriteList<T> other) {
            _backing = other._backing;
            if (_backing != null) {
                lock (((ICollection) _backing).SyncRoot) {
                    _backing.AddRef();
                }
            }
        }

        public CopyOnWriteList<T> Clone() {
            return new CopyOnWriteList<T>(this);
        }

        public bool HasSameBacking(CopyOnWriteList<T> other) {
            return ReferenceEquals(other._backing, _backing);
        }

        public void Add(T item) {
            WriteOperation.Add(item);
        }

        public void Clear() {
            WriteOperation.Clear();
        }

        public bool Contains(T item) {
            return ReadOperation.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            ReadOperation.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator() {
            if (_backing == null) {
                return Enumerable.Empty<T>().GetEnumerator();
            }
            return new Enumerator(_backing);
        }

        public int IndexOf(T item) {
            return ReadOperation.IndexOf(item);
        }

        public void Insert(int index, T item) {
            WriteOperation.Insert(index, item);
        }

        public bool Remove(T item) {
            return WriteOperation.Remove(item);
        }

        public void RemoveAt(int index) {
            WriteOperation.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        struct Enumerator : IEnumerator<T> {
            private readonly BackingList _backing;
            private IEnumerator<T> _inner;

            public Enumerator(BackingList backing) {
                _backing = backing;
                _inner = _backing.GetEnumerator();
                _backing.AddRef();
            }

            public T Current {
                get {
                    return _inner.Current;
                }
            }

            object IEnumerator.Current {
                get {
                    return Current;
                }
            }

            public void Dispose() {
                // TODO This could decrement references on the backing store
                // to prevent additional copies (performance)
                _inner.Dispose();
            }

            public bool MoveNext() {
                return _inner.MoveNext();
            }

            public void Reset() {
                _inner.Reset();
            }
        }

        class BackingList : List<T> {

            internal static readonly BackingList Empty = new BackingList();

            private int _refCount = 1;

            public BackingList() {
            }

            public BackingList(IEnumerable<T> collection) : base(collection) {
            }

            public bool HasNoClones {
                get {
                    return _refCount == 1;
                }
            }

            internal BackingList CloneForWriteIfNecessary() {
                if (!HasNoClones) {
                    _refCount--;
                    return new BackingList(this);
                }
                return this;
            }

            public void AddRef() {
                _refCount++;
            }
        }
    }
}
