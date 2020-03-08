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

        private Buffer<DomElement> _cache;
        private readonly DomContainer _owner;
        private readonly Func<DomContainer, IEnumerable<DomElement>> _func;

        public override DomElement this[int index] {
            get {
                return EnsureCache()[index];
            }
        }

        public override DomElement this[string name] {
            get {
                if (string.IsNullOrEmpty(name)) {
                    throw Failure.NullOrEmptyString(nameof(name));
                }

                return EnsureCache().FirstOrDefault(t => t.Id == name);
            }
        }

        public override int Count {
            get {
                return EnsureCache().Count;
            }
        }

        internal DefaultDomElementCollection(DomContainer owner, Func<DomContainer, IEnumerable<DomElement>> func) {
            // TODO Actually synchronize on owner
            _owner = owner;
            _func = func;
        }

        public override IEnumerator<DomElement> GetEnumerator() {
            return EnsureCache().GetEnumerator();
        }

        private Buffer<DomElement> EnsureCache() {
            if (_cache == null) {
                _cache = new Buffer<DomElement>(_func(_owner));
            }
            return _cache;
        }

    }
}
