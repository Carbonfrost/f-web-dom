//
// Copyright 2014, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
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
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    [Providers]
    public abstract class DomProviderFactory {

        public static readonly DomProviderFactory Default
            = new DefaultDomProviderFactory();

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

            typeof(DomAttributeDefinition),
            typeof(DomElementDefinition),

            typeof(DomWriter),
            typeof(DomReader),

            // TODO This is an incomplete list, possible DomNode/DomContainer shouldn't be used since non-leaf types
        };

        internal static IEnumerable<DomProviderFactory> All {
            get {
                return App.GetProviders<DomProviderFactory>();
            }
        }

        public virtual IDomNodeFactory NodeFactory {
            get {
                return DomNodeFactory.Default;
            }
        }

        public static DomProviderFactory FromName(string name) {
            return App.GetProvider<DomProviderFactory>(name);
        }

        public static DomProviderFactory ForProviderObject(object instance) {
            if (instance == null) {
                throw new ArgumentNullException("instance");
            }

            var e = GetProvidersByLikelihood(instance.GetType());
            return e.FirstOrDefault(t => t.IsProviderObject(instance));
        }

        public virtual bool IsProviderObject(Type providerObjectType) {
            if (providerObjectType == null) {
                throw new ArgumentNullException("providerObjectType");
            }

            return ConsideredProviderObject(providerObjectType) && providerObjectType.GetTypeInfo().Assembly == GetType().GetTypeInfo().Assembly;
        }

        public virtual string GenerateDefaultName(Type providerObjectType) {
            return null;
        }

        internal static bool ConsideredProviderObject(Type pot) {
            var providerObjectType = pot.GetTypeInfo();
            if (providerObjectType.IsEnum || providerObjectType.IsArray || providerObjectType.IsPointer || providerObjectType.IsPrimitive || providerObjectType.IsGenericParameter)
                return false;
            if (!Utility.GetAncestorTypes(pot).Any(PROVIDER_TYPES.Contains))
                return false;

            return true;
        }

        public bool IsProviderObject(object providerObject) {
            if (providerObject == null)
                throw new ArgumentNullException("providerObject");

            return IsProviderObject(providerObject.GetType());
        }

        public static DomProviderFactory ForProviderObject(Type providerObjectType) {
            if (providerObjectType == null)
                throw new ArgumentNullException("providerObjectType");

            var e = GetProvidersByLikelihood(providerObjectType);
            return e.FirstOrDefault(t => t.IsProviderObject(providerObjectType));
        }

        static IEnumerable<DomProviderFactory> GetProvidersByLikelihood(Type providerObjectType) {
            // TODO Some providers are checked twice (performance, rare)
            return App.GetProviders<DomProviderFactory>(
                new {
                    Assembly = providerObjectType.GetTypeInfo().Assembly,
                }).Concat(App.GetProviders<DomProviderFactory>());
        }

        public DomNodeWriter CreateWriter(DomNode node, DomNodeWriterSettings settings) {
            return CreateDomWriter(node);
        }

        public DomNodeReader CreateReader(DomNode node) {
            return CreateDomReader(node);
        }

        protected virtual DomNodeWriter CreateDomWriter(DomNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            return new DomNodeWriter(node);
        }

        protected virtual DomNodeReader CreateDomReader(DomNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

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
            if (reader == null)
                throw new ArgumentNullException("reader");

            return CreateDomReader(reader, settings);
        }

        protected virtual DomReader CreateDomReader(TextReader reader, DomReaderSettings settings) {
            return DomReader.CreateXml(reader);
        }
    }
}
