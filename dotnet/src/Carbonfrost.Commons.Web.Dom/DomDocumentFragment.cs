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

    public partial class DomDocumentFragment : DomContainer, IDomDocumentLoaderApiConventions {

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

        protected internal DomDocumentFragment() {
        }

        public void Load(string fileName) {
            Load(StreamContext.FromFile(fileName));
        }

        public void Load(Uri source) {
            Load(StreamContext.FromSource(source));
        }

        public void Load(Stream input) {
            Load(StreamContext.FromStream(input));
        }

        public void Load(StreamContext input) {
            if (input == null) {
                throw new ArgumentNullException("input");
            }

            LoadText(input.OpenText());
        }

        protected virtual void LoadText(TextReader input) {
            if (input == null) {
                throw new ArgumentNullException("input");
            }

            var reader = OwnerDocument.ProviderFactory.CreateReader(input);

            // TODO By default, use document semantics on loading
            throw new NotImplementedException();
        }

        public void LoadXml(string xml) {
            var settings = new XmlReaderSettings {
                ConformanceLevel = ConformanceLevel.Fragment,
            };
            using (var xr = XmlReader.Create(new StringReader(xml), settings)) {
                Load(xr);
            }
        }

        public void Load(XmlReader reader) {
            CoreLoadXml(reader);
        }
    }
}
