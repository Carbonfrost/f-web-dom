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
using System.Text;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    class DomPathLeaf : DomPath, IReadOnlyList<DomPathExpression> {
        private readonly DomPath _parent;
        private readonly DomPathExpression _expression;

        public DomPathLeaf(DomPath parent, DomPathExpression expression) {
            _parent = parent;
            _expression = expression;
        }

        public DomPathExpression this[int index] {
            get {
                if (index < 0) {
                    throw Failure.IndexOutOfRange(nameof(index), index);
                }
                int count = Count; // cached because it is somewhat expensive
                if (index >= count) {
                    throw Failure.IndexOutOfRange(nameof(index), index);
                }
                if (index == count - 1) {
                    return _expression;
                }
                return Parent.Expressions[index];
            }
        }

        public override DomPath Parent {
            get {
                return _parent;
            }
        }

        public override IReadOnlyList<DomPathExpression> Expressions {
            get {
                return this;
            }
        }

        public int Count {
            get {
                return Parent.Expressions.Count + 1;
            }
        }

        public IEnumerator<DomPathExpression> GetEnumerator() {
            if (_parent != null) {
                foreach (var e in _parent.Expressions) {
                    yield return e;
                }
            }
            yield return _expression;
        }

        public override bool Matches(DomNode node) {
            throw new NotImplementedException();
        }

        public override DomObjectQuery Select(DomNode node) {
            if (node is null) {
                throw new ArgumentNullException(nameof(node));
            }

            var q = node.AsObjectQuery();
            var root = q;
            foreach (var e in this) {
                q = e.Apply(root, q);
            }
            return q;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        internal override void Append(StringBuilder sb) {
            if (Parent != null) {
                Parent.Append(sb);
            }
            sb.Append(_expression);
        }
    }
}
