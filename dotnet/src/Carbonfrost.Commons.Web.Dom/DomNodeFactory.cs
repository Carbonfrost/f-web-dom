//
// Copyright 2013, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    [Providers]
    public class DomNodeFactory : IDomNodeFactory {

        [DomNodeFactoryUsage]
        public static readonly IDomNodeFactory Null = new NullDomNodeFactory();

        [DomNodeFactoryUsage]
        public static readonly IDomNodeFactory Default = new DomNodeFactory();

        public static IDomNodeFactory FromName(string name) {
            return App.GetProvider<IDomNodeFactory>(name);
        }

        [DomNodeFactoryUsage]
        public static IDomNodeFactory Compose(params IDomNodeFactory[] items) {
            return Compose((IEnumerable<IDomNodeFactory>) items);
        }

        public static IDomNodeFactory Compose(IEnumerable<IDomNodeFactory> items) {
            return Utility.OptimalComposite(
                items, m => new CompositeDomNodeFactory(m), Null
            );
        }

        public DomComment CreateComment(string data) {
            var result = CreateComment();
            result.Data = data;
            return result;
        }

        public virtual DomComment CreateComment() {
            return new DomComment();
        }

        public virtual DomCDataSection CreateCDataSection() {
            return new DomCDataSection();
        }

        public DomCDataSection CreateCDataSection(string data) {
            var result = CreateCDataSection();
            result.Data = data;
            return result;
        }

        public virtual DomText CreateText(string data) {
            var result = CreateText();
            result.Data = data;
            return result;
        }

        public virtual DomText CreateText() {
            return new DomText();
        }

        public DomProcessingInstruction CreateProcessingInstruction(string target) {
            return CreateProcessingInstruction(target, null);
        }

        public virtual DomProcessingInstruction CreateProcessingInstruction(string target, string data) {
            if (target == null)
                throw new ArgumentNullException("target");
            if (target.Length == 0)
                throw Failure.EmptyString("target");

            var result = new DomProcessingInstruction(target);
            result.Data = data;
            return result;
        }

        public virtual Type GetAttributeNodeType(string name) {
            return typeof(DomAttribute);
        }

        public virtual Type GetElementNodeType(string name) {
            return typeof(DomElement);
        }

        public virtual Type GetProcessingInstructionNodeType(string name) {
            return typeof(DomProcessingInstruction);
        }

        public virtual Type GetTextNodeType(string name) {
            return typeof(DomText);
        }

        public virtual Type GetCommentNodeType(string name) {
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

        public virtual DomAttribute CreateAttribute(string name) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw Failure.EmptyString("name");

            return new DomAttribute(name);
        }

        public DomAttribute CreateAttribute(string name, string value) {
            var attr = CreateAttribute(name);
            attr.Value = value;
            return attr;
        }

        public DomAttribute CreateAttribute(string name, IDomValue value) {
            if (value == null)
                throw new ArgumentNullException("value");

            string t = value.Value;
            var result = CreateAttribute(name, t);
            result.DomValue = value;
            return result;
        }

        public virtual DomElement CreateElement(string name) {
            return new DomElement(name);
        }

        public virtual DomEntityReference CreateEntityReference(string name) {
            return new DomEntityReference(name);
        }

        public virtual DomDocumentType CreateDocumentType(string name, string publicId, string systemId) {
            var result = new DomDocumentType(name);

            // TODO Look up based on name
            result.PublicId = publicId;
            result.SystemId = systemId;
            return result;
        }

        // TODO DomDocument should delegate to node factory

    }
}
