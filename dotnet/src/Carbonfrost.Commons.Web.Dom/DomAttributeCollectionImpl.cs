//
// Copyright 2013, 2016, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

    class DomAttributeCollectionImpl : DomAttributeCollection {

        internal static readonly DomAttributeCollection ReadOnly = new DomAttributeCollectionImpl(
            false
        );

        private readonly IDictionary<DomName, DomAttribute> _map;
        private readonly IList<DomAttribute> _items;
        private readonly IEqualityComparer<DomName> _comparer;

        private IList<DomAttribute> Items {
            get {
                return _items;
            }
        }

        private DomAttributeCollectionImpl(bool dummy) {
            _items = new ReadOnlyCollection<DomAttribute>(Array.Empty<DomAttribute>());
            _map = new ReadOnlyDictionary<DomName, DomAttribute>(new Dictionary<DomName, DomAttribute>());
        }

        internal DomAttributeCollectionImpl(IList<DomAttribute> items, IEqualityComparer<DomName> comparer) {
            if (items == null) {
                throw new ArgumentNullException(nameof(items));
            }
            _items = items;
            _map = new Dictionary<DomName, DomAttribute>(comparer);
            foreach (var a in items) {
                _map.Add(a.Name, a);
            }
            _comparer = comparer;
        }

        internal override IEqualityComparer<DomName> _Comparer {
            get {
                return _comparer;
            }
        }

        public override bool IsReadOnly {
            get {
                return Items.IsReadOnly;
            }
        }

        public override DomAttribute this[DomName name] {
            get {
                return GetByName(name);
            }
        }

        private DomAttribute GetByName(DomName name) {
            DomAttribute result;
            if (TryGetValue(RequireName(name), out result)) {
                return result;
            } else {
                return null;
            }
        }

        private bool TryGetValue(DomName name, out DomAttribute result) {
            return _map.TryGetValue(name, out result);
        }

        public override bool Contains(string name) {
            return IndexOf(name) >= 0;
        }

        public override int IndexOf(DomName name) {
            RequireName(name);
            for (int i = 0; i < Count; i++) {
                if (_comparer.Equals(this[i].Name, name)) {
                    return i;
                }
            }
            return -1;
        }

        // TODO Comparer should be by name

        private void InsertItem(int index, DomAttribute item) {
            int existing = IndexOf(item.Name);

            if (existing < 0) {

            }
            else if (Items[existing] == item) {
                return;
            }
            else {
                throw DomFailure.AttributeWithGivenNameExists(item.Name, nameof(item));
            }

            _map.Add(item.Name, item);
            Items.Insert(index, item);
        }

        private void ClearItems() {
            _map.Clear();
            _items.Clear();
        }

        private void RemoveItem(int index) {
            var m = Items[index];
            _map.Remove(m.Name);
            _items.RemoveAt(index);
        }

        private void SetItem(int index, DomAttribute item) {
            var old = Items[index];
            _map.Remove(Items[index].Name);
            _map.Add(item.Name, item);
            _items[index] = item;
        }

        public override int IndexOf(DomAttribute item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }
            return Items.IndexOf(item);
        }

        public override void Insert(int index, DomAttribute item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            InsertItem(index, item);
        }

        public override void RemoveAt(int index) {
            if (index < 0 || index >= Count) {
                throw Failure.IndexOutOfRange(nameof(index), index, 0, Count - 1);
            }

            RemoveItem(index);
        }

        public override DomAttribute this[int index] {
            get {
                return _items[index];
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                SetItem(index, value);
            }
        }

        public override void Add(DomAttribute item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            InsertItem(Count, item);
        }

        public override void Clear() {
            ClearItems();
        }

        public override bool Contains(DomAttribute item) {
            return IndexOf(item) >= 0;
        }

        public override bool Remove(DomAttribute item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            int index = IndexOf(item);
            bool bounds = index >= 0;
            if (bounds) {
                RemoveItem(index);
            }

            return bounds;
        }

        public override int Count {
            get {
                return _items.Count;
            }
        }

        public override IEnumerator<DomAttribute> GetEnumerator() {
            return _items.GetEnumerator();
        }

        internal DomAttributeCollectionImpl SetComparer(DomNameComparer comparer, Action<DomAttribute> onRejected) {
            if (_comparer == comparer) {
                return this;
            }

            var newItems = _items.DistinctWithEvents(
                DomNameComparer.AttributeAdapter(comparer), onRejected
            );
            return new DomAttributeCollectionImpl(newItems, comparer);
        }
    }

}
