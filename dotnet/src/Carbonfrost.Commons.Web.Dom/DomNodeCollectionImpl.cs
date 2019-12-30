//
// Copyright 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Collections.ObjectModel;
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    sealed class DomNodeCollectionImpl : DomNodeCollection {

        private readonly List<DomNode> _items;

        public DomNodeCollectionImpl(DomContainer ownerNode) : base(ownerNode) {
            _items = new List<DomNode>();
        }

        public override int Count {
            get {
                return _items.Count;
            }
        }

        public override IEnumerator<DomNode> GetEnumerator() {
            return new Enumerator(_items.GetEnumerator());
        }

        public override int IndexOf(DomNode node) {
            return _items.IndexOf(node);
        }

        internal override DomNode GetItemCore(int index) {
            return _items[index];
        }

        internal override void InsertCore(int index, DomNode item) {
            _items.Insert(index, item);
        }

        internal override void ClearCore() {
            _items.Clear();
        }

        internal override void RemoveAtCore(int index) {
            _items.RemoveAt(index);
        }

        internal override bool RemoveCore(DomNode node) {
            int index = _items.IndexOf(node);
            if (index < 0) {
                return false;
            }
            _items.RemoveAt(index);
            return true;
        }

        internal override void SetItemCore(int index, DomNode item) {
            _items[index] = item;
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
