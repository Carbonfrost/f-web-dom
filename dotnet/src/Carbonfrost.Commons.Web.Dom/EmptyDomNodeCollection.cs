//
// Copyright 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System.Collections.Generic;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    sealed class EmptyDomNodeCollectionImpl : DomNodeCollection {

        internal EmptyDomNodeCollectionImpl() : base(null) {}

        public override int Count {
            get {
                return 0;
            }
        }

        public override bool IsReadOnly {
            get {
                return true;
            }
        }

        public override IEnumerator<DomNode> GetEnumerator() {
            yield break;
        }

        public override int IndexOf(DomNode node) {
            return -1;
        }

        internal override DomNode GetItemCore(int index) {
            throw Failure.IndexOutOfRange("index", index);
        }

        internal override void InsertCore(int index, DomNode item) {
            throw Failure.ReadOnlyCollection();
        }

        internal override void ClearCore() {
            throw Failure.ReadOnlyCollection();
        }

        internal override void RemoveAtCore(int index) {
            throw Failure.ReadOnlyCollection();
        }

        internal override bool RemoveCore(DomNode node) {
            throw Failure.ReadOnlyCollection();
        }

        internal override void SetItemCore(int index, DomNode item) {
            throw Failure.ReadOnlyCollection();
        }
    }
}
