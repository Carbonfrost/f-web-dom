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

namespace Carbonfrost.Commons.Web.Dom {

    // API layer which validates connection between nodes
    class DomNodeCollectionApi : DomNodeCollection, IDomNodeCollection {
        private readonly DomContainer _owner;

        // strictly provides storage and is unsafe for maintaining relationships between nodes
        private readonly DomNodeCollection _items;

        internal DomContainer OwnerNode {
            get {
                return _owner;
            }
        }

        internal DomNodeCollection UnsafeItems {
            get {
                return _items;
            }
        }

        internal DomNodeCollectionApi(DomContainer ownerNode, DomNodeCollection items) {
            _owner = ownerNode;
            _items = items;
        }

        DomNode IDomNodeCollection.OwnerNode {
            get {
                return OwnerNode;
            }
        }

        public override DomNode this[int index] {
            get {
                CheckIndex(index);
                return _items[index];
            }
            set {
                CheckIndex(index);
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }
                if (Contains(value)) {
                    int existing = value.NodePosition;
                    if (existing == index) {
                        return;
                    }

                    // Unsafe remove because we are changing its position
                    _items.Remove(value);
                    if (existing < index) {
                        _items[--index] = value;
                    } else {
                        _items[index] = value;
                    }
                    return;
                }
                SetItem(index, value);
            }
        }

        public override int Count {
            get {
                return _items.Count;
            }
        }

        internal override DomNode GetNextSibling(DomNode other) {
            return _items.GetNextSibling(other);
        }

        internal override DomNode GetPreviousSibling(DomNode other) {
            return _items.GetPreviousSibling(other);
        }

        public override void Add(DomNode item) {
            Insert(Count, item);
        }

        public override void Clear() {
            // HACK Linked list needs to maintain the connection to the
            // unsafe items list so it has to clear before unlinking
            bool shouldClearBeforeUnlinking = _items is LinkedDomNodeList;
            var allItems = _items.ToArray();

            if (shouldClearBeforeUnlinking) {
                var unlink = _items.ToArray();
                _items.Clear();
                this.UnlinkAll(unlink);
            } else {
                this.UnlinkAll(_items);
                _items.Clear();
            }
            _owner.OwnerDocumentOrSelf.ChildNodesChanged(DomMutation.Remove, _owner, allItems, null, null);
        }

        public override bool Contains(DomNode item) {
            return item._Siblings == this;
        }

        public override IEnumerator<DomNode> GetEnumerator() {
            return _items.GetEnumerator();
        }

        public override int IndexOf(DomNode item) {
            return _items.IndexOf(item);
        }

        public override void Insert(int index, DomNode item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }
            int currentIndex = IndexOf(item);
            if (currentIndex == index) {
                return;
            }

            if (currentIndex >= 0) {
                _items.Remove(item);
                index = Math.Max(0, index - 1);
            }
            if (item.IsDocumentFragment) {
                InsertRange(index, item.ChildNodes);
            } else {
                Entering(item, null);
                _items.Insert(index, item);
                _owner.ChildNodeChanged(DomMutation.Add, item, GetPreviousSibling(item), GetNextSibling(item));
            }
        }

        public override bool Remove(DomNode item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            var prev = GetPreviousSibling(item);
            var next = GetNextSibling(item);
            if (_items.Remove(item)) {
                item.Unlink();
                _owner.ChildNodeChanged(DomMutation.Remove, item, prev, next);
                return true;
            }

            return false;
        }

        public override void RemoveAt(int index) {
            CheckIndex(index);
            this[index].Unlink();

            var prev = _items.ElementAtOrDefault(index - 1);
            var next = _items.ElementAtOrDefault(index + 1);
            var item = _items[index];
            _items.RemoveAt(index);
            _owner.ChildNodeChanged(DomMutation.Remove, item, prev, next);
        }

        bool IDomNodeCollection.Remove(DomObject node) {
            var dn = node as DomNode;
            return dn != null && Remove(dn);
        }

        int IDomNodeCollection.GetSiblingIndex(DomObject node) {
            var dn = node as DomNode;
            if (dn != null) {
                return IndexOf(dn);
            }
            return -1;
        }

        public override void InsertRange(int index, IEnumerable<DomNode> items) {
            if (items == null || !items.Any()) {
                return;
            }

            using (_owner.NewMutationBatch()) {
                base.InsertRange(index, items);
            }
        }

        public override void AddRange(IEnumerable<DomNode> items) {
            if (items == null || !items.Any()) {
                return;
            }

            using (_owner.NewMutationBatch()) {
                base.AddRange(items);
            }
        }

        private void Entering(DomNode item, DomNode willReplace) {
            item.Link(this);
            _owner.AssertCanAppend(item, willReplace);
        }

        private void SetItem(int index, DomNode item) {
            var old = this[index];
            old.Unlink();
            Entering(item, old);
            _items[index] = item;
        }
    }
}
