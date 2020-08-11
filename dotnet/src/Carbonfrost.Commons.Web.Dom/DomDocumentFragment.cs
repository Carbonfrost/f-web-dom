//
// Copyright 2013, 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomDocumentFragment : DomContainer, IDomXmlLoader<DomDocumentFragment> {

        private DomDocument _implicitOwnerDocument;

        public override DomNodeType NodeType {
            get {
                return DomNodeType.DocumentFragment;
            }
        }

        public override string NodeName {
            get {
                return "#document-fragment";
            }
        }

        protected override DomAttributeCollection DomAttributes {
            get {
                return null;
            }
        }

        private protected override DomDocument DomOwnerDocument {
            get {
                var doc = _implicitOwnerDocument ?? base.DomOwnerDocument;
                if (doc == null) {
                    doc = _implicitOwnerDocument = DomProviderFactory.ForProviderObject(GetType()).CreateDocument();
                }
                return doc;
            }
        }

        public DomDocumentFragment() {
        }

        public DomDocumentFragment Load(string fileName) {
            return Load(StreamContext.FromFile(fileName));
        }

        public DomDocumentFragment Load(Uri source) {
            return Load(StreamContext.FromSource(source));
        }

        public DomDocumentFragment Load(Stream input) {
            return Load(StreamContext.FromStream(input));
        }

        public DomDocumentFragment Load(StreamContext input) {
            if (input == null) {
                throw new ArgumentNullException(nameof(input));
            }

            LoadText(input.OpenText());
            return this;
        }

        protected virtual void LoadText(TextReader input) {
            if (input == null) {
                throw new ArgumentNullException(nameof(input));
            }

            var reader = FindProviderFactory().CreateReader(input);
            reader.CopyTo(this);
        }

        public DomDocumentFragment LoadXml(string xml) {
            var settings = new XmlReaderSettings {
                ConformanceLevel = ConformanceLevel.Fragment,
            };
            using (var xr = XmlReader.Create(new StringReader(xml), settings)) {
                Load(xr);
            }
            return this;
        }

        public DomDocumentFragment Load(XmlReader reader) {
            CoreLoadXml(reader);
            return this;
        }

        public DomDocumentFragment WithSchema(DomSchema schema) {
            var result = OwnerDocument.WithSchema(schema).CreateDocumentFragment();
            CopyContentsTo(result);
            return result;
        }

        protected override DomNode CloneCore() {
            var result = OwnerDocument.CreateDocumentFragment();
            result.CopyAnnotationsFrom(AnnotationList);
            Append(CloneAll(ChildNodes));
            return result;
        }
    }
}
