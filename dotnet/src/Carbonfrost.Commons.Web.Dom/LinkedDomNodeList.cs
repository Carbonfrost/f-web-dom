//
// Copyright 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    partial class LinkedDomNodeList : DomNodeCollection {

        internal override DomNode GetNextSibling(DomNode other) {
            return other.next;
        }

        internal override DomNode GetPreviousSibling(DomNode other) {
            return other.prev;
        }

        public override void InsertRange(int index, IEnumerable<DomNode> items) {
            if (items == null) {
                throw new ArgumentNullException(nameof(items));
            }
            if (index < 0 || index > Count) {
                throw Failure.IndexOutOfRange(nameof(index), index);
            }
            if (index == Count) {
                AddRange(items);
                return;
            }
            var current = Find(index);
            var nodes = items.ToList();
            nodes.Reverse();
            foreach (var element in nodes) {
                AddBefore(current, element);
                current = element;
            }
        }
    }
}
