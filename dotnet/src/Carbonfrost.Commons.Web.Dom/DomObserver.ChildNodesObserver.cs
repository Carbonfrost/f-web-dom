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

namespace Carbonfrost.Commons.Web.Dom {

    public abstract partial class DomObserver {

        internal static DomObserverBase ChildNodes(DomNode target, DomScope scope, Action<DomMutationEvent> handler) {
            return new ChildNodesObserver(target, scope, handler);
        }

        sealed class ChildNodesObserver : DomObserverBase {

            private Action<DomMutationEvent> _handler;

            internal ChildNodesObserver(DomNode target, DomScope scope, Action<DomMutationEvent> handler) : base(target, scope) {
                _handler = handler;
            }

            internal override void ChildNodesChanged(DomMutation mutation, DomNode parent, DomNode[] nodes, DomNode previous, DomNode next) {
                if (IsDisposed) {
                    return;
                }

                var evt = DomMutationEvent.Create(mutation, parent, nodes, previous, next);
                _handler(evt);
            }

            protected override void Dispose(bool manualDispose) {
                _handler = null;
                base.Dispose(manualDispose);
            }
        }
    }

}
