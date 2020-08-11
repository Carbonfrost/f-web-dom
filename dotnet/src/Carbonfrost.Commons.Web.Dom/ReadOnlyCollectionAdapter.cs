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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    struct ReadOnlyCollectionAdapter<TInput, TOutput> : ICollection<TOutput>, IReadOnlyCollection<TOutput> {
        private readonly IReadOnlyCollection<TInput> _inner;

        public ReadOnlyCollectionAdapter(IReadOnlyCollection<TInput> inner) {
            _inner = inner;
        }

        private IEnumerable<TOutput> Output {
            get {
                return _inner.Cast<TOutput>();
            }
        }

        public int Count {
            get {
                return _inner.Count;
            }
        }

        bool ICollection<TOutput>.IsReadOnly {
            get {
                return false;
            }
        }

        public IEnumerator<TOutput> GetEnumerator() {
            return Output.GetEnumerator();
        }

        void ICollection<TOutput>.Add(TOutput item) {
            throw Failure.ReadOnlyCollection();
        }

        void ICollection<TOutput>.Clear() {
            throw Failure.ReadOnlyCollection();
        }

        bool ICollection<TOutput>.Contains(TOutput item) {
            return Output.Contains(item);
        }

        void ICollection<TOutput>.CopyTo(TOutput[] array, int arrayIndex) {
            Output.ToList().CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        bool ICollection<TOutput>.Remove(TOutput item) {
            throw Failure.ReadOnlyCollection();
        }
    }
}
