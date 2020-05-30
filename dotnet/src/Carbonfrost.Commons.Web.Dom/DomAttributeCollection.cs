//
// Copyright 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomAttributeCollection : IList<DomAttribute>, IReadOnlyList<DomAttribute> {

        public abstract DomAttribute this[int index] {
            get;
            set;
        }

        public abstract DomAttribute this[string name] {
            get;
        }

        public abstract int Count { get; }
        public virtual bool IsReadOnly {
            get {
                return false;
            }
        }

        public virtual void InsertRange(int index, IEnumerable<DomAttribute> items) {
            if (items == null) {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var element in items.ToList()) {
                Insert(index++, element);
            }
        }

        public virtual void AddRange(IEnumerable<DomAttribute> items) {
            if (items == null) {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var element in items.ToList()) {
                Add(element);
            }
        }

        public abstract void Add(DomAttribute item);
        public abstract void Clear();
        public abstract bool Contains(DomAttribute item);

        public void CopyTo(DomAttribute[] array, int arrayIndex) {
            Utility.CopyToArray(this, array, arrayIndex);
        }

        public abstract IEnumerator<DomAttribute> GetEnumerator();
        public abstract int IndexOf(DomAttribute item);
        public abstract void Insert(int index, DomAttribute item);
        public abstract bool Remove(DomAttribute item);
        public abstract void RemoveAt(int index);

        public virtual int IndexOf(string name) {
            RequireName(name);

            for (int i = 0; i < Count; i++) {
                if (this[i].Name == name) {
                    return i;
                }
            }
            return -1;
        }

        public bool Remove(string name) {
            var attr = IndexOf(name);
            if (attr < 0) {
                return false;
            }
            RemoveAt(attr);
            return true;
        }

        public virtual bool Contains(string name) {
            return IndexOf(name) >= 0;
        }

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


        internal static string RequireName(string name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (name.Length == 0) {
                throw Failure.EmptyString(nameof(name));
            }

            return name;
        }
    }

}
