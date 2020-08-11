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

namespace Carbonfrost.Commons.Web.Dom {

    partial class FrugalList<T> {

        internal sealed class ArrayState : FrugalList<T> {

            private T[] _values;
            private int _count;

            public override int Count {
                get {
                    return _count;
                }
            }

            internal bool Compacted {
                get {
                    return _count == _values.Length;
                }
            }

            internal int Capacity {
                get {
                    return _values.Length;
                }
            }

            public ArrayState(T one, T two) {
                _values = new [] { one, two };
                _count = 2;
            }

            private ArrayState(T[] compacted) {
                _values = compacted;
                _count = compacted.Length;
            }

            public override bool Contains(T value) {
                return _values.Contains(value);
            }

            public override FrugalList<T> Clone() {
                if (_values.OfType<IDomObjectReferenceLifecycle>().Any()) {
                    return new ArrayState(CloneSlow().ToArray());
                }
                return this;
            }

            private IEnumerable<T> CloneSlow() {
                foreach (var o in _values) {
                    if (o == null) {
                        continue;
                    }
                    else if (o is IDomObjectReferenceLifecycle lc) {
                        yield return (T) lc.Clone();
                    }
                    else {
                        yield return o;
                    }
                }
            }

            public override FrugalList<T> Add(T value) {
                // Search for an empty space
                int exists = Array.IndexOf(_values, value);
                if (exists >= 0) {
                    return this;
                }

                int emptyIndex;
                if (Compacted) {
                    EnsureCapacity();
                    emptyIndex = Count;
                } else {
                    emptyIndex = Array.IndexOf(_values, null);
                    if (emptyIndex < 0) {
                        throw new NotImplementedException();
                    }
                }

                _count++;
                _values[emptyIndex] = value;
                return this;
            }

            public override FrugalList<T> Remove(T value) {
                for (int i = 0; i < _values.Length; i++) {
                    if (ReferenceEquals(_values[i], value)) {
                        _values[i] = null;
                        _count--;
                        break;
                    }
                }
                return this;
            }

            public override IEnumerable<T> GetAll() {
                if (Compacted) {
                    return _values;
                }
                return GetAllSlow();
            }

            private IEnumerable<T> GetAllSlow() {
                foreach (var o in _values) {
                    if (o == null) {
                        continue;
                    }
                    else {
                        yield return o;
                    }
                }
            }

            public override FrugalList<T> Remove(Func<T, bool> predicate, Action<T> onRemoved) {
                for (int i = 0; i < _values.Length; i++) {
                    var val = _values[i];
                    if (val == null) {
                        continue;
                    }
                    if (predicate(val)) {
                        onRemoved(val);
                        _values[i] = null;
                    }
                }
                return this;
            }

            private void EnsureCapacity() {
                if (_count == _values.Length) {
                    Array.Resize(ref _values, 2 * _values.Length);
                }
            }
        }
    }
}
