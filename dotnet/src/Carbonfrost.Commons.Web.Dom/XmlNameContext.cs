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
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    class XmlNameContext : DomNameContext, IDomObjectReferenceLifecycle, IDomNamePrefixResolver {

        private DomObject _node;
        private DomObserver _observer;
        private IDomNamePrefixResolver _resolverCache;

        private IDomNamePrefixResolver Resolver {
            get {
                if (_node == null || !(_node is DomElement)) {
                    return DomNamePrefixResolver.Null;
                }
                if (_resolverCache == null) {
                    _resolverCache = DomNamePrefixResolver.ForElement(_node as DomElement);
                }
                return _resolverCache;
            }
        }

        internal DomContainer Container {
            get {
                if (_node == null) {
                    return null;
                }
                return _node.ContainerOrSelf;
            }
        }

        public void Attaching(DomObject instance) {
            try {
                AttachingCore(instance);
            } finally {
                _node = instance;
            }
        }

        public void Detaching() {
            try {
                _observer.Dispose();
            } finally {
                _node = null;
            }
        }

        object IDomObjectReferenceLifecycle.Clone() {
            return Clone();
        }

        public XmlNameContext Clone() {
            return new XmlNameContext();
        }

        public override bool IsValidLocalName(string name) {
            return DomNameContext.Default.IsValidLocalName(name);
        }

        public override bool IsValidNamespaceUri(string namespaceUri) {
            return DomNameContext.Default.IsValidNamespaceUri(namespaceUri);
        }

        private void AttachingCore(DomObject instance) {
            if (!(instance is DomContainer)) {
                return;
            }

            var doc = (instance is DomDocument dd) ? dd : instance.OwnerDocument;

            // Special case: monitor the document node to add it to the document element
            // if the document element is later added
            DomObserver docElement = null;
            if (instance is DomDocument document) {
                docElement = doc.ObserveChildNodes((DomNode) instance, AddNamespaceToDocumentElement, DomScope.Target);
                if (document.DocumentElement != null) {
                    document.DocumentElement.NameContext = new XmlNameContext();
                }
            }

            _observer = DomObserver.Compose(
                doc.ObserveAttributes((DomNode) instance, FixupNamespace, DomScope.TargetAndDescendants),
                docElement
            );
        }

        public override DomName GetName(DomName name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(name.Prefix) && name.Namespace != null && !name.Namespace.IsDefault) {
                var prefix = Resolver.GetPrefix(name.Namespace, DomScope.TargetAndAncestors);
                if (prefix != null) {
                    return new DomName.QName(prefix, name);
                }
            }

            if (name.TryXmlSemantics() is XmlNameSemantics xml) {
                if (xml.Prefix == "xmlns") {
                    return name;
                }

                var ns = Resolver.GetNamespace(xml.Prefix, DomScope.TargetAndAncestors);
                if (ns != null) {
                    return new DomName.QName(xml.Prefix, ns + xml.LocalName);
                }

                return new DomName.QName(
                    xml.Prefix,
                    new DomNamespace($"urn:unbound-xmlns:{xml.Prefix}") + xml.LocalName
                );
            }
            return name;
        }

        private void FixupNamespace(DomAttributeEvent evt) {
            if (!(evt.LocalName.StartsWith("xmlns:") || evt.LocalName == "xmlns")) {
                return;
            }

            if (evt.Target.ActualNameContext == null) {
                evt.Target.NameContext = new XmlNameContext();
            }
        }

        private void AddNamespaceToDocumentElement(DomMutationEvent evt) {
            if (evt.AddedNodes.Count > 0 && evt.AddedNodes[0] is DomElement e && e.IsDocumentElement && e.ActualNameContext == null) {
                e.NameContext = new XmlNameContext();
            }
        }

        public DomNamespace GetNamespace(string prefix, DomScope scope) {
            return Resolver.GetNamespace(prefix, scope);
        }

        public IEnumerable<string> GetPrefixes(DomNamespace ns, DomScope scope) {
            return Resolver.GetPrefixes(ns, scope);
        }

        public string GetPrefix(DomNamespace ns, DomScope scope) {
            return Resolver.GetPrefix(ns, scope);
        }

        public string RegisterPrefix(DomNamespace ns, string preferredPrefix) {
            return Resolver.RegisterPrefix(ns, preferredPrefix);
        }

        public IReadOnlyDictionary<string, DomNamespace> GetPrefixBindings(DomScope scope) {
            return Resolver.GetPrefixBindings(scope);
        }
    }

    partial class Extensions {

        public static DomElement SetDefaultXmlns(this DomElement self, DomNamespace ns) {
            if (self is null) {
                throw new ArgumentNullException(nameof(self));
            }

            return self.Attribute("xmlns", ns);
        }

        public static DomElement SetXmlns(this DomElement self, string prefix, DomNamespace ns) {
            if (self is null) {
                throw new ArgumentNullException(nameof(self));
            }
            if (ns is null) {
                throw new ArgumentNullException(nameof(ns));
            }
            if (string.IsNullOrEmpty(prefix)) {
                throw new NotImplementedException();
            }
            return self.Attribute($"xmlns:{prefix}", ns);
        }

        public static void UseXmlNameSemantics(this DomContainer self) {
            if (self is null) {
                throw new ArgumentNullException(nameof(self));
            }
            self.NameContext = DomNameContext.Xml;
        }
    }
}
