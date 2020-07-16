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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomNodeCollection : IList<DomNode>, IReadOnlyList<DomNode> {

        internal static readonly DomNodeCollection Empty = new EmptyDomNodeCollectionImpl();

        public abstract DomNode this[int index] {
            get;
            set;
        }

        public abstract int Count {
            get;
        }

        public virtual bool IsReadOnly {
            get {
                return false;
            }
        }

        internal static DomNodeCollection Create(IEnumerable<DomNode> nodes) {
            if (nodes == null) {
                return Empty;
            }
            var result = new ListDomNodeCollection();
            result.AddRange(nodes);
            return result;
        }

        internal abstract DomNode GetNextSibling(DomNode other);

        internal abstract DomNode GetPreviousSibling(DomNode other);

        public virtual void InsertRange(int index, IEnumerable<DomNode> items) {
            if (items == null) {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var element in items.ToList()) {
                Insert(index++, element);
            }
        }

        public virtual void AddRange(IEnumerable<DomNode> items) {
            if (items == null) {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var element in items.ToList()) {
                Add(element);
            }
        }

        public abstract void Add(DomNode item);
        public abstract void Clear();
        public abstract bool Contains(DomNode item);

        public void CopyTo(DomNode[] array, int arrayIndex) {
            Utility.CopyToArray(this, array, arrayIndex);
        }

        public abstract IEnumerator<DomNode> GetEnumerator();
        public abstract int IndexOf(DomNode item);
        public abstract void Insert(int index, DomNode item);
        public abstract bool Remove(DomNode item);
        public abstract void RemoveAt(int index);

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        internal void CheckIndex(int index) {
            if (index < 0) {
                throw Failure.IndexOutOfRange(nameof(index), index);
            }
            if (index >= Count) {
                throw Failure.IndexOutOfRange(nameof(index), index);
            }
        }

    }
}
