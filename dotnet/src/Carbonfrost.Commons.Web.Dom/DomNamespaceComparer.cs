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

    public abstract class DomNamespaceComparer : IComparer<DomNamespace>, IEqualityComparer<DomNamespace> {

        public static readonly DomNamespaceComparer Default = new DefaultImpl();
        public static readonly DomNamespaceComparer Ordinal = new OrdinalImpl();

        public static DomNamespaceComparer Create(DomNamespaceComparison comparison) {
            if (comparison == DomNamespaceComparison.Default) {
                return Default;
            }
            return Ordinal;
        }

        public abstract int Compare(DomNamespace x, DomNamespace y);
        public abstract bool Equals(DomNamespace x, DomNamespace y);
        public abstract int GetHashCode(DomNamespace obj);

        public class OrdinalImpl : DomNamespaceComparer {

            public override int Compare(DomNamespace x, DomNamespace y) {
                if (x is null || y is null) {
                    return Comparer<object>.Default.Compare(x, y);
                }
                return StringComparer.Ordinal.Compare(x.NamespaceUri, y.NamespaceUri);
            }

            public override bool Equals(DomNamespace x, DomNamespace y) {
                if (x is null || y is null) {
                    return ReferenceEquals(x, y);
                }
                return StringComparer.Ordinal.Equals(x.NamespaceUri, y.NamespaceUri);
            }

            public override int GetHashCode(DomNamespace obj) {
                if (obj is null) {
                    return -1;
                }
                return StringComparer.Ordinal.GetHashCode(obj.NamespaceUri);
            }

        }

        public class DefaultImpl : DomNamespaceComparer {

            public override int Compare(DomNamespace x, DomNamespace y) {
                if (x is null || y is null) {
                    return Comparer<object>.Default.Compare(x, y);
                }
                return string.Compare(
                    NormalizeUri(x), NormalizeUri(y), StringComparison.OrdinalIgnoreCase
                );
            }

            public override bool Equals(DomNamespace x, DomNamespace y) {
                return Compare(x, y) == 0;
            }

            public override int GetHashCode(DomNamespace obj) {
                return NormalizeUri(obj).GetHashCode();
            }

            private static string NormalizeUri(DomNamespace ns) {
                if (ns == null) {
                    return null;
                }
                string s = ns.NamespaceUri;
                s = Regex.Replace(s, "^(http://)", @"https://");
                s = Regex.Replace(s, "/$", "");

                if (!s.StartsWith("https://")) {
                    return "https://" + s;
                }
                return s;
            }
        }
    }
}
