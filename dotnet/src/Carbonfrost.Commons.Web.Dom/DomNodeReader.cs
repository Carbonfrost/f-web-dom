//
// - DomNodeReader.cs -
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

namespace Carbonfrost.Commons.Web.Dom {

    public class DomNodeReader : DomReader {

        private DomNode node;

        public new DomNodeReaderSettings Settings {
            get {
                return (DomNodeReaderSettings) base.Settings;
            }
        }

        public DomNodeReader(DomNode node, DomNodeReaderSettings settings)
            : base(settings) {

            this.node = node;
        }

        public override string GetAttribute(string name, string namespaceUri) {
            throw new NotImplementedException();
        }

        public override string GetAttribute(string name) {
            throw new NotImplementedException();
        }

        public override string GetAttribute(int index) {
            throw new NotImplementedException();
        }
        public override DomNodeType NodeType {
            get {
                throw new NotImplementedException();
            }
        }

        public override string Name {
            get {
                throw new NotImplementedException();
            }
        }

        public override string NamespaceUri {
            get {
                throw new NotImplementedException();
            }
        }
        public override string Value {
            get {
                throw new NotImplementedException();
            }
        }
        public override int AttributeCount {
            get {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
        }

        public override string Prefix {
            get {
                throw new NotImplementedException();
            }
        }

        public override DomReadState ReadState {
            get {
                throw new NotImplementedException();
            }
        }
    }
}
