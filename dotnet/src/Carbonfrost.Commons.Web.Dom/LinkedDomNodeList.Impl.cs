// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    // Adapted from LinkedList<T> except removing generics and treating DomNode as the
    // node type; slimmed down a great deal

    partial class LinkedDomNodeList {

        // This LinkedList is a doubly-Linked circular list.
        internal DomNode _head;
        internal int count;
        internal int version;

        public override int Count {
            get { return count; }
        }

        public override int IndexOf(DomNode node) {
            int result;
            Find(node, out result);
            return result;
        }

        public override DomNode this[int index] {
            get {
                return GetItemCore(index);
            }
            set {
                SetItemCore(index, value);
            }
        }

        public override void Add(DomNode item) {
            InsertCore(Count, item);
        }

        public override void Clear() {
            ClearCore();
        }

        public override bool Contains(DomNode item) {
            return item.list == this;
        }

        public override void Insert(int index, DomNode item) {
            InsertCore(index, item);
        }

        public override bool Remove(DomNode item) {
            return RemoveCore(item);
        }

        public override void RemoveAt(int index) {
            RemoveAtCore(index);
        }

        private DomNode GetItemCore(int index) {
            return Find(index);
        }

        private void InsertCore(int index, DomNode item) {
            if (index == Count) {
                AddLast(item);
            } else {
                AddBefore(Find(index), item);
            }
        }

        private void ClearCore() {
            DomNode current = _head;
            while (current != null) {
                DomNode temp = current;
                current = current.Next;   // use Next the instead of "next", otherwise it will loop forever
                temp.Invalidate();
            }

            _head = null;
            count = 0;
            version++;
        }

        private void RemoveAtCore(int index) {
            RemoveCore(Find(index));
        }

        private bool RemoveCore(DomNode value) {
            DomNode node = value;
            if (node != null) {
                InternalRemoveNode(node);
                return true;
            }
            return false;
        }

        private void SetItemCore(int index, DomNode item) {
            var current = Find(index);
            AddBefore(current, item);
            RemoveCore(current);
        }

        private void AddAfter(DomNode node, DomNode newNode) {
            ValidateNode(node);
            ValidateNewNode(newNode);
            InternalInsertNodeBefore(node.next, newNode);
        }

        private void AddBefore(DomNode node, DomNode newNode) {
            ValidateNode(node);
            ValidateNewNode(newNode);
            InternalInsertNodeBefore(node, newNode);
            if (node == _head) {
                _head = newNode;
            }
        }

        private void AddLast(DomNode value) {
            DomNode result = value;
            if (_head == null) {
                InternalInsertNodeToEmptyList(result);
            } else {
                InternalInsertNodeBefore(_head, result);
            }
        }

        public DomNode Find(DomNode value) {
            int index;
            return Find(value, out index);
        }

        public DomNode Find(DomNode value, out int index) {
            index = 0;
            DomNode node = _head;
            if (node != null) {
                do {
                    if (ReferenceEquals(node, value)) {
                        return node;
                    }
                    node = node.next;
                    index++;
                } while (node != _head);
            }
            index = -1;
            return null;
        }

        public DomNode Find(int index) {
            int current = 0;
            DomNode node = _head;
            if (node != null) {
                do {
                    if (current == index) {
                        // TODO Cache previous index as promised
                        return node;
                    }
                    node = node.next;
                    current++;
                } while (node != _head);
            }
            return null;
        }

        public override IEnumerator<DomNode> GetEnumerator() {
            return new Enumerator(this);
        }

        private void InternalInsertNodeBefore(DomNode node, DomNode newNode) {
            newNode.next = node;
            newNode.prev = node.prev;
            node.prev.next = newNode;
            node.prev = newNode;
            version++;
            count++;
        }

        private void InternalInsertNodeToEmptyList(DomNode newNode) {
            Debug.Assert(_head == null && count == 0, "LinkedList must be empty when this method is called!");
            newNode.next = newNode;
            newNode.prev = newNode;
            _head = newNode;
            version++;
            count++;
        }

        internal void InternalRemoveNode(DomNode node) {
            Debug.Assert(_head != null, "This method shouldn't be called on empty list!");
            if (node.next == node) {
                Debug.Assert(count == 1 && _head == node, "this should only be true for a list with only one node");
                _head = null;
            } else {
                node.next.prev = node.prev;
                node.prev.next = node.next;
                if (_head == node) {
                    _head = node.next;
                }
            }
            node.Invalidate();
            count--;
            version++;
        }

        internal void ValidateNewNode(DomNode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }
        }

        internal void ValidateNode(DomNode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }
        }

        public struct Enumerator : IEnumerator<DomNode>, System.Collections.IEnumerator {
            private readonly LinkedDomNodeList _list;
            private DomNode _node;
            private DomNode _current;
            private int _version;
            private int _index;

            internal Enumerator(LinkedDomNodeList list) {
                _list = list;
                _version = list.version;
                _node = list._head;
                _index = 0;
                _current = null;
            }

            public DomNode Current {
                get {
                    if (_index == 0 || (_index == 1 + _list.Count)) {
                        throw Failure.OutsideEnumeration();
                    }
                    return _current;
                }
            }

            object System.Collections.IEnumerator.Current {
                get {
                    return Current;
                }
            }

            public bool MoveNext() {
                if (_version != _list.version) {
                    throw Failure.CollectionModified();
                }

                if (_node == null) {
                    _index = _list.Count + 1;
                    return false;
                }

                ++_index;
                _current = _node;
                _node = _node.next;
                if (_node == _list._head) {
                    _node = null;
                }
                return true;
            }

            void System.Collections.IEnumerator.Reset() {
                if (_version != _list.version) {
                    throw Failure.CollectionModified();
                }
                _node = _list._head;
                _index = 0;
            }

            public void Dispose() {
            }
        }
    }

    partial class DomNode {

        internal DomNode next;
        internal DomNode prev;

        internal LinkedDomNodeList list {
            get {
                return (LinkedDomNodeList) ((DomNodeCollectionApi) _Siblings).UnsafeItems;
            }
        }

        internal DomNode Next {
            get { return next == null || next == list._head ? null : next; }
        }

        internal DomNode Previous {
            get { return prev == null || this == list._head ? null : prev; }
        }

        internal void Invalidate() {
            next = null;
            prev = null;
        }
    }
}
