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

namespace Carbonfrost.Commons.Web.Dom {

    partial class FrugalList<T> {

        internal class SingletonState : FrugalList<T> {

            private readonly T _value;

            public override int Count {
                get {
                    return 1;
                }
            }

            public SingletonState(T value) {
                _value = value;
            }

            public override FrugalList<T> Add(T value) {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }
                if (_value == value) {
                    return this;
                }
                return new ArrayState(_value, value);
            }

            public override FrugalList<T> Clone() {
                if (_value is IDomObjectReferenceLifecycle cl) {
                    return new SingletonState((T) cl.Clone());
                }
                return this;
            }

            public override bool Contains(T value) {
                return _value == value;
            }

            public override IEnumerable<T> GetAll() {
                return new [] { _value };
            }

            public override FrugalList<T> Remove(T value) {
                if (value == _value) {
                    return Empty;
                }
                return this;
            }

            public override FrugalList<T> Remove(Func<T, bool> predicate, Action<T> onRemoved) {
                if (predicate(_value)) {
                    onRemoved(_value);
                    return Empty;
                }
                return this;
            }
        }
    }
}
