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

    class DefaultDomElementCollection : DomElementCollection {

        private readonly DomContainer _owner;
        private readonly Func<DomContainer, IEnumerable<DomElement>> _func;

        public override DomElement this[int index] {
            get {
                var result = Find((i, _) => i == index);
                if (result == null) {
                    throw Failure.IndexOutOfRange(nameof(index), index);
                }
                return result;
            }
        }

        public override DomElement this[string name] {
            get {
                if (string.IsNullOrEmpty(name)) {
                    throw Failure.NullOrEmptyString(nameof(name));
                }
                return Find((_, t) => t.Id == name);
            }
        }

        public override int Count {
            get {
                return All.Count;
            }
        }

        internal DefaultDomElementCollection(DomContainer owner, Func<DomContainer, IEnumerable<DomElement>> func) {
            _owner = owner;
            _func = func;
        }

        public override IEnumerator<DomElement> GetEnumerator() {
            return All.GetEnumerator();
        }

        private IReadOnlyList<DomElement> All {
            get {
                // TODO It would be better to cache this result when we can
                return _func(_owner).ToArray();
            }
        }

        private DomElement Find(Func<int, DomElement, bool> predicate) {
            int index = 0;
            foreach (var f in _func(_owner)) {
                if (predicate(index, f)) {
                    return f;
                }
                index++;
            }
            return null;
        }

    }
}
