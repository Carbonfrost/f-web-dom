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

        internal static DomObserverBase Attributes(DomNode target, DomScope scope, Action<DomAttributeEvent> handler) {
            return new AttributeObserver(target, scope, handler);
        }

        sealed class AttributeObserver : DomObserverBase {

            private Action<DomAttributeEvent> _handler;

            internal AttributeObserver(DomNode target, DomScope scope, Action<DomAttributeEvent> handler) : base(target, scope) {
                _handler = handler;
            }

            internal override void AttributeValueChanged(DomAttribute attr, DomElement target, string oldValue) {
                if (IsDisposed) {
                    return;
                }

                var evt = new DomAttributeEvent(target, attr.Name, oldValue);
                _handler(evt);
            }

            protected override void Dispose(bool manualDispose) {
                _handler = null;
                base.Dispose(manualDispose);
            }
        }
    }

}
