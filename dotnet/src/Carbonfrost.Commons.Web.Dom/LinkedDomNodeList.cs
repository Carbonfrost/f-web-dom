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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    partial class LinkedDomNodeList : DomNodeCollection {

        public LinkedDomNodeList(DomContainer owner)
            : base(owner) {}

        public override void InsertRange(int index, IEnumerable<DomNode> items) {
            if (items == null) {
                throw new ArgumentNullException("items");
            }
            if (index < 0 || index > Count) {
                throw Failure.IndexOutOfRange("index", index);
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
