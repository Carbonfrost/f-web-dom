//
// Copyright 2014, 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    [Providers]
    public abstract class DomProviderFactory : IDomDocumentFactory {

        [DomProviderFactoryUsage(Extensions = ".xml")]
        public static readonly DomProviderFactory Default
            = new DefaultDomProviderFactory();

        private DomElementQuery _elementQueryCache;
        private DomObjectQuery _objectQueryCache;

        internal static readonly Assembly THIS_ASSEMBLY = typeof(DefaultDomProviderFactory).GetTypeInfo().Assembly;
        static readonly HashSet<Type> PROVIDER_TYPES = new HashSet<Type> {
            typeof(DomNode),
            typeof(DomElement),
            typeof(DomProcessingInstruction),
            typeof(DomDocument),
            typeof(DomDocumentFragment),
            typeof(DomText),
            typeof(DomCharacterData),
            typeof(DomCDataSection),
            typeof(DomComment),
            typeof(DomContainer),
            typeof(DomEntity),
            typeof(DomEntityReference),
            typeof(DomNotation),
            typeof(DomEscaper),

            typeof(DomAttributeDefinition),
            typeof(DomElementDefinition),

            typeof(DomWriter),
            typeof(DomReader),
            typeof(DomWriterSettings),
            typeof(DomReaderSettings),

            typeof(DomSelector),
            typeof(CssSelector),
            typeof(DomElementQuery),
            typeof(DomObjectQuery),

            // TODO This is an incomplete list, possible DomNode/DomContainer shouldn't be used since non-leaf types
        };

        public virtual IDomNodeTypeProvider NodeTypeProvider {
            get {
                return DomNodeTypeProvider.Default;
            }
        }

        internal DomObjectQuery EmptyObjectQuery {
            get {
                if (_objectQueryCache == null) {
                    _objectQueryCache = CreateObjectQuery(Array.Empty<DomObject>());
                }
                return _objectQueryCache;
            }
        }

        internal DomElementQuery EmptyElementQuery {
            get {
                if (_elementQueryCache == null) {
                    _elementQueryCache = CreateElementQuery(Array.Empty<DomElement>());
                }
                return _elementQueryCache;
            }
        }

        private static IEnumerable<DomProviderFactory> All {
            get {
                return App.GetProviders<DomProviderFactory>();
            }
        }

        public static DomProviderFactory FromName(string name) {
            return App.GetProvider<DomProviderFactory>(name);
        }

        public static DomProviderFactory FromCriteria(object criteria) {
            return App.GetProvider<DomProviderFactory>(criteria);
        }

        internal static DomProviderFactory ForFileName(object settings, string fileName) {
            if (string.IsNullOrEmpty(fileName)) {
                throw Failure.NullOrEmptyString(nameof(fileName));
            }

            var result = (settings != null ? DomProviderFactory.ForProviderObject(settings) : null);
            var fromCriteria = FromCriteria(new {
                Extension = Path.GetExtension(fileName)
            });

            // If the one from criteria is more specific, prefer it
            if (result is DefaultDomProviderFactory) {
                return fromCriteria ?? result;
            }
            return result ?? fromCriteria ?? Default;
        }

        public static DomProviderFactory ForProviderObject(object instance) {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }
            return ForProviderObject(instance.GetType());
        }

        public virtual bool IsProviderObject(Type providerObjectType) {
            if (providerObjectType == null) {
                throw new ArgumentNullException(nameof(providerObjectType));
            }

            return ConsideredProviderObject(providerObjectType) && providerObjectType.GetTypeInfo().Assembly == GetType().GetTypeInfo().Assembly;
        }

        internal static bool ConsideredProviderObject(Type pot) {
            var providerObjectType = pot.GetTypeInfo();
            if (providerObjectType.IsEnum || providerObjectType.IsArray || providerObjectType.IsPointer || providerObjectType.IsPrimitive || providerObjectType.IsGenericParameter) {
                return false;
            }
            if (!Utility.GetAncestorTypes(pot).Any(PROVIDER_TYPES.Contains)) {
                return false;
            }

            return true;
        }

        public bool IsProviderObject(object providerObject) {
            if (providerObject == null) {
                throw new ArgumentNullException(nameof(providerObject));
            }

            return IsProviderObject(providerObject.GetType());
        }

        public static DomProviderFactory ForProviderObject(Type providerObjectType) {
            if (providerObjectType == null) {
                throw new ArgumentNullException(nameof(providerObjectType));
            }

            var e = GetProvidersByLikelihood(providerObjectType);
            return e.FirstOrDefault(t => t.IsProviderObject(providerObjectType));
        }

        static IEnumerable<DomProviderFactory> GetProvidersByLikelihood(Type providerObjectType) {
            if (providerObjectType == null) {
                throw new ArgumentNullException(nameof(providerObjectType));
            }

            if (providerObjectType.Assembly == THIS_ASSEMBLY) {
                return new [] { Default };
            }

            // TODO Some providers are checked twice (performance, rare)
            return App.GetProviders<DomProviderFactory>(
                new {
                    Assembly = providerObjectType.GetTypeInfo().Assembly,
                }
            ).Concat(All);
        }

        public IDomNodeFactory CreateNodeFactory(IDomNodeTypeProvider nodeTypeProvider) {
            return CreateDomNodeFactory(nodeTypeProvider);
        }

        protected virtual IDomNodeFactory CreateDomNodeFactory(IDomNodeTypeProvider nodeTypeProvider) {
            return new DomNodeFactory(nodeTypeProvider);
        }

        public DomWriter CreateWriter(TextWriter writer) {
            return CreateWriter(writer, null);
        }

        public DomWriter CreateWriter(TextWriter writer, DomWriterSettings settings) {
            if (writer == null) {
                throw new ArgumentNullException(nameof(writer));
            }

            return CreateDomWriter(writer, settings);
        }

        protected virtual DomWriter CreateDomWriter(TextWriter textWriter, DomWriterSettings settings) {
            return new DefaultDomWriter(textWriter, settings);
        }

        public DomNodeWriter CreateWriter(DomNode node, DomNodeWriterSettings settings) {
            return CreateDomWriter(node);
        }

        public DomNodeReader CreateReader(DomNode node) {
            return CreateDomReader(node);
        }

        protected virtual DomNodeWriter CreateDomWriter(DomNode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            return new DomNodeWriter(node);
        }

        protected virtual DomNodeReader CreateDomReader(DomNode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }

            return new DomNodeReader(node, null);
        }

        public DomReader CreateReader(string path) {
            return CreateReader(path, null);
        }

        public DomReader CreateReader(string path, DomReaderSettings settings) {
            return CreateDomReader(File.OpenText(path), settings);
        }

        public DomReader CreateReader(TextReader reader) {
            return CreateReader(reader, null);
        }

        public DomReader CreateReader(TextReader reader, DomReaderSettings settings) {
            if (reader == null) {
                throw new ArgumentNullException(nameof(reader));
            }

            return CreateDomReader(reader, settings);
        }

        protected virtual DomReader CreateDomReader(TextReader reader, DomReaderSettings settings) {
            return DomReader.Create(XmlReader.Create(reader));
        }

        public DomObjectQuery CreateObjectQuery(IEnumerable<DomObject> items) {
            return CreateDomObjectQuery(items);
        }

        protected virtual DomObjectQuery CreateDomObjectQuery(IEnumerable<DomObject> items) {
            return new DomObjectQuery(items);
        }

        public DomElementQuery CreateElementQuery(IEnumerable<DomElement> items) {
            return CreateDomElementQuery(items);
        }

        protected virtual DomElementQuery CreateDomElementQuery(IEnumerable<DomElement> items) {
            return new DomElementQuery(items);
        }

        public DomSelector CreateSelector(string selector) {
            return CreateDomSelector(selector);
        }

        protected virtual DomSelector CreateDomSelector(string selector) {
            return CssSelector.Parse(selector);
        }

        public DomDocument CreateDocument() {
            return CreateDomDocument();
        }

        protected virtual DomDocument CreateDomDocument() {
            return new DomDocument();
        }
    }
}
