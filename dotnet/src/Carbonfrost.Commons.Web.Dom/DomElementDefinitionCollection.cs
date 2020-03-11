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

namespace Carbonfrost.Commons.Web.Dom {

    public class DomElementDefinitionCollection : IDomNodeDefinitionCollection<DomElementDefinition> {

        private readonly DomContainerDefinition _owner;
        private readonly DomNodeDefinitionCollection<DomElementDefinition> _items;

        protected internal DomElementDefinitionCollection(DomContainerDefinition owner) {
            _owner = owner;
            _items = new DomNodeDefinitionCollection<DomElementDefinition>(s => new DomElementDefinition(s));
        }

        public DomElementDefinition this[string name] {
            get {
                return _items[name];
            }
        }

        public int Count {
            get {
                return _items.Count;
            }
        }

        public bool IsReadOnly {
            get {
                return _items.IsReadOnly;
            }
        }

        internal void MakeReadOnly() {
            _items.MakeReadOnly();
        }

        public void Add(DomElementDefinition item) {
            AddItem(item);
        }

        public DomElementDefinition AddNew(string name) {
            return AddNewItem(name);
        }

        public void Clear() {
            ClearItems();
        }

        public bool Contains(DomElementDefinition item) {
            return _items.Contains(item);
        }

        public bool Contains(string name) {
            return _items.Contains(name);
        }

        public void CopyTo(DomElementDefinition[] array, int arrayIndex) {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<DomElementDefinition> GetEnumerator() {
            return _items.GetEnumerator();
        }

        public bool Remove(DomElementDefinition item) {
            return RemoveItem(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _items.GetEnumerator();
        }

        protected virtual void AddItem(DomElementDefinition item) {
            _items.Add(item);
        }

        protected virtual DomElementDefinition AddNewItem(string name) {
            return _items.AddNew(name);
        }

        protected virtual void ClearItems() {
            _items.Clear();
        }

        protected virtual bool RemoveItem(DomElementDefinition item){
            return _items.Remove(item);
        }
    }
}
