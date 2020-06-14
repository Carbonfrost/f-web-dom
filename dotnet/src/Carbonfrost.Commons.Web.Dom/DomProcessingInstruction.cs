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

    public partial class DomProcessingInstruction : DomNode {

        private readonly string _target;
        private string _data;

        protected override DomAttributeCollection DomAttributes {
            get {
                return null;
            }
        }

        public string Target {
            get {
                return _target;
            }
        }

        public string Data {
            get {
                return TextContent;
            }
            set {
                TextContent = value;
            }
        }

        public override string TextContent {
            get {
                return _data ?? string.Empty;
            }
            set {
                _data = (value ?? string.Empty).Trim();
            }
        }

        protected internal DomProcessingInstruction() {
            _target = RequireFactoryGeneratedName(
                GetType(),
                (e, t) => e.GetProcessingInstructionTarget(t)
            );
        }

        protected internal DomProcessingInstruction(string target) {
            if (target == null) {
                throw new ArgumentNullException(nameof(target));
            }
            if (string.IsNullOrEmpty(target)) {
                throw Failure.EmptyString(nameof(target));
            }

            _target = target;
        }

        public sealed override DomNodeType NodeType {
            get {
                return DomNodeType.ProcessingInstruction;
            }
        }

        public sealed override string NodeName {
            get {
                return Target;
            }
        }

        public sealed override string NodeValue {
            get {
                return Data;
            }
            set {
                Data = value;
            }
        }

        public override string Prefix {
            get {
                return null;
            }
        }

        public override string LocalName {
            get {
                return null;
            }
        }

        public override DomNamespace Namespace {
            get {
                return null;
            }
        }

        protected override DomNodeCollection DomChildNodes {
            get {
                return DomNodeCollection.Empty;
            }
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
            return Target;
        }

        public DomProcessingInstruction AppendData(string data) {
            Data += data;
            return this;
        }
    }
}

