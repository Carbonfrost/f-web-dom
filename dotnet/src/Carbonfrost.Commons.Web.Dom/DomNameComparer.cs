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

    public abstract class DomNameComparer : IDomNameComparer {

        public static readonly DomNameComparer Ordinal = new OrdinalImpl();
        public static readonly DomNameComparer IgnoreCase = new IgnoreCaseImpl();

        public abstract int Compare(DomName x, DomName y);
        public abstract bool Equals(DomName x, DomName y);
        public abstract int GetHashCode(DomName obj);

        public static DomNameComparer Create(DomNamespaceComparer namespaceComparer, StringComparer localNameComparer) {
            return new Impl(namespaceComparer, localNameComparer);
        }

        internal static IEqualityComparer<DomAttribute> AttributeAdapter(IEqualityComparer<DomName> c) {
            return new AttributeAdapterImpl(c);
        }

        class IgnoreCaseImpl : DomNameComparer {
            public override int Compare(DomName x, DomName y) {
                return StringComparer.OrdinalIgnoreCase.Compare(
                    Normalize(x), Normalize(y)
                );
            }

            public override bool Equals(DomName x, DomName y) {
                return StringComparer.OrdinalIgnoreCase.Equals(
                    Normalize(x), Normalize(y)
                );
            }

            public override int GetHashCode(DomName obj) {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(Normalize(obj));
            }

            private static string Normalize(DomName x) {
                var ns = Convert.ToString(x.NamespaceUri).ToLowerInvariant();
                return string.Join(" ", ns, x.LocalName.ToLowerInvariant());
            }
        }

        class OrdinalImpl : DomNameComparer {
            public override int Compare(DomName x, DomName y) {
                int result = StringComparer.Ordinal.Compare(x.NamespaceUri, y.NamespaceUri);
                if (result == 0) {
                    result = StringComparer.Ordinal.Compare(x.LocalName, y.LocalName);
                }
                return result;
            }

            public override bool Equals(DomName x, DomName y) {
                return x.NamespaceUri == y.NamespaceUri &&
                    x.LocalName == y.LocalName;
            }

            public override int GetHashCode(DomName obj) {
                int hashCode = -1450740237;
                hashCode = hashCode * -1521134295 + obj.NamespaceUri.GetHashCode();
                hashCode = hashCode * -1521134295 + obj.LocalName.GetHashCode();
                return hashCode;
            }
        }

        private class AttributeAdapterImpl : IEqualityComparer<DomAttribute> {
            private readonly IEqualityComparer<DomName> _comparer;

            public AttributeAdapterImpl(IEqualityComparer<DomName> c) {
                _comparer = c;
            }

            public bool Equals(DomAttribute x, DomAttribute y) {
                if (x is null && y is null) {
                    return true;
                }
                return x != null && y != null && _comparer.Equals(x.Name, y.Name);
            }

            public int GetHashCode(DomAttribute obj) {
                if (obj == null) {
                    return -2;
                }
                return _comparer.GetHashCode(obj.Name);
            }
        }

        private class Impl : DomNameComparer {
            private readonly DomNamespaceComparer _namespaceComparer;
            private readonly StringComparer _localNameComparer;

            public Impl(DomNamespaceComparer namespaceComparer, StringComparer localNameComparer) {
                _namespaceComparer = namespaceComparer;
                _localNameComparer = localNameComparer;
            }

            public override int Compare(DomName x, DomName y) {
                int result = _namespaceComparer.Compare(x.Namespace, y.Namespace);
                if (result != 0) {
                    return result;
                }
                return _localNameComparer.Compare(x.LocalName, y.LocalName);
            }

            public override bool Equals(DomName x, DomName y) {
                return _namespaceComparer.Equals(x.Namespace, y.Namespace)
                    && _localNameComparer.Equals(x.LocalName, y.LocalName);
            }

            public override int GetHashCode(DomName obj) {
                if (obj == null) {
                    return -1;
                }
                int hashCode = -1459834297;
                hashCode = hashCode * -1521134295 + _namespaceComparer.GetHashCode(obj.Namespace);
                hashCode = hashCode * -1521134295 + _localNameComparer.GetHashCode(obj.LocalName);
                return hashCode;
            }
        }
    }
}
