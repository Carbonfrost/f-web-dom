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

    public class DomMutationEvent : DomEvent {

        public DomNodeCollection AddedNodes {
            get;
        }

        public DomNodeCollection RemovedNodes {
            get;
        }

        public DomNode PreviousSiblingNode {
            get;
        }

        public DomNode NextSiblingNode {
            get;
        }

        private DomMutationEvent(
            DomNode target,
            IEnumerable<DomNode> addedNodes,
            IEnumerable<DomNode> removedNodes,
            DomNode previousSiblingNode,
            DomNode nextSiblingNode
        ) : base(target) {
            AddedNodes = DomNodeCollection.Create(addedNodes);
            RemovedNodes = DomNodeCollection.Create(removedNodes);
            PreviousSiblingNode = previousSiblingNode;
            NextSiblingNode = nextSiblingNode;
        }

        internal static DomMutationEvent Create(DomMutation mutation, DomNode target, IEnumerable<DomNode> nodes, DomNode previous, DomNode next) {
            if (mutation == DomMutation.Add) {
                return Added(target, nodes, previous, next);
            } else {
                return Removed(target, nodes, previous, next);
            }
        }

        internal static DomMutationEvent Added(DomNode target, IEnumerable<DomNode> nodes, DomNode previous, DomNode next) {
            return new DomMutationEvent(target, nodes, null, previous, next);
        }

        internal static DomMutationEvent Removed(DomNode target, IEnumerable<DomNode> nodes, DomNode previous, DomNode next) {
            return new DomMutationEvent(target, null, nodes, previous, next);
        }
    }
}
