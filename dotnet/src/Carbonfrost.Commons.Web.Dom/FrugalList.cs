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

namespace Carbonfrost.Commons.Web.Dom {

    // Provides a list that is designed to hold a relatively small number of items
    // and optimizes for memory usage.  The mutation methods either modify the current
    // list in palce OR return a copy with modifications.  Clients should deal with the
    // return value for subsequent modifications.
    //
    // Order of items is not guaranteed to be stable
    abstract partial class FrugalList<T> : IReadOnlyCollection<T>
        where T : class
    {

        internal static readonly FrugalList<T> Empty = new EmptyState();

        private FrugalList<T> _state {
            get {
                return this;
            }
        }

        public FrugalList<T> Clear() {
            return Empty;
        }

        public IEnumerator<T> GetEnumerator() {
            return _state.GetAll().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public abstract int Count {
            get;
        }

        public abstract FrugalList<T> Add(T value);
        public abstract FrugalList<T> Remove(T value);
        public abstract bool Contains(T value);
        public abstract FrugalList<T> Clone();
        public abstract FrugalList<T> Remove(Func<T, bool> predicate, Action<T> onRemoved);
        public abstract IEnumerable<T> GetAll();

        public bool TryRemove(T value, out FrugalList<T> result) {
            int count = Count;
            result = Remove(value);
            return result.Count < count;
        }

        public IEnumerable<TDerived> OfType<TDerived>() where TDerived : class {
            return GetAll().OfType<TDerived>();
        }

        public IEnumerable<T> OfType(Type type) {
            return GetAll().Where(o => type.IsInstanceOfType(o));
        }

        public FrugalList<T> RemoveOfType(Type type, Action<object> onRemoved) {
            return Remove(a => type.IsInstanceOfType(a), onRemoved);
        }
    }
}
