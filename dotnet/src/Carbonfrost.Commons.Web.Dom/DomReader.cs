//
// Copyright 2013, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.IO;
using System.Xml;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomReader : DisposableObject {

        private readonly DomReaderSettings _settings;
        private readonly DomErrorCollection _errors;

        public DomErrorCollection Errors {
            get {
                return _errors;
            }
        }

        public DomReaderSettings Settings {
            get {
                return _settings;
            }
        }

        protected DomReader() : this(null) {}

        protected DomReader(DomReaderSettings settings) {
            _settings = settings ?? DomReaderSettings.Empty;
            _errors = new DomErrorCollection(_settings.MaxErrors);
        }

        public void CopyTo(DomWriter writer) {
            if (writer == null) {
                throw new ArgumentNullException(nameof(writer));
            }

            throw new NotImplementedException();
        }

        public void CopyTo(DomNode node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }
            var writer = node.FindProviderFactory().CreateWriter(node, null);
            CopyTo(writer);
        }

        public void Close() {
            Dispose(true);
        }

        public abstract DomNodeType NodeType { get; }
        public abstract string Name { get; }
        public abstract string NamespaceUri { get; }
        public abstract string Value { get; }

        public abstract int AttributeCount { get; }
        public abstract string BaseUri { get; }
        public abstract int Depth { get; }
        public abstract bool EOF { get; }

        public virtual bool HasAttributes {
            get {
                return AttributeCount > 0;
            }
        }

        public abstract bool HasValue { get; }
        public abstract bool IsEmptyElement { get; }

        public virtual string this[string name, string namespaceUri] {
            get {
                return GetAttribute(name, namespaceUri);
            }
        }

        public virtual string this[string name] {
            get {
                return GetAttribute(name);
            }
        }

        public virtual string this[int index] {
            get {
                return GetAttribute(index);
            }
        }

        public abstract string LocalName { get; }

        public abstract string Prefix { get; }

        public virtual char QuoteChar {
            get {
                return '"';
            }
        }

        public abstract DomReadState ReadState { get; }

        public abstract string GetAttribute(string name, string namespaceUri);
        public abstract string GetAttribute(string name);
        public abstract string GetAttribute(int index);

        public DomDocument ReadDocument() {
            return ReadDomDocument();
        }

        protected virtual DomDocument ReadDomDocument() {
            throw new NotImplementedException();
        }

        public static DomReader Create(TextReader reader, DomReaderSettings settings) {
            if (settings == null) {
                return Create(XmlReader.Create(reader));
            }

            var pro = DomProviderFactory.ForProviderObject(settings) ?? DomProviderFactory.Default;
            return pro.CreateReader(reader, settings);
        }

        public static DomReader Create(XmlReader reader) {
            if (reader == null) {
                throw new ArgumentNullException(nameof(reader));
            }

            throw new NotImplementedException();
        }

        public static DomReader Create(StreamContext input) {
            return Create(input, null);
        }

        public static DomReader Create(string fileName) {
            return Create(fileName, null);
        }

        public static DomReader Create(StreamContext input, DomReaderSettings settings) {
            if (input == null) {
                throw new ArgumentNullException(nameof(input));
            }
            var provider = DomProviderFactory.ForFileName(settings, Utility.LocalPath(input.Uri));
            return provider.CreateReader(input.OpenText(), settings);
        }

        public static DomReader Create(string fileName, DomReaderSettings settings) {
            if (fileName == null) {
                throw new ArgumentNullException(nameof(fileName));
            }
            return Create(StreamContext.FromFile(fileName), settings);
        }
    }

}
