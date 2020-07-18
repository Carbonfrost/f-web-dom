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

    partial class DomDocument {

        // Copy observers because it is possible new observers could be added in callbacks
        private readonly DomObserverRegistration _observers = new DomObserverRegistration();

        public IObservable<DomEvent> DomEvents {
            get {
                return _observers;
            }
        }

        private IEnumerable<IObserver<DomEvent>> GetObservers(DomObserverEventScope s, DomNode node) {
            return _observers.GetObservers(node, s);
        }

        internal void AttributeValueChanged(DomAttribute attr, DomElement element, string oldValue) {
            DomAttributeEvent evt = null;
            foreach (var o in GetObservers(DomObserverEventScope.SpecificAttribute(attr.Name), element)) {
                if (evt == null) {
                    evt = new DomAttributeEvent(element, attr.Name, oldValue);
                }
                o.OnNext(evt);
            }
        }

        internal void ChildNodesChanged(DomMutation mutation, DomNode target, DomNode[] nodes, DomNode prev, DomNode next) {
            DomEvent evt = null;
            foreach (var o in GetObservers(DomObserverEventScope.ChildNodes, target)) {
                if (evt == null) {
                    evt = DomMutationEvent.Create(mutation, target, nodes, prev, next);
                }
                o.OnNext(evt);
            }
        }

        public DomObserver ObserveAttributes(DomName attribute, Action<DomAttributeEvent> handler) {
            return ObserveAttributes(this, attribute, handler, DomScope.TargetAndDescendants);
        }

        public DomObserver ObserveAttributes(DomNode target, DomName attribute, Action<DomAttributeEvent> handler) {
            return ObserveAttributes(target, attribute, handler, DomScope.Target);
        }

        public DomObserver ObserveAttributes(DomNode target, DomName attribute, Action<DomAttributeEvent> handler, DomScope scope) {
            if (target is null) {
                throw new ArgumentNullException(nameof(target));
            }
            if (attribute is null) {
                throw new ArgumentNullException(nameof(attribute));
            }
            if (handler is null) {
                throw new ArgumentNullException(nameof(handler));
            }

            var result = DomObserver.Attributes(handler);
            _observers.RegisterObserver(
                result, target, scope, DomObserverEventScope.AnyAttribute
            );
            return result;
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

            var result = DomObserver.Attributes(handler);
            _observers.RegisterObserver(
                result, target, scope, DomObserverEventScope.AnyAttribute
            );
            return result;
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

            var result = DomObserver.ChildNodes(handler);
            _observers.RegisterObserver(
                result, target, scope, DomObserverEventScope.ChildNodes
            );
            return result;
        }

    }
}
