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

    public abstract class DomElementCollection : IReadOnlyList<DomElement> {

        public static readonly DomElementCollection Empty = new EmptyImpl();

        public abstract DomElement this[int index] {
            get;
        }

        public virtual DomElement this[string name] {
            get {
                if (string.IsNullOrEmpty(name)) {
                    throw Failure.NullOrEmptyString(nameof(name));
                }

                return this.FirstOrDefault(t => t.Id == name);
            }
        }

        public abstract int Count {
            get;
        }

        protected DomElementCollection() {}

        public abstract IEnumerator<DomElement> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        class EmptyImpl : DomElementCollection {
            public override DomElement this[string name] {
                get {
                    return null;
                }
            }

            public override DomElement this[int index] {
                get {
                    throw Failure.IndexOutOfRange(nameof(index), index);
                }
            }

            public override int Count {
                get {
                    return 0;
                }
            }

            public override  IEnumerator<DomElement> GetEnumerator() {
                return Enumerable.Empty<DomElement>().GetEnumerator();
            }
        }
    }
}
