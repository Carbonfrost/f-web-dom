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

        public DomElementIndex<TKey> CreateAttributeIndex<TKey>(DomName name, Func<string, TKey> func = null, IEqualityComparer<TKey> keyComparer = null) {
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }

            if (func is null) {
                func = s => Activation.FromText<TKey>(s);
            }

            keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;

            return new ElementsByAttributeIndex<TKey>(this, name, func, keyComparer);
        }

        public DomElementIndex<string> CreateAttributeIndex(DomName name) {
            return CreateAttributeIndex(name, s => s ?? string.Empty, null);
        }

        public DomElementIndex<string> CreateAttributeIndex(DomName name, IEqualityComparer<string> keyComparer = null) {
            return new ElementsByAttributeIndex<string>(this, name, s => s, keyComparer);
        }

        class ElementsByAttributeIndex<TKey> : DomElementIndex<TKey> {
            private readonly DomObserver _observer;
            private readonly DomName _attributeName;
            private readonly Func<string, TKey> _func;

            public ElementsByAttributeIndex(DomDocument owner, DomName name, Func<string, TKey> func, IEqualityComparer<TKey> comparer) : base(comparer) {
                _attributeName = name;
                _func = func;
                _observer = DomObserver.Compose(
                    owner.ObserveAttributes(name, Handler),
                    owner.ObserveChildNodes(Handler)
                );

                foreach (var desc in owner.DescendantsAndSelf) {
                    Add(desc.Attribute(_attributeName), desc);
                }
            }

            private void Handler(DomAttributeEvent evt) {
                Remove(evt.OldValue, evt.Target);
                Add(evt.Value, evt.Target);
            }

            private void Handler(DomMutationEvent evt) {
                foreach (var node in evt.RemovedNodes.OfType<DomElement>()) {
                    Remove(node.Attribute(_attributeName), node);
                }
                foreach (var node in evt.AddedNodes.OfType<DomElement>()) {
                    Add(node.Attribute(_attributeName), node);
                }
            }

            protected override void DisconnectCore() {
                _observer.Dispose();
            }

            protected virtual void Add(string value, DomElement target) {
                if (value == null) {
                    return;
                }
                Items.Add(Key(value), target);
            }

            protected virtual void Remove(string value, DomElement target) {
                if (value == null) {
                    return;
                }
                Items.Remove(Key(value), target);
            }

            protected TKey Key(string v) {
                var result = _func(v);
                if (result == null) {
                    throw new NotImplementedException();
                }
                return result;
            }
        }
    }
}
