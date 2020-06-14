//
// Copyright 2014, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Collections.Generic;

namespace Carbonfrost.Commons.Web.Dom {

    class CompositeDomNodeFactory : IDomNodeFactory, IEnumerable<IDomNodeFactory> {

        readonly IDomNodeFactory[] items;

        public CompositeDomNodeFactory(IDomNodeFactory[] items) {
            this.items = items;
        }

        public DomAttribute CreateAttribute(DomName name) {
            return items.FirstNonNull(t => t.CreateAttribute(name));
        }

        public DomCDataSection CreateCDataSection() {
            return items.FirstNonNull(t => t.CreateCDataSection());
        }

        public DomText CreateText() {
            return items.FirstNonNull(t => t.CreateText());
        }

        public DomDocumentType CreateDocumentType(string name) {
            return items.FirstNonNull(t => t.CreateDocumentType(name));
        }

        public DomElement CreateElement(DomName name) {
            return items.FirstNonNull(t => t.CreateElement(name));
        }

        public DomComment CreateComment() {
            return items.FirstNonNull(t => t.CreateComment());
        }

        public DomEntityReference CreateEntityReference(string name) {
            return items.FirstNonNull(t => t.CreateEntityReference(name));
        }

        public DomProcessingInstruction CreateProcessingInstruction(string target) {
            return items.FirstNonNull(t => t.CreateProcessingInstruction(target));
        }

        public DomDocumentFragment CreateDocumentFragment() {
            return items.FirstNonNull(t => t.CreateDocumentFragment());
        }

        public DomEntity CreateEntity(string name) {
            return items.FirstNonNull(t => t.CreateEntity(name));
        }

        public DomNotation CreateNotation(string name) {
            return items.FirstNonNull(t => t.CreateNotation(name));
        }

        public Type GetAttributeNodeType(DomName name) {
            return items.FirstNonNull(t => t.GetAttributeNodeType(name));
        }

        public Type GetElementNodeType(DomName name) {
            return items.FirstNonNull(t => t.GetElementNodeType(name));
        }

        public Type GetProcessingInstructionNodeType(string name) {
            return items.FirstNonNull(t => t.GetProcessingInstructionNodeType(name));
        }

        public DomName GetAttributeName(Type attributeType) {
            return items.FirstNonNull(t => t.GetAttributeName(attributeType));
        }

        public DomName GetElementName(Type elementType) {
            return items.FirstNonNull(t => t.GetElementName(elementType));
        }

        public string GetProcessingInstructionTarget(Type processingInstructionType) {
            return items.FirstNonNull(t => t.GetProcessingInstructionTarget(processingInstructionType));
        }

        public IEnumerator<IDomNodeFactory> GetEnumerator() {
            return ((IEnumerable<IDomNodeFactory>) this.items).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
