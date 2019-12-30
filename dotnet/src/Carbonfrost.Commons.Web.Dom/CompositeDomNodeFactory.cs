//
// - CompositeDomNodeFactory.cs -
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
using System.Collections.Generic;
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    class CompositeDomNodeFactory : IDomNodeFactory, IEnumerable<IDomNodeFactory> {

        readonly IDomNodeFactory[] items;

        public CompositeDomNodeFactory(IDomNodeFactory[] items) {
            this.items = items;
        }

        public DomAttribute CreateAttribute(string name) {
            return items.FirstNonNull(t => t.CreateAttribute(name));
        }

        public DomAttribute CreateAttribute(string name, IDomValue value) {
            return items.FirstNonNull(t => t.CreateAttribute(name, value));
        }

        public DomAttribute CreateAttribute(string name, string value) {
            return items.FirstNonNull(t => t.CreateAttribute(name, value));
        }

        public DomCDataSection CreateCDataSection() {
            return items.FirstNonNull(t => t.CreateCDataSection());
        }

        public DomCDataSection CreateCDataSection(string data) {
            return items.FirstNonNull(t => t.CreateCDataSection(data));
        }

        public DomText CreateText() {
            return items.FirstNonNull(t => t.CreateText());
        }

        public DomComment CreateComment(string data) {
            return items.FirstNonNull(t => t.CreateComment(data));
        }

        public DomDocumentType CreateDocumentType(string name, string publicId, string systemId) {
            return items.FirstNonNull(t => t.CreateDocumentType(name, publicId, systemId));
        }

        public DomElement CreateElement(string name) {
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

        public DomProcessingInstruction CreateProcessingInstruction(string target, string data) {
            return items.FirstNonNull(t => t.CreateProcessingInstruction(target, data));
        }

        public DomText CreateText(string data) {
            return items.FirstNonNull(t => t.CreateText());
        }

        public Type GetAttributeNodeType(string name) {
            return items.FirstNonNull(t => t.GetAttributeNodeType(name));
        }

        public Type GetCommentNodeType(string name) {
            return items.FirstNonNull(t => t.GetCommentNodeType(name));
        }

        public Type GetElementNodeType(string name) {
            return items.FirstNonNull(t => t.GetElementNodeType(name));
        }

        public Type GetEntityReferenceNodeType(string name) {
            return items.FirstNonNull(t => t.GetEntityReferenceNodeType(name));
        }

        public Type GetEntityNodeType(string name) {
            return items.FirstNonNull(t => t.GetEntityNodeType(name));
        }

        public Type GetNotationNodeType(string name) {
            return items.FirstNonNull(t => t.GetNotationNodeType(name));
        }

        public Type GetProcessingInstructionNodeType(string name) {
            return items.FirstNonNull(t => t.GetProcessingInstructionNodeType(name));
        }

        public Type GetTextNodeType(string name) {
            return items.FirstNonNull(t => t.GetTextNodeType(name));
        }

        public IEnumerator<IDomNodeFactory> GetEnumerator() {
            return ((IEnumerable<IDomNodeFactory>) this.items).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}


