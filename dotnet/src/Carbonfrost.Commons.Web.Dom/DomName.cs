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
using System.Globalization;
using System.Text.RegularExpressions;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract partial class DomName : IEquatable<DomName>, IComparable<DomName>, IFormattable, IDomNameApiConventions {

        public virtual string Prefix {
            get {
                return null;
            }
        }

        public abstract string LocalName {
            get;
        }

        public abstract DomNamespace Namespace {
            get;
        }

        public string NamespaceUri {
            get {
                if (Namespace == null) {
                    return null;
                }
                return Namespace.NamespaceUri;
            }
        }

        DomName IDomNameApiConventions.Name {
            get {
                return this;
            }
        }

        internal virtual XmlNameSemantics? TryXmlSemantics() {
            return null;
        }

        internal DomName Resolve(DomNameContext nameContext) {
            if (nameContext == null) {
                return this;
            }
            return nameContext.GetName(this);
        }

        public virtual DomName WithLocalName(string value) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            if (value.Length == 0) {
                throw Failure.EmptyString(nameof(value));
            }
            return Namespace + value;
        }

        public virtual DomName WithPrefix(string prefix) {
            return new QName(prefix, this);
        }

        public DomName WithNamespace(DomNamespace value) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            return value + LocalName;
        }

        public static DomName Create(DomNamespace namespaceUri, string name) {
            return (namespaceUri ?? DomNamespace.Default) + name;
        }

        public static DomName Create(string name) {
            if (string.IsNullOrEmpty(name)) {
                throw Failure.NullOrEmptyString(nameof(name));
            }

            return new SimpleName(name);
        }

        public static DomName Create(Uri namespaceUri, string localName) {
            DomNamespace nu = DomNamespace.Default;
            if (namespaceUri != null) {
                nu = DomNamespace.Create(namespaceUri);
            }
            return nu + localName;
        }

        public static DomName Create(string namespaceUri, string localName) {
            DomNamespace nu = DomNamespace.Default;
            if (namespaceUri != null) {
                nu = DomNamespace.Create(namespaceUri);
            }
            return nu + localName;
        }

        public static bool TryParse(string text, out DomName result) {
            return _TryParse(text, out result) == null;
        }

        public static DomName Parse(string text) {
            DomName result;
            var ex = _TryParse(text, out result);
            if (ex != null) {
                throw ex;
            }
            return result;
        }

        static Exception _TryParse(string text, out DomName result) {
            result = null;

            if (string.IsNullOrEmpty(text)) {
                throw Failure.NullOrEmptyString(nameof(text));
            }

            if (text[0] == '{') {
                int num = text.LastIndexOf('}');

                if ((num <= 1) || (num == (text.Length - 1))) {
                    return Failure.NotParsable(nameof(text), typeof(DomName));
                }

                if (num - 1 == 0) {
                    // The default namespace is used (as in '{} expandedName')
                    result = DomNamespace.Default.GetName(text.Trim());
                    return null;

                } else {
                    // Some other namespace is used
                    string ns = text.Substring(1, num - 1);
                    string localName = text.Substring(num + 1).Trim();

                    DomNamespace nu = DomNamespace._TryParse(ns, false);
                    if (nu == null) {
                        return Failure.NotParsable(nameof(text), typeof(DomName));
                    }
                    result = nu.GetName(localName);
                    return null;
                }

            }

            if (!text.Contains(":")) {
                result = DomNamespace.Default.GetName(text.Trim());
                return null;
            }

            return Failure.NotParsable(nameof(text), typeof(DomName));
        }

        public static bool operator ==(DomName left, DomName right) {
            return StaticEquals(left, right);
        }

        public static bool operator !=(DomName left, DomName right) {
            return !StaticEquals(left, right);
        }

        public static implicit operator DomName(string name) {
            return Parse(name);
        }

        public override bool Equals(object obj) {
            return Equals(obj as DomName);
        }

        public override int GetHashCode() {
            unchecked {
                var result = 1000000009 * LocalName.GetHashCode();
                result += 1000000021 * Namespace.GetHashCode();
                return result;
            }
        }

        public override string ToString() {
            return FullName();
        }

        internal virtual string PrefixFormat() {
            return LocalName;
        }

        string FullName() {
            if (Namespace.IsDefault) {
                return LocalName;
            }
            return string.Concat("{", Namespace.NamespaceUri, "} ",LocalName);
        }

        public string ToString(string format) {
            return ToString(format, CultureInfo.InvariantCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider) {
            if (string.IsNullOrEmpty(format)) {
                return FullName();
            }

            if (format.Length > 1) {
                throw new FormatException();
            }

            switch (char.ToUpperInvariant(format[0])) {
                case 'G':
                case 'F':
                    return FullName();
                case 'S':
                    return LocalName;
                case 'P':
                    return PrefixFormat();
                case 'N':
                    return Namespace.NamespaceUri;
                case 'M':
                    return string.Concat("{", Namespace.NamespaceUri, "}");
                default:
                    throw new FormatException();
            }
        }

        internal static bool StaticEquals(DomName a, DomName b) {
            if (object.ReferenceEquals(a, b)) {
                return true;
            }
            if (object.ReferenceEquals(a, null)
                || object.ReferenceEquals(b, null)) {
                return false;
            }
            return a.LocalName == b.LocalName
                && object.Equals(a.Namespace, b.Namespace);
        }

        public bool Equals(DomName other) {
            return StaticEquals(this, other);
        }

        internal static void VerifyLocalName(string argName, string value) {
            if (value == null) {
                throw new ArgumentNullException(argName);
            }
            if (value.Length == 0) {
                throw Failure.EmptyString(argName);
            }
            if (!Regex.IsMatch(value, @"\A#?[a-z]\S*\Z", RegexOptions.IgnoreCase)) {
                throw DomFailure.NotValidLocalName(argName);
            }
        }

        public int CompareTo(DomName other) {
            if (other == null) {
                return 1;
            }
            return string.Compare(FullName(), other.FullName(), StringComparison.Ordinal);
        }

        internal bool EqualsIgnoreCase(DomName name) {
            return this == name
                || (Namespace == name.Namespace && string.Equals(LocalName, name.LocalName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
