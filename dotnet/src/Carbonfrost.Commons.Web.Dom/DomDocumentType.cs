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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomDocumentType : DomNode {

        private readonly string _name;
        // TODO These should be read-only like w3c

        public string PublicId {
            get;
            set;
        }

        public string SystemId {
            get;
            set;
        }

        protected internal DomDocumentType(string name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrEmpty(name)) {
                throw Failure.EmptyString(nameof(name));
            }

            _name = name;
        }

        public override DomNodeType NodeType {
            get {
                return DomNodeType.DocumentType;
            }
        }

        public new string Name {
            get {
                return _name;
            }
        }

        public override string NodeName {
            get {
                return Name;
            }
        }

        protected override DomAttributeCollection DomAttributes {
            get {
                return null;
            }
        }

        protected override DomNodeCollection DomChildNodes {
            get {
                return DomNodeCollection.Empty;
            }
        }

        public override DomNameContext NameContext {
            get {
                return ParentElement.NameContext;
            }
            set {
                throw new NotSupportedException();
            }
        }

        protected override DomNode CloneCore() {
            var result = OwnerDocument.CreateDocumentType(NodeName);
            result.CopyAnnotationsFrom(AnnotationList);
            return result;
        }
    }
}
