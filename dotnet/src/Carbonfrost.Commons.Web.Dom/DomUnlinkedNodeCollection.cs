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
using System.Collections.Generic;
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    sealed class DomUnlinkedNodeCollection : IDomUnlinkedNodeCollection {

        readonly List<DomObject> _list = new List<DomObject>(16);
        readonly DomDocument _ownerDocument;

        public DomUnlinkedNodeCollection(DomDocument domDocument) {
            this._ownerDocument = domDocument;
        }

        private void Entering(DomObject item) {
            item.SetSiblingNodes(this);
        }

        private void Leaving(DomObject item) {
            item.SetSiblingNodes((DomNodeCollection) null);
        }

        public int GetSiblingIndex(DomObject item) {
            return -1;
        }

        public void UnsafeRemove(DomObject item) {
            _list.Remove(item);
        }

        public void UnsafeAdd(DomObject item) {
            _list.Add(item);
        }

        public void Add(DomObject item) {
            Entering(item);
            _list.Add(item);
        }

        public bool Remove(DomObject item) {
            bool result = _list.Remove(item);
            if (result) {
                Leaving(item);
            }

            return result;
        }

        public bool Contains(DomObject item) {
            return _list.Contains(item);
        }

        public DomNode OwnerNode {
            get {
                return _ownerDocument;
            }
        }
    }

}
