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

namespace Carbonfrost.Commons.Web.Dom {

    class DomAttributeCollectionApi : DomAttributeCollection, IDomNodeCollection {
        private readonly DomAttributeCollection _items;
        private readonly DomElement _owner;

        internal DomElement OwnerElement {
            get {
                return _owner;
            }
        }

        public DomAttributeCollectionApi(DomElement owner, DomAttributeCollection items) {
            _owner = owner;
            _items = items;
        }

        public override DomAttribute this[int index] {
            get {
                return _items[index];
            }
            set {
                _items[index].Unlink();
                value.Link(this);

                // Possible that the item will collide by name, in which case
                // it implicitly removes the one with the matching name
                int existing = IndexOf(value.Name);
                if (existing >= 0 && existing != index) {
                    RemoveAt(existing);

                    if (existing < index) {
                        index--;
                    }
                }
                _items[index] = value;
            }
        }

        public override DomAttribute this[string name] {
            get {
                return _items[name];
            }
        }

        public override int Count {
            get {
                return _items.Count;
            }
        }

        public override void Add(DomAttribute item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }

            item.Link(this);
            _items.Add(item);
        }

        public override void Clear() {
            this.UnlinkAll(_items);
            _items.Clear();
        }

        public override bool Contains(DomAttribute item) {
            return FastContains(item) && _items.Contains(item);
        }

        public override IEnumerator<DomAttribute> GetEnumerator() {
            return _items.GetEnumerator();
        }

        public override int IndexOf(DomAttribute item) {
            return FastContains(item)
                ? _items.IndexOf(item)
                : -1;
        }

        public override int IndexOf(string name) {
            return _items.IndexOf(name);
        }

        public override void Insert(int index, DomAttribute item) {
            _items.Insert(index, item);
        }

        public override bool Remove(DomAttribute item) {
            if (item == null) {
                throw new ArgumentNullException(nameof(item));
            }
            item.Unlink();
            return _items.Remove(item);
        }

        public override void RemoveAt(int index) {
            CheckIndex(index);
            var item = this[index];
            item.Unlink();
            _items.RemoveAt(index);
        }

        bool IDomNodeCollection.Remove(DomObject node) {
            var attr = node as DomAttribute;
            if (attr != null && _items.Remove(attr)) {
                return true;
            }
            return false;
        }

        int IDomNodeCollection.GetSiblingIndex(DomObject node) {
            var dn = node as DomAttribute;
            if (dn != null) {
                return IndexOf(dn);
            }
            return -1;
        }

        DomNode IDomNodeCollection.OwnerNode {
            get {
                return OwnerElement;
            }
        }

        private bool FastContains(DomAttribute item) {
            return item.SiblingAttributes == this;
        }
    }
}

