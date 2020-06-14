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

using System.Collections.Generic;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    sealed class EmptyDomNodeCollectionImpl : DomNodeCollection {

        public override int Count {
            get {
                return 0;
            }
        }

        public override bool IsReadOnly {
            get {
                return true;
            }
        }

        public override DomNode this[int index] {
            get {
                return GetItemCore(index);
            }
            set {
                SetItemCore(index, value);
            }
        }

        public override void Add(DomNode item) {
            InsertCore(Count, item);
        }

        public override void Clear() {
            ClearCore();
        }

        public override bool Contains(DomNode item) {
            return false;
        }

        public override void Insert(int index, DomNode item) {
            InsertCore(index, item);
        }

        public override bool Remove(DomNode item) {
            return RemoveCore(item);
        }

        public override void RemoveAt(int index) {
            RemoveAtCore(index);
        }

        internal override DomNode GetNextSibling(DomNode other) {
            return null;
        }

        internal override DomNode GetPreviousSibling(DomNode other) {
            return null;
        }

        public override IEnumerator<DomNode> GetEnumerator() {
            yield break;
        }

        public override int IndexOf(DomNode node) {
            return -1;
        }

        private DomNode GetItemCore(int index) {
            CheckIndex(index);
            return null;
        }

        private void InsertCore(int index, DomNode item) {
            throw Failure.ReadOnlyCollection();
        }

        private void ClearCore() {
            throw Failure.ReadOnlyCollection();
        }

        private void RemoveAtCore(int index) {
            throw Failure.ReadOnlyCollection();
        }

        private bool RemoveCore(DomNode node) {
            throw Failure.ReadOnlyCollection();
        }

        private void SetItemCore(int index, DomNode item) {
            throw Failure.ReadOnlyCollection();
        }
    }
}
