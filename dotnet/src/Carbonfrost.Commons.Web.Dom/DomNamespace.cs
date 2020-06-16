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

    public class DomNamespace : IEquatable<DomNamespace>, IFormattable {

        private readonly string _namespaceUri;
        private readonly IDictionary<string, DomName> _names = new Dictionary<string, DomName>();

        private static readonly DomNamespace xml = new DomNamespace("http://www.w3.org/XML/1998/namespace");
        private static readonly DomNamespace xmlns = new DomNamespace("http://www.w3.org/2000/xmlns/");
        private static readonly DomNamespace defaultNamespace = new DomNamespace("");

        public static DomNamespace Default {
            get {
                return defaultNamespace;
            }
        }

        public bool IsDefault {
            get {
                return this == DomNamespace.Default;
            }
        }

        public string NamespaceUri {
            get {
                return _namespaceUri;
            }
        }

        public static DomNamespace Xml {
            get {
                return xml;
            }
        }

        public static DomNamespace Xmlns {
            get {
                return xmlns;
            }
        }

        internal DomNamespace(string namespaceName) {
            _namespaceUri = namespaceName;
        }

        public static DomNamespace Create(string namespaceName) {
            if (string.IsNullOrEmpty(namespaceName)) {
                return Default;
            }

            return Parse(namespaceName);
        }

        public static DomNamespace Create(Uri uri) {
            if (uri == null) {
                throw new ArgumentNullException(nameof(uri));
            }

            return Parse(uri.ToString());
        }

        public static bool TryParse(string text, out DomNamespace value) {
            value = _TryParse(text, true);
            return value != null;
        }

        public static DomNamespace Parse(string text) {
            return _TryParse(text, true);
        }

        internal static DomNamespace _TryParse(string text, bool throwOnError) {
            if (text == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(text));
                }
                return null;
            }
            if (text.Length == 0) {
                return DomNamespace.Default;
            }

            return new DomNamespace(text);
        }

        public DomName GetName(string localName) {
            DomName.VerifyLocalName(nameof(localName), localName);

            DomName result = null;

            if (_names.TryGetValue(localName, out result)) {
                return result;
            }

            result = new DomName.DefaultImpl(this, localName);
            _names.Add(localName, result);
            return result;
        }

        public static DomName operator +(DomNamespace ns, string localName) {
            return (ns ?? DomNamespace.Default).GetName(localName);
        }

        public static bool operator ==(DomNamespace left, DomNamespace right) {
            return object.ReferenceEquals(left, right);
        }

        [CLSCompliant(false)]
        public static implicit operator DomNamespace(string namespaceName) {
            if (namespaceName == null) {
                return null;
            }
            return Parse(namespaceName);
        }

        public static bool operator !=(DomNamespace left, DomNamespace right) {
            return !object.ReferenceEquals(left, right);
        }

        public override int GetHashCode() {
            unchecked {
                return 9 * NamespaceUri.GetHashCode();
            }
        }

        public override bool Equals(object obj) {
            return Equals(obj as DomNamespace);
        }

        public static bool Equals(DomNamespace x, DomNamespace y) {
            return Equals(x, y, DomNamespaceComparison.Default);
        }

        public static bool Equals(DomNamespace x, DomNamespace y, DomNamespaceComparison comparison) {
            return DomNamespaceComparer.Create(comparison).Equals(x, y);
        }

        public override string ToString() {
            return _namespaceUri;
        }

        public string ToString(string format, IFormatProvider formatProvider = null) {
            if (string.IsNullOrEmpty(format)) {
                format = "G";
            }

            if (format.Length > 1) {
                throw new FormatException();
            }

            switch (char.ToLowerInvariant(format[0])) {
                case 'g':
                case 'f':
                    return NamespaceUri;
                case 'b':
                    return string.Concat("{", NamespaceUri, "}");;

                default:
                    throw new FormatException();
            }
        }

        public bool Equals(DomNamespace other) {
            return Equals(this, other, DomNamespaceComparison.Default);
        }
    }
}
