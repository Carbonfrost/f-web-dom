//
// Copyright 2013, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomNodeCollection : IList<DomNode>, IReadOnlyList<DomNode>, IDomNodeCollection {

        internal static readonly DomNodeCollection Empty = new EmptyDomNodeCollectionImpl();

        private readonly DomContainer _owner;

        public virtual bool IsReadOnly {
            get {
                return false;
            }
        }

        internal DomContainer OwnerNode {
            get { return _owner; }
        }

        internal DomNodeCollection(DomContainer ownerNode) {
            _owner = ownerNode;
        }

        internal virtual DomNode GetNextSibling(DomNode other) {
            int index = IndexOf(other);
            if (index < 0 || index == Count - 1) {
                return null;
            }
            return this[index + 1];
        }

        internal virtual DomNode GetPreviousSibling(DomNode other) {
            int index = IndexOf(other);
            if (index <= 0) {
                return null;
            }
            return this[index - 1];
        }

        public virtual void InsertRange(int index, IEnumerable<DomNode> items) {
            if (items == null) {
                throw new ArgumentNullException("items");
            }

            foreach (var element in items) {
                Insert(index++, element);
            }
        }

        public virtual void AddRange(IEnumerable<DomNode> items) {
            if (items == null) {
                throw new ArgumentNullException("items");
            }

            foreach (var element in items) {
                Add(element);
            }
        }

        DomNode IDomNodeCollection.OwnerNode {
            get {
                return OwnerNode;
            }
        }

        int IDomNodeCollection.GetSiblingIndex(DomObject node) {
            var dn = node as DomNode;
            if (dn != null) {
                return IndexOf(dn);
            }
            return -1;
        }

        void IDomNodeCollection.UnsafeRemove(DomObject node) {
            var dn = node as DomNode;
            if (dn != null) {
                RemoveCore(dn);
            }
        }

        void IDomNodeCollection.UnsafeAdd(DomObject node) {
            var dn = node as DomNode;
            if (dn != null) {
                InsertCore(Count, dn);
            }
        }

        bool IDomNodeCollection.Remove(DomObject node) {
            var dn = node as DomNode;
            return dn != null && Remove(dn);
        }

        private void CheckIndex(int index) {
            if (index < 0) {
                throw Failure.IndexOutOfRange("index", index);
            }
            if (index >= Count) {
                throw Failure.IndexOutOfRange("index", index);
            }
        }

        public bool Contains(DomNode item) {
            return item.Siblings == this;
        }

        public abstract int IndexOf(DomNode item);

        public DomNode this[int index] {
            get {
                CheckIndex(index);
                return GetItemCore(index);
            }
            set {
                CheckIndex(index);
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
                if (Contains(value)) {
                    int existing = value.NodePosition;
                    if (existing == index) {
                        return;
                    }

                    // Unsafe remove because we are changing its position
                    RemoveCore(value);
                    if (existing < index) {
                        SetItemCore(--index, value);
                    } else {
                        SetItemCore(index, value);
                    }
                    return;
                }
                SetItem(index, value);
            }
        }

        public abstract int Count {
            get;
        }

        public void Add(DomNode item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            Insert(Count, item);
        }

        public void Insert(int index, DomNode item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            if (Contains(item)) {
                // Unsafe remove because we are changing its position
                RemoveCore(item);
                index--;
            }
            if (item.IsDocumentFragment) {
                InsertRange(index, item.ChildNodes.ToList());
            } else {
                Entering(item, null);
                InsertCore(index, item);
            }
        }

        internal abstract DomNode GetItemCore(int index);
        internal abstract void InsertCore(int index, DomNode item);
        internal abstract bool RemoveCore(DomNode item);
        internal abstract void ClearCore();
        internal abstract void RemoveAtCore(int index);
        internal abstract void SetItemCore(int index, DomNode item);

        public bool Remove(DomNode item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }

            if (RemoveCore(item)) {
                item.Unlinked();
                return true;
            }

            return false;
        }

        public void Clear() {
            ClearItems();
        }

        public void CopyTo(DomNode[] array, int arrayIndex) {
            if (array == null) {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0 || arrayIndex >= array.Length) {
                throw Failure.IndexOutOfRange("arrayIndex", arrayIndex);
            }
            if (arrayIndex + Count > array.Length) {
                throw Failure.NotEnoughSpaceInArray("array", array);
            }
            foreach (var e in this) {
                array[arrayIndex] = e;
                arrayIndex++;
            }
        }

        public void RemoveAt(int index) {
            CheckIndex(index);
            this[index].Unlinked();
            RemoveAtCore(index);
        }

        public abstract IEnumerator<DomNode> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private void ClearItems() {
            ClearCore();
            foreach (var m in this) {
                m.Unlinked();
            }
        }

        private void SetItem(int index, DomNode item) {
            var old = this[index];
            old.Unlinked();
            Entering(item, old);
            SetItemCore(index, item);
        }

        private void Entering(DomNode item, DomNode willReplace) {
            item.SetSiblingNodes(this);
            _owner.AssertCanAppend(item, willReplace);
        }
    }

}
