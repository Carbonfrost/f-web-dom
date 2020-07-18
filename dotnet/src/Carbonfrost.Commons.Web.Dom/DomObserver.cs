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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract partial class DomObserver : DisposableObject, IObserver<DomEvent> {

        public static readonly DomObserver Empty = new EmptyImpl();
        private IDisposable _lifetime = DisposableObject.Null;

        [DomNodeFactoryUsage]
        public static DomObserver Compose(params DomObserver[] items) {
            return Compose((IEnumerable<DomObserver>) items);
        }

        public static DomObserver Compose(IEnumerable<DomObserver> items) {
            return Utility.OptimalComposite(
                items, m => new CompositeObserver(m), Empty
            );
        }

        internal void SetLifetime(IDisposable lifetime) {
            _lifetime = lifetime;
        }

        protected virtual void OnError(Exception error) {
        }

        protected virtual void OnMutationEvent(DomMutationEvent value) {
        }

        protected virtual void OnAttributeEvent(DomAttributeEvent value) {
        }

        protected virtual void OnEvent(DomEvent value) {
            if (IsDisposed) {
                return;
            }

            switch (value) {
                case DomAttributeEvent attr:
                    OnAttributeEvent(attr);
                    break;

                case DomMutationEvent mut:
                    OnMutationEvent(mut);
                    break;
            }
        }

        protected override void Dispose(bool manualDispose) {
            if (manualDispose) {
                _lifetime.Dispose();
            }
            base.Dispose(manualDispose);
        }

        void IObserver<DomEvent>.OnCompleted() {
        }

        void IObserver<DomEvent>.OnError(Exception error) {
            OnError(error);
        }

        void IObserver<DomEvent>.OnNext(DomEvent value) {
            OnEvent(value);
        }

        sealed class EmptyImpl : DomObserver {
        }

    }

}
