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

    public class DomName : IEquatable<DomName>, IComparable<DomName>, IFormattable, IDomNameApiConventions {

        static readonly Regex VALID_NAME = new Regex(@"\A[A-Za-z0-9._:-]+\Z");
        private readonly string _localName;
        private readonly DomNamespace _ns;

        public string LocalName {
            get {
                return _localName;
            }
        }

        public DomNamespace Namespace {
            get {
                return _ns;
            }
        }

        public string NamespaceUri {
            get {
                return _ns.NamespaceUri;
            }
        }

        DomName IDomNameApiConventions.Name {
            get {
                return this;
            }
        }

        internal DomName(DomNamespace ns, string localName) {
            _ns = ns;
            _localName = localName;
        }

        public DomName ChangeLocalName(string value) {
            VerifyLocalName(nameof(value), value);
            return Namespace + value;
        }

        public DomName ChangeNamespace(DomNamespace value) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            return value + LocalName;
        }

        public static DomName Create(DomNamespace namespaceUri, string name) {
            return (namespaceUri ?? DomNamespace.Default) + name;
        }

        public static DomName Create(string name) {
            return Create(DomNamespace.Default, name);
        }

        public static DomName Create(Uri namespaceUri, string name) {
            DomNamespace nu = DomNamespace.Default;
            if (namespaceUri != null) {
                nu = DomNamespace.Create(namespaceUri);
            }

            return nu + name;
        }

        public static bool TryParse(string text, IServiceProvider serviceProvider, out DomName result) {
            return _TryParse(text, serviceProvider, out result) == null;
        }

        public static bool TryParse(string text, out DomName result) {
            return _TryParse(text, null, out result) == null;
        }

        public static DomName Parse(string text) {
            DomName result;
            var ex = _TryParse(text, ServiceProvider.Null, out result);
            if (ex != null) {
                throw ex;
            }
            return result;
        }

        public static DomName Parse(string text, IServiceProvider serviceProvider) {
            DomName result;
            var ex = _TryParse(text, serviceProvider, out result);
            if (ex != null) {
                throw ex;
            }
            return result;
        }

        static Exception _TryParse(string text, IServiceProvider serviceProvider, out DomName result) {
            serviceProvider = serviceProvider ?? ServiceProvider.Null;
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

        public static DomName Create(string namespaceUri, string localName) {
            return DomNamespace.Parse(namespaceUri).GetName(localName);
        }

        public static bool operator ==(DomName left, DomName right) {
            return StaticEquals(left, right);
        }

        public static bool operator !=(DomName left, DomName right) {
            return !StaticEquals(left, right);
        }

        public override bool Equals(object obj) {
            return Equals(obj as DomName);
        }

        public override int GetHashCode() {
            unchecked {
                var result = 1000000009 * _localName.GetHashCode();
                result += 1000000021 * _ns.GetHashCode();
                return result;
            }
        }

        public override string ToString() {
            return FullName();
        }

        string FullName() {
            if (_ns.IsDefault) {
                return _localName;
            }
            return string.Concat("{", _ns.NamespaceUri, "} ", _localName);
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
                    return _localName;
                case 'N':
                    return _ns.NamespaceUri;
                case 'M':
                    return string.Concat("{", _ns.NamespaceUri, "}");
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
            return a._localName == b._localName
                && object.Equals(a._ns, b._ns);
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
            if (!VALID_NAME.IsMatch(value)) {
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
