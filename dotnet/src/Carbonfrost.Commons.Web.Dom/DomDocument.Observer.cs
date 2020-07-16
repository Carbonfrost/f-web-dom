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

    partial class DomDocument {

        private readonly List<DomObserverBase> _observers = new List<DomObserverBase>();

        private IEnumerable<DomObserverBase> GetObservers(DomNode node) {
            // Copy observers because it is possible new observers could be added in callbacks
            return _observers.ToList().Where(o => o.AppliesTo(node));
        }

        internal void AttributeValueChanged(DomAttribute attr, DomElement element, string oldValue) {
            foreach (var o in GetObservers(element)) {
                o.AttributeValueChanged(attr, element, oldValue);
            }
        }

        internal void ChildNodesChanged(DomMutation mutation, DomNode target, DomNode[] nodes, DomNode prev, DomNode next) {
            foreach (var o in GetObservers(target)) {
                o.ChildNodesChanged(mutation, target, nodes, prev, next);
            }
        }

        public DomObserver ObserveAttributes(Action<DomAttributeEvent> handler) {
            return ObserveAttributes(this, handler, DomScope.TargetAndDescendants);
        }

        public DomObserver ObserveAttributes(DomNode target, Action<DomAttributeEvent> handler) {
            return ObserveAttributes(target, handler, DomScope.Target);
        }

        public DomObserver ObserveAttributes(DomNode target, Action<DomAttributeEvent> handler, DomScope scope) {
            if (target is null) {
                throw new ArgumentNullException(nameof(target));
            }
            if (handler is null) {
                throw new ArgumentNullException(nameof(handler));
            }

            return AddObserver(DomObserver.Attributes(target, scope, handler));
        }

        public DomObserver ObserveChildNodes(Action<DomMutationEvent> handler) {
            return ObserveChildNodes(this, handler, DomScope.TargetAndDescendants);
        }

        public DomObserver ObserveChildNodes(DomNode target, Action<DomMutationEvent> handler) {
            return ObserveChildNodes(target, handler, DomScope.Target);
        }

        public DomObserver ObserveChildNodes(DomNode target, Action<DomMutationEvent> handler, DomScope scope) {
            if (target is null) {
                throw new ArgumentNullException(nameof(target));
            }
            if (handler is null) {
                throw new ArgumentNullException(nameof(handler));
            }

            return AddObserver(DomObserver.ChildNodes(target, scope, handler));
        }

        private DomObserver AddObserver(DomObserverBase obs) {
            _observers.Add(obs);
            return obs;
        }

    }
}
