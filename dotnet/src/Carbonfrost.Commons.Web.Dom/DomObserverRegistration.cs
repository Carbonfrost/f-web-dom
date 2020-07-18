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

    class DomObserverRegistration : IObservable<DomEvent> {

        private readonly Dictionary<DomObserverEventScope, IList<DomObserverTarget>> _items
            = new Dictionary<DomObserverEventScope, IList<DomObserverTarget>>();

        public IEnumerable<IObserver<DomEvent>> GetObservers(DomNode node, DomObserverEventScope scope) {
            var answer = scope.AncestorsAndSelf.SelectMany(
                s => _items.GetValueOrDefault(s, Array.Empty<DomObserverTarget>())
            );

            // TODO Should have a heuristic for freeing DomObserverTargets when they are
            // disposed or their lists become empty
            return answer.Where(a => a.AppliesTo(node)).Select(a => a.Observer);
        }

        public void RegisterObserver(DomObserver observer, DomNode target, DomScope scope, DomObserverEventScope events) {
            var list = GetItemsForScope(events);
            list.Add(new ScopedDomObserverTarget(observer, target, scope));
        }

        private IList<DomObserverTarget> GetItemsForScope(DomObserverEventScope scope) {
            IList<DomObserverTarget> list;
            if (!_items.TryGetValue(scope, out list)) {

                // CopyOnWriteList for observers because it is possible new observers could be added in callbacks
                _items[scope] = list = new CopyOnWriteList<DomObserverTarget>();
            }
            return list;
        }

        public IDisposable Subscribe(IObserver<DomEvent> observer) {
            if (observer is null) {
                throw new ArgumentNullException(nameof(observer));
            }

            var list = GetItemsForScope(DomObserverEventScope.AnyEvent);
            var result = new SimpleDomObserverTarget(observer);
            list.Add(result);
            return result;
        }

        abstract class DomObserverTarget : IDisposable {
            public readonly IObserver<DomEvent> Observer;
            private bool _disposed;

            internal bool Disposed {
                get {
                    return _disposed;
                }
            }

            protected DomObserverTarget(IObserver<DomEvent> observer) {
                Observer = observer;

                if (observer is DomObserver d) {
                    // DomObserver allows disposal via the registration result or the observer itself
                    d.SetLifetime(this);
                }
            }

            public void Dispose() {
                _disposed = true;
            }

            internal abstract bool AppliesTo(DomNode node);
        }

        sealed class SimpleDomObserverTarget : DomObserverTarget {

            public SimpleDomObserverTarget(IObserver<DomEvent> observer) : base(observer) {
            }

            internal override bool AppliesTo(DomNode node) {
                if (Disposed) {
                    return false;
                }
                return true;
            }
        }

        sealed class ScopedDomObserverTarget : DomObserverTarget {

            public readonly DomNode Target;
            public readonly DomScope Scope;

            public ScopedDomObserverTarget(DomObserver observer, DomNode target, DomScope scope) : base(observer) {
                Target = target;
                Scope = scope;
            }

            internal override bool AppliesTo(DomNode node) {
                if (Disposed) {
                    return false;
                }

                if (Target == node) {
                    return true;
                }

                if (Scope == DomScope.TargetAndDescendants) {
                    return node.AncestorNodes.Any(n => n == Target);
                }

                if (Scope == DomScope.TargetAndAncestors) {
                    return Target.AncestorNodes.Any(n => n == node);
                }

                return false;
            }
        }
    }
}
