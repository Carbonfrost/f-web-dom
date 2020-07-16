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
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomContainer {

        internal IDisposable NewMutationBatch() {
            return _batch.NewScope();
        }

        private class MutationBatchStack {

            private readonly Stack<MutationBatch> _stack = new Stack<MutationBatch>();
            private readonly DomContainer _owner;

            private DomDocument Document {
                get {
                    return _owner.OwnerDocumentOrSelf;
                }
            }

            public MutationBatchStack(DomContainer owner) {
                _owner = owner;
            }

            internal void Add(DomMutation mut, DomNode node, DomNode prev, DomNode next) {
                if (_stack.Count > 0) {
                    _stack.Peek().Add(mut, node, prev, next);
                    return;
                }

                _owner.OwnerDocumentOrSelf.ChildNodesChanged(mut, _owner, new [] { node }, prev, next);
            }

            internal IDisposable NewScope() {
                var result = new MutationBatch(this);
                _stack.Push(result);
                return result;
            }

            private void PopRecord() {
                var top = _stack.Pop();
                if (_stack.Count > 0) {
                    var newTop = _stack.Peek();
                    if (newTop._previous == null) {
                        newTop._previous = top._previous;
                        newTop._current = top._current;
                        return;
                    }
                }

                if (_stack.Count == 0) {
                    top.PopRecord(Document, _owner);
                }
            }

            private class MutationBatch : IDisposable {
                private readonly MutationBatchStack _owner;
                private DomMutation _mutation;

                // Mutation batch will track forward, contiguous insertions of nodes.
                // _current will be whichever node was added, and we expect the next call
                // in the batch to move relative to the _current node.
                // _previous and _next track the initial insertion point
                internal DomNode _current;
                internal DomNode _previous;
                internal DomNode _next;

                internal IEnumerable<DomNode> Nodes {
                    get {
                        return _previous.FollowingSiblingNodes.TakeWhile(s => s != null && s != _next);
                    }
                }

                public MutationBatch(MutationBatchStack owner) {
                    _owner = owner;
                }

                internal void Add(DomMutation mut, DomNode node, DomNode prev, DomNode next) {
                    if (_current == null) {
                        // This is the first insertion, so track the initial state
                        _current = node;
                        _previous = prev;
                        _next = next;
                        _mutation = mut;

                    } else if (mut == _mutation && prev == _current) {
                        // An adjacent insertion in the forward direction
                        _current = node;
                    } else {
                        _owner.PopRecord();
                    }
                }

                internal void PopRecord(DomDocument document, DomContainer target) {
                    if (_previous == null) {
                        return;
                    }

                    document.ChildNodesChanged(_mutation, target, Nodes.ToArray(), _previous, _next);
                }

                public void Dispose() {
                    _owner.PopRecord();
                }
            }
        }
    }
}
