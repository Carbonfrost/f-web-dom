//
// - XmlDomReader.cs -
//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using System.Xml;

namespace Carbonfrost.Commons.Web.Dom {

    class XmlDomReader : DomReader {

        // TODO XmlDomReader

        readonly XmlReader xml;

        public XmlDomReader(XmlReader xml,
                            DomReaderSettings settings) : base(settings) {
            this.xml = xml;
        }

        public override string GetAttribute(string name, string namespaceUri) {
            return xml.GetAttribute(name, namespaceUri);
        }

        public override string GetAttribute(string name) {
            return xml.GetAttribute(name);
        }

        public override string GetAttribute(int index) {
            return xml.GetAttribute(index);
        }

        public override DomNodeType NodeType {
            get {
                throw new NotImplementedException();
            }
        }

        public override string Name {
            get {
                return xml.Name;
            }
        }

        public override string NamespaceUri {
            get {
                return xml.NamespaceURI;
            }
        }

        public override string Value {
            get {
                return xml.Value;
            }
        }

        public override int AttributeCount {
            get {
                return xml.AttributeCount;
            }
        }

        public override string BaseUri {
            get {
                throw new NotImplementedException();
            }
        }

        public override int Depth {
            get {
                throw new NotImplementedException();
            }
        }

        public override bool EOF {
            get {
                return xml.EOF;
            }
        }

        public override bool HasValue {
            get {
                throw new NotImplementedException();
            }
        }

        public override bool IsEmptyElement {
            get {
                throw new NotImplementedException();
            }
        }

        public override string LocalName {
            get {
                return xml.LocalName;
            }
        }

        public override string Prefix {
            get {
                return xml.Prefix;
            }
        }

        public override DomReadState ReadState {
            get {
                throw new NotImplementedException();
            }
        }
    }
}
