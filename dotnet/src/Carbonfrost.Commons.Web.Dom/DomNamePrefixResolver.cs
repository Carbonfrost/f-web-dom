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

using System.Collections.Generic;
using System.Linq;
using System;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomNamePrefixResolver : IDomNamePrefixResolver {

        public static readonly IDomNamePrefixResolver Null = new NullImpl();

        public static IDomNamePrefixResolver ForElement(DomElement element) {
            if (element is null) {
                throw new ArgumentNullException(nameof(element));
            }

            return new DomElementPrefixResolver(element);
        }

        public abstract DomNamespace GetNamespace(string prefix, DomScope scope);

        public virtual string GetPrefix(DomNamespace ns, DomScope scope) {
            return GetPrefixes(ns, scope).FirstOrDefault();
        }

        public abstract IEnumerable<string> GetPrefixes(DomNamespace ns, DomScope scope);
        public abstract string RegisterPrefix(DomNamespace ns, string preferredPrefix);
        public abstract IReadOnlyDictionary<string, DomNamespace> GetPrefixBindings(DomScope scope);

        class NullImpl : IDomNamePrefixResolver {

            public DomNamespace GetNamespace(string prefix, DomScope scope) {
                return null;
            }

            public string GetPrefix(DomNamespace ns, DomScope scope) {
                return null;
            }

            public IReadOnlyDictionary<string, DomNamespace> GetPrefixBindings(DomScope scope) {
                return Empty<string, DomNamespace>.ReadOnlyDictionary;
            }

            public IEnumerable<string> GetPrefixes(DomNamespace ns, DomScope scope) {
                return Enumerable.Empty<string>();
            }

            public string RegisterPrefix(DomNamespace ns, string preferredPrefix) {
                throw new NotSupportedException();
            }
        }
    }
}
