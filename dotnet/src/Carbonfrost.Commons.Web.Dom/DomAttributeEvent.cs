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

namespace Carbonfrost.Commons.Web.Dom {

    public class DomAttributeEvent : DomEvent, IDomNameApiConventions {

        private readonly DomName _attribute;
        private readonly string _oldValue;

        public string OldValue {
            get {
                return  _oldValue;
            }
        }

        public new DomElement Target {
            get {
                return (DomElement) base.Target;
            }
        }

        public string Value {
            get {
                return Target.Attribute(_attribute);
            }
        }

        public DomName Name {
            get {
                return _attribute;
            }
        }

        public string LocalName {
            get {
                return _attribute.LocalName;
            }
        }

        public DomNamespace Namespace {
            get {
                return _attribute.Namespace;
            }
        }

        public string NamespaceUri {
            get {
                return _attribute.NamespaceUri;
            }
        }

        public string Prefix {
            get {
                return _attribute.Prefix;
            }
        }

        internal DomAttributeEvent(DomNode target, DomName attribute, string oldValue) : base(target) {
            _attribute = attribute;
            _oldValue = oldValue;
        }
    }

}
