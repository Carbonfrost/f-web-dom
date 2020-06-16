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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    class DomElementPrefixResolver : IDomNamePrefixResolver {

        private readonly DomElement _element;

        public DomElementPrefixResolver(DomElement element) {
            _element = element;
        }

        public IReadOnlyDictionary<string, DomNamespace> GetPrefixBindings(DomScope scope) {
            IEnumerable<DomElement> elements;

            if (scope == DomScope.TargetAndAncestors) {
                elements = _element.AncestorsAndSelf;
            } else {
                // We don't do descendant search even if was requested
                elements = new [] { _element };
            }

            var result = new Dictionary<string, DomNamespace>();
            foreach (var e in elements) {
                foreach (var attr in e.Attributes) {
                    if (TryExtractXmlnsPrefixMapping(attr, out string prefix)) {
                        result[prefix] = attr.Value;
                    }
                }
            }
            return result;
        }

        public DomNamespace GetNamespace(string prefix, DomScope scope) {
            if (string.IsNullOrEmpty(prefix)) {
                throw Failure.NullOrEmptyString(nameof(prefix));
            }
            if (prefix == "xmlns") {
                return null;
            }

            string ns;
            if (scope == DomScope.TargetAndAncestors) {
                ns = GetNamespaceAncestors(_element, prefix);
            } else {
                // We don't do descendant search even if was requested
                ns = GetNamespaceCore(_element, prefix);
            }
            if (ns == null) {
                return null;
            }
            return DomNamespace.Create(ns);
        }

        public string GetPrefix(DomNamespace ns, DomScope scope) {
            return GetPrefixes(ns, scope).FirstOrDefault();
        }

        public IEnumerable<string> GetPrefixes(DomNamespace ns, DomScope scope) {
            if (ns is null) {
                throw new ArgumentNullException(nameof(ns));
            }

            IEnumerable<DomElement> elements;

            if (scope == DomScope.TargetAndAncestors) {
                elements = _element.AncestorsAndSelf;
            } else {
                // We don't do descendant search even if was requested
                elements = new [] { _element };
            }
            return elements.SelectMany(
                e => e.Attributes.TrySelect<DomAttribute, string>(TryExtractXmlnsPrefixMapping)
            );
        }

        public string RegisterPrefix(DomNamespace ns, string preferredPrefix) {
            if (ns is null) {
                throw new ArgumentNullException(nameof(ns));
            }
            _element.SetXmlns(preferredPrefix, ns);
            return preferredPrefix;
        }

        private static string GetNamespaceAncestors(DomElement e, string prefix) {
            string ns = null;
            while (e != null && ns == null) {
                ns = GetNamespaceCore(e, prefix);
                e = e.Parent;
            }
            return ns;
        }

        private static string GetNamespaceCore(DomElement element, string prefix) {
            if (string.IsNullOrEmpty(prefix)) {
                return element.Attribute("xmlns");
            }
            return element.Attribute("xmlns:" + prefix);
        }

        private bool TryExtractXmlnsPrefixMapping(DomAttribute a, out string prefix) {
            // xmlns attributes are never mapped to QNames, so we can assert directly
            // on the format of the attribute

            // TODO This could be re-entrant due to a.LocalName referencing
            // DomName => XmlNameContext => this resolver

            var xml = XmlNameSemantics.FromName(a.LocalName);
             if (xml.Prefix == "xmlns") {
                prefix = xml.LocalName;
                return true;
            }
            prefix = null;
            return false;
        }

    }
}
