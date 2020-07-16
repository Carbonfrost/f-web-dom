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

using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    class DomObserverBase : DomObserver {
        private readonly DomNode _target;
        private readonly DomScope _scope;

        internal DomObserverBase(DomNode target, DomScope scope) {
            _target = target;
            _scope = scope;
        }

        internal virtual void AttributeValueChanged(DomAttribute attr, DomElement target, string oldValue) {
        }

        internal virtual void ChildNodesChanged(DomMutation mutation, DomNode parent, DomNode[] nodes, DomNode previous, DomNode next) {
        }

        internal bool AppliesTo(DomNode node) {
            if (_target == node) {
                return true;
            }

            if (_scope == DomScope.TargetAndDescendants) {
                return node.AncestorNodes.Any(n => n == _target);
            }

            if (_scope == DomScope.TargetAndAncestors) {
                return _target.AncestorNodes.Any(n => n == node);
            }

            return false;
        }
    }

}
