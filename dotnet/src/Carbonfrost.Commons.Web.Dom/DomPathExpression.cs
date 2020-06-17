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

    public abstract partial class DomPathExpression : IDomNameApiConventions {

        public virtual DomName Name {
            get {
                throw new NotSupportedException();
            }
        }

        public string Prefix {
            get {
                return Name.Prefix;
            }
        }

        public virtual string Value {
            get {
                throw new NotSupportedException();
            }
        }

        public virtual int Index {
            get {
                throw new NotSupportedException();
            }
        }

        public virtual string Id {
            get {
                throw new NotSupportedException();
            }
        }

        public abstract DomPathExpressionType Type { get; }

        public string LocalName {
            get {
                return Name.LocalName;
            }
        }

        public DomNamespace Namespace {
            get {
                return Name.Namespace;
            }
        }

        public string NamespaceUri {
            get {
                return Name.NamespaceUri;
            }
        }

        internal DomPathExpression() {
        }

        internal abstract DomObjectQuery Apply(DomObjectQuery root, DomObjectQuery q);

        internal static IEnumerable<DomElement> FilterChildren(DomObjectQuery q, Func<DomElement, bool> predicate) {
            return q.OfType<DomContainer>().SelectMany(e => e.Children.Where(predicate));
        }

        internal static IEnumerable<DomNode> FilterDescendants(DomObjectQuery q, Func<DomNode, bool> predicate) {
            return q.OfType<DomNode>().SelectMany(e => e.DescendantNodesAndSelf.Where(predicate));
        }

    }
}
