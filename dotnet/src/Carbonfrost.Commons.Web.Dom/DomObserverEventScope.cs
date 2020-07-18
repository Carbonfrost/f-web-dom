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

    struct DomObserverEventScope : IEquatable<DomObserverEventScope> {

        private readonly DomName _name;
        private readonly Kind _kind;

        public static readonly DomObserverEventScope AnyEvent = new DomObserverEventScope(Kind.AnyEvent, null);
        public static readonly DomObserverEventScope ChildNodes = new DomObserverEventScope(Kind.ChildNodes, null);
        public static readonly DomObserverEventScope AnyAttribute = new DomObserverEventScope(Kind.Attribute, null);

        public static DomObserverEventScope SpecificAttribute(DomName name) {
            return new DomObserverEventScope(Kind.Attribute, name);
        }

        public DomObserverEventScope? Parent {
            get {
                switch (_kind) {
                    case Kind.ChildNodes:
                    case Kind.Attribute when _name is null:
                        return AnyEvent;
                    case Kind.Attribute:
                        return AnyAttribute;
                    case Kind.AnyEvent:
                    default:
                        return null;
                }
            }
        }

        public IEnumerable<DomObserverEventScope> AncestorsAndSelf {
            get {
                DomObserverEventScope? s = this;
                while (s is DomObserverEventScope ss) {
                    yield return ss;
                    s = ss.Parent;
                }
            }
        }

        private DomObserverEventScope(Kind kind, DomName name) {
            _name = name;
            _kind = kind;
        }

        public override bool Equals(object obj) {
            return obj is DomObserverEventScope scope && Equals(scope);
        }

        public override int GetHashCode() {
            int hashCode = 189556954;
            if (_name != null) {
                hashCode = hashCode * -1521134295 + _name.GetHashCode();
            }
            hashCode = hashCode * -1521134295 + _kind.GetHashCode();
            return hashCode;
        }

        public override string ToString() {
            return string.Join(" ", _kind, _name);
        }

        public bool Equals(DomObserverEventScope other) {
            return _name == other._name && _kind == other._kind;
        }

        enum Kind {
            AnyEvent,
            ChildNodes,
            Attribute,
        }
    }
}
