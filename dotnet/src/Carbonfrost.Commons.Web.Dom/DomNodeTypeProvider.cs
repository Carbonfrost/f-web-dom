//
// Copyright 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

namespace Carbonfrost.Commons.Web.Dom {

    public class DomNodeTypeProvider : IDomNodeTypeProvider {

        public static readonly IDomNodeTypeProvider Default = new DomNodeTypeProvider();

        public virtual Type GetAttributeNodeType(string name) {
            return typeof(DomAttribute);
        }

        public virtual Type GetElementNodeType(string name) {
            return typeof(DomElement);
        }

        public virtual Type GetProcessingInstructionNodeType(string name) {
            return typeof(DomProcessingInstruction);
        }

        public virtual Type GetTextNodeType() {
            return typeof(DomText);
        }

        public virtual Type GetCommentNodeType() {
            return typeof(DomComment);
        }

        public virtual Type GetNotationNodeType(string name) {
            return typeof(DomNotation);
        }

        public virtual Type GetEntityReferenceNodeType(string name) {
            return typeof(DomEntityReference);
        }

        public virtual Type GetEntityNodeType(string name) {
            return typeof(DomEntity);
        }

        public virtual Type GetCDataSectionNodeType() {
            return typeof(DomCDataSection);
        }

        public virtual Type GetDocumentFragmentNodeType() {
            return typeof(DomDocumentFragment);
        }

        public virtual Type GetDocumentTypeNodeType(string name) {
            return typeof(DomDocumentType);
        }
    }
}


