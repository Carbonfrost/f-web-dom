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
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomDocument {

        public DomElementIndex<TKey> CreateAttributeTokenIndex<TKey>(DomName name, Func<string, TKey> func = null, IEqualityComparer<TKey> keyComparer = null) {
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }

            if (func is null) {
                func = s => Activation.FromText<TKey>(s);
            }

            keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;

            return new ElementsByTokenAttributeIndex<TKey>(this, name, func, keyComparer);
        }

        public DomElementIndex<string> CreateAttributeTokenIndex(DomName name) {
            return CreateAttributeTokenIndex(name, s => s ?? string.Empty, null);
        }

        public DomElementIndex<string> CreateAttributeTokenIndex(DomName name, IEqualityComparer<string> keyComparer) {
            return CreateAttributeTokenIndex(name, s => s, keyComparer);
        }

        class ElementsByTokenAttributeIndex<TKey> : ElementsByAttributeIndex<TKey> {

            public ElementsByTokenAttributeIndex(DomDocument owner, DomName name, Func<string, TKey> func, IEqualityComparer<TKey> comparer) : base(owner, name, func, comparer) {
            }

            protected override void Add(string value, DomElement target) {
                if (string.IsNullOrEmpty(value)) {
                    return;
                }
                foreach (var k in GetKeys(value)) {
                    Items.Add(k, target);
                }
            }

            protected override void Remove(string value, DomElement target) {
                if (string.IsNullOrEmpty(value)) {
                    return;
                }
                foreach (var k in GetKeys(value)) {
                    Items.Remove(k, target);
                }
            }

            private IEnumerable<TKey> GetKeys(string value) {
                return DomStringTokenList.Parse(value).Select(Key);
            }
        }
    }
}
