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

using System.Collections.Generic;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    sealed class ListDomNodeCollection : DomNodeCollection {

        private readonly List<DomNode> _items = new List<DomNode>();

        public override int Count {
            get {
                return _items.Count;
            }
        }

        public override DomNode this[int index] {
            get {
                return _items[index];
            }
            set {
                _items[index] = value;
            }
        }

        internal override DomNode GetNextSibling(DomNode other) {
            int index = IndexOf(other);
            if (index < 0 || index == Count - 1) {
                return null;
            }
            return this[index + 1];
        }

        internal override DomNode GetPreviousSibling(DomNode other) {
            int index = IndexOf(other);
            if (index <= 0) {
                return null;
            }
            return this[index - 1];
        }

        public override IEnumerator<DomNode> GetEnumerator() {
            return new Enumerator(_items.GetEnumerator());
        }

        public override int IndexOf(DomNode node) {
            return _items.IndexOf(node);
        }

        public override void Add(DomNode item) {
            _items.Add(item);
        }

        public override void Clear() {
            _items.Clear();
        }

        public override bool Contains(DomNode item) {
            return _items.Contains(item);
        }

        public override void Insert(int index, DomNode item) {
            _items.Insert(index, item);
        }

        public override bool Remove(DomNode item) {
            return _items.Remove(item);
        }

        public override void RemoveAt(int index) {
            _items.RemoveAt(index);
        }

        public struct Enumerator : IEnumerator<DomNode> {

            private readonly IEnumerator<DomNode> _e;
            private bool _moved;

            internal Enumerator(IEnumerator<DomNode> e) {
                _e = e;
                _moved = false;
            }

            public DomNode Current {
                get {
                    if (!_moved) {
                        throw Failure.OutsideEnumeration();
                    }
                    return _e.Current;
                }
            }

            object System.Collections.IEnumerator.Current {
                get {
                    return Current;
                }
            }

            public bool MoveNext() {
                _moved = true;
                return _e.MoveNext();
            }

            void System.Collections.IEnumerator.Reset() {
                _moved = false;
                _e.Reset();
            }

            public void Dispose() {
            }
        }
    }
}
