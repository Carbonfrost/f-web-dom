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
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomObjectQuery : IDomNodeManipulation<DomObjectQuery> {

        public DomObjectQuery Unwrap() {
            return new DomObjectQuery(this.Select(m => m.Unwrap()).NonNull());
        }

        public DomObjectQuery RemoveAttributes() {
            return new DomObjectQuery(this.Select(m => m.RemoveAttributes()));
        }

        public DomObjectQuery RemoveAttribute(string name) {
            return new DomObjectQuery(this.Select(m => m.RemoveAttribute(name)));
        }

        public DomObjectQuery AddClass(string className) {
            return new DomObjectQuery(this.Select(m => m.AddClass(className)));
        }

        public DomObjectQuery RemoveClass(string className) {
            return new DomObjectQuery(this.Select(m => m.RemoveClass(className)));
        }

        public DomObjectQuery SetName(string name) {
            return new DomObjectQuery(SetNameInternal(name));
        }

        public DomObjectQuery Wrap(string element) {
            return new DomObjectQuery(this.Select(m => m.Wrap(element)));
        }

        public DomObjectQuery Wrap(DomNode newParent) {
            return new DomObjectQuery(this.Select(m => m.Wrap(newParent)));
        }

        public DomObjectQuery After(DomNode nextSibling) {
            throw new NotImplementedException();
        }

        public DomObjectQuery After(params DomNode[] nextSiblings) {
            throw new NotImplementedException();
        }

        public DomObjectQuery After(string text) {
            throw new NotImplementedException();
        }

        public DomObjectQuery InsertAfter(DomNode other) {
            throw new NotImplementedException();
        }

        public DomObjectQuery Before(DomNode previousSibling) {
            throw new NotImplementedException();

        }
        public DomObjectQuery Before(params DomNode[] previousSiblings) {
            throw new NotImplementedException();
        }

        public DomObjectQuery Before(string text) {
            throw new NotImplementedException();
        }

        public DomObjectQuery InsertBefore(DomNode other) {
            throw new NotImplementedException();
        }

        public DomWriter Append() {
            throw new NotImplementedException();
        }

        public DomObjectQuery Append(DomNode child) {
            throw new NotImplementedException();
        }

        public DomObjectQuery Append(string text) {
            throw new NotImplementedException();
        }

        public DomObjectQuery AppendTo(DomNode parent) {
            throw new NotImplementedException();
        }

        public DomWriter Prepend() {
            throw new NotImplementedException();
        }

        public DomObjectQuery Prepend(DomNode child) {
            throw new NotImplementedException();
        }

        public DomObjectQuery Prepend(string text) {
            throw new NotImplementedException();
        }

        public DomObjectQuery PrependTo(DomNode parent) {
            throw new NotImplementedException();
        }

        public DomObjectQuery ReplaceWith(DomNode other) {
            throw new NotImplementedException();
        }

        public DomObjectQuery ReplaceWith(params DomNode[] others) {
            throw new NotImplementedException();
        }

        public DomObjectQuery ReplaceWith(string text) {
            throw new NotImplementedException();
        }

        private IEnumerable<DomNode> SetNameInternal(string name) {
            foreach (var m in this) {
                if (m.IsAttribute || m.IsElement) {
                    yield return m.SetName(name);
                } else {
                    yield return m;
                }
            }
        }

    }

}
