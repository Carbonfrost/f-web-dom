//
// - DomReader.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.IO;
using System.Xml;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomReader : DisposableObject {

        readonly DomReaderSettings settings;

        public DomReaderSettings Settings {
            get {
                return settings;
            }
        }

        protected DomReader(DomReaderSettings settings) {
            this.settings = settings ?? DomReaderSettings.Empty;
        }

        public void CopyTo(DomWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            throw new NotImplementedException();
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
                return this.GetAttribute(name, namespaceUri);
            }
        }

        public virtual string this[string name] {
            get {
                return this.GetAttribute(name);
            }
        }

        public virtual string this[int i] {
            get {
                return this.GetAttribute(i);
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

        public static DomReader Create(TextReader reader, DomReaderSettings settings) {
            if (settings == null)
                return CreateXml(reader);

            var pro = DomProviderFactory.ForProviderObject(settings);

            if (pro == null)
                return CreateXml(reader);

            return pro.CreateReader(reader, settings);
        }

        public static DomReader CreateXml(TextReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return CreateXml(XmlReader.Create(reader));
        }

        public static DomReader CreateXml(XmlReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader");

            throw new NotImplementedException();
        }
    }

}
