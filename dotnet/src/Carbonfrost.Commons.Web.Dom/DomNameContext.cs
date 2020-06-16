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
using System.Text.RegularExpressions;

namespace Carbonfrost.Commons.Web.Dom {

    public class DomNameContext : IDomNameComparer {

        public static readonly DomNameContext Html = new Impl(
            @"#?[a-zA-Z]\S*",
            ".+",
            DomNameComparer.IgnoreCase
        );

        public static readonly DomNameContext Default = new Impl(
            @"#?[a-zA-Z][A-Za-z0-9._:-]*",
            @".+",
            DomNameComparer.IgnoreCase
        );

        public static readonly DomNameContext Xml = new XmlNameContext();

        public virtual DomNameComparer Comparer {
            get {
                return DomNameComparer.Ordinal;
            }
        }

        public virtual bool IsValidName(DomName name) {
            return name != null
                && IsValidLocalName(name.LocalName)
                && IsValidNamespaceUri(name.NamespaceUri);
        }

        public virtual bool IsValidLocalName(string name) {
            return true;
        }

        public virtual bool IsValidNamespaceUri(string namespaceUri) {
            return true;
        }

        public DomName GetName(string localName) {
            return DomName.Create(localName).Resolve(this);
        }

        public virtual DomName GetName(DomName name) {
            return name;
        }

        internal T DemandValidName<T>(string argName, T name) where T : IDomNameApiConventions {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (!IsValidLocalName(name.LocalName)) {
                throw DomFailure.NotValidNameForThisContext(argName, name);
            }
            return name;
        }

        public int Compare(DomName x, DomName y) {
            return ((IComparer<DomName>)Comparer).Compare(x, y);
        }

        // The methods of IEqualityComparer are provided for convenience so
        // that DomNameContext can be used directly, but they are not API because
        // they have confusing names

        bool IEqualityComparer<DomName>.Equals(DomName x, DomName y) {
            return Comparer.Equals(x, y);
        }

        int IEqualityComparer<DomName>.GetHashCode(DomName obj) {
            return Comparer.GetHashCode(obj);
        }

        public class Impl : DomNameContext {
            private readonly Regex _namePattern;
            private readonly Regex _namespaceUriPattern;
            private readonly DomNameComparer _comparer;

            public Impl(string namePattern, string namespaceUriPattern, DomNameComparer comparer) {
                _namePattern = new Regex($@"\A{namePattern}\Z");
                _namespaceUriPattern = new Regex($@"\A{namespaceUriPattern}\Z");
                _comparer = comparer;
            }

            public override DomNameComparer Comparer {
                get {
                    return _comparer;
                }
            }

            public override bool IsValidLocalName(string name) {
                return _namePattern.IsMatch(name);
            }

            public override bool IsValidNamespaceUri(string namespaceUri) {
                return _namespaceUriPattern.IsMatch(namespaceUri);
            }
        }
    }
}
