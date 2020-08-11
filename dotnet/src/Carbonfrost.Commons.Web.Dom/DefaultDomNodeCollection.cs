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

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    class DefaultDomNodeCollection : DomNodeCollection {

        private readonly DomNode _owner;
        private readonly Func<DomNode, IEnumerable<DomNode>> _func;

        public override DomNode this[int index] {
            get {
                var result = Find((i, _) => i == index);
                if (result == null) {
                    throw Failure.IndexOutOfRange(nameof(index), index);
                }
                return result;
            }
            set {
                throw Failure.ReadOnlyCollection();
            }
        }

        public override int Count {
            get {
                return All.Count;
            }
        }

        internal DefaultDomNodeCollection(DomNode owner, Func<DomNode, IEnumerable<DomNode>> func) {
            _owner = owner;
            _func = func;
        }

        public override IEnumerator<DomNode> GetEnumerator() {
            return All.GetEnumerator();
        }

        private IReadOnlyList<DomNode> All {
            get {
                // TODO It would be better to cache this result when we can
                return _func(_owner).ToArray();
            }
        }

        private DomNode Find(Func<int, DomNode, bool> predicate) {
            int index = 0;
            foreach (var f in _func(_owner)) {
                if (predicate(index, f)) {
                    return f;
                }
                index++;
            }
            return null;
        }

        public override void Add(DomNode item) {
            throw Failure.ReadOnlyCollection();
        }

        public override void Clear() {
            throw Failure.ReadOnlyCollection();
        }

        public override bool Contains(DomNode item) {
            return IndexOf(item) >= 0;
        }

        public override int IndexOf(DomNode item) {
            int index = 0;
            foreach (var e in this) {
                if (item == e) {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public override void Insert(int index, DomNode item) {
            throw Failure.ReadOnlyCollection();
        }

        public override bool Remove(DomNode item) {
            throw Failure.ReadOnlyCollection();
        }

        public override void RemoveAt(int index) {
            throw Failure.ReadOnlyCollection();
        }

        internal override DomNode GetNextSibling(DomNode other) {
            throw new NotSupportedException();
        }

        internal override DomNode GetPreviousSibling(DomNode other) {
            throw new NotSupportedException();
        }
    }
}
