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

        sealed class CompositeObserver : DomObserver {

            private readonly DomObserver[] _items;

            public CompositeObserver(DomObserver[] items) {
                _items = items;
            }

            protected override void OnAttributeEvent(DomAttributeEvent value) {
                Each(i => i.OnAttributeEvent(value));
            }

            protected override void OnMutationEvent(DomMutationEvent value) {
                Each(i => i.OnMutationEvent(value));
            }

            protected override void OnEvent(DomEvent value) {
                Each(i => i.OnEvent(value));
            }

            protected override void Dispose(bool manualDispose) {
                foreach (var i in _items) {
                    try {
                        i.Dispose();
                    } catch {
                    }
                }
            }

            private void Each(Action<DomObserver> p) {
                var agg = FrugalList<Exception>.Empty;
                foreach (var i in _items) {
                    try {
                        p(i);
                    } catch (Exception ex) {
                        agg = agg.Add(ex);
                    }
                }
                if (agg.Count > 0) {
                    throw new AggregateException(agg);
                }
            }
        }
    }

}
