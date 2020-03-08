//
// - DomProcessingInstruction.cs -
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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomProcessingInstruction : DomNode {

        private readonly string target;
        private string data;

        protected override DomAttributeCollection DomAttributes {
            get {
                return null;
            }
        }

        public string Target {
            get {
                return target;
            }
        }

        public string Data {
            get {
                return data ?? string.Empty;
            }
            set {
                data = (value ?? string.Empty).Trim();
            }
        }

        public override string TextContent {
            get {
                return this.Data;
            }
            set {
                this.Data = value;
            }
        }

        protected internal DomProcessingInstruction() {
            this.target = RequireFactoryGeneratedName(GetType());
        }

        protected internal DomProcessingInstruction(string target) {
            if (target == null)
                throw new ArgumentNullException("target");
            if (string.IsNullOrEmpty(target))
                throw Failure.EmptyString("target");

            this.target = target;
        }

        public override DomNodeType NodeType {
            get {
                return DomNodeType.ProcessingInstruction;
            }
        }

        public override string NodeName {
            get {
                return Target;
            }
        }

        public override string NodeValue {
            get {
                return Data;
            }
            set {
                Data = value;
            }
        }

        public override string Prefix {
            get { return null; }
        }

        public override string LocalName {
            get { return null; }
        }

        public override string NamespaceUri {
            get { return null; }
        }

        protected override DomNodeCollection DomChildNodes {
            get { return DomNodeCollection.Empty; }
        }

        public DomProcessingInstructionDefinition ProcessingInstructionDefinition {
            get {
                return DomProcessingInstructionDefinition;
            }
        }

        protected virtual DomProcessingInstructionDefinition DomProcessingInstructionDefinition {
            get {
                if (OwnerDocument == null || OwnerDocument.Schema == null) {
                    return new DomProcessingInstructionDefinition(Target);
                }
                return OwnerDocument.Schema.GetProcessingInstructionDefinition(Target);
            }
        }

        public override string ToString() {
            return this.TextContent;
        }

        internal override void AcceptVisitor(IDomNodeVisitor visitor) {
            visitor.Visit(this);
        }

        public void AppendData(string data) {
            this.Data += data;
        }

        internal override TResult AcceptVisitor<TArgument, TResult>(IDomNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            return visitor.Visit(this, argument);
        }
    }
}

