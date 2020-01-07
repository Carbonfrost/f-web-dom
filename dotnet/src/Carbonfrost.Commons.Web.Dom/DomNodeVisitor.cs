//
// - DomNodeVisitor.cs -
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
using System.Collections.Generic;
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomNodeVisitor : IDomNodeVisitor {

        public static readonly DomNodeVisitor Null = new NullNodeVisitor();

        protected DomNodeVisitor() {}

        public static void Visit(DomNode node, Action<DomNode> visitor) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }
            if (visitor == null) {
                return;
            }
            new ThunkVisitor(visitor).Visit(node);
        }

        public static void Visit(DomObject obj, Action<DomObject> visitor) {
            if (obj == null) {
                throw new ArgumentNullException("obj");
            }
            if (visitor == null) {
                return;
            }
            new ThunkVisitor(visitor).Visit(obj);
        }

        public virtual void Visit(DomObject obj) {
            if (obj == null) {
                throw new ArgumentNullException("obj");
            }

            obj.AcceptVisitor(this);
        }

        public virtual void Visit(IEnumerable<DomObject> objects) {
            if (objects == null) {
                throw new ArgumentNullException("objects");
            }

            foreach (var obj in objects) {
                Visit(obj);
            }
        }

        protected virtual void DefaultVisit(DomObject obj) {
            if (obj == null) {
                throw new ArgumentNullException("obj");
            }

            if (!obj.IsAttribute) {
                Visit(((DomNode) obj).ChildNodes);
            }
        }

        protected virtual void VisitElement(DomElement element) {
            if (element == null)
                throw new ArgumentNullException("element");

            DefaultVisit(element);
            Visit(element.Attributes);
        }

        protected virtual void VisitAttribute(DomAttribute attribute) {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            DefaultVisit(attribute);
        }

        protected virtual void VisitDocument(DomDocument document) {
            if (document == null)
                throw new ArgumentNullException("document");

            DefaultVisit(document);
        }

        protected virtual void VisitCDataSection(DomCDataSection section) {
            if (section == null)
                throw new ArgumentNullException("section");

            DefaultVisit(section);
        }

        protected virtual void VisitComment(DomComment comment) {
            if (comment == null)
                throw new ArgumentNullException("comment");

            DefaultVisit(comment);
        }

        protected virtual void VisitText(DomText text) {
            if (text == null)
                throw new ArgumentNullException("text");

            DefaultVisit(text);
        }

        protected virtual void VisitProcessingInstruction(DomProcessingInstruction instruction) {
            if (instruction == null)
                throw new ArgumentNullException("instruction");

            DefaultVisit(instruction);
        }

        protected virtual void VisitNotation(DomNotation notation) {
            if (notation == null)
                throw new ArgumentNullException("notation");

            DefaultVisit(notation);
        }

        protected virtual void VisitDocumentType(DomDocumentType documentType) {
            if (documentType == null)
                throw new ArgumentNullException("documentType");

            DefaultVisit(documentType);
        }

        protected virtual void VisitDocumentFragment(DomDocumentFragment fragment) {
            if (fragment == null)
                throw new ArgumentNullException("fragment");

            DefaultVisit(fragment);
        }

        protected virtual void VisitEntityReference(DomEntityReference reference) {
            if (reference == null)
                throw new ArgumentNullException("reference");

            DefaultVisit(reference);
        }

        protected virtual void VisitEntity(DomEntity entity) {
            if (entity == null)
                throw new ArgumentNullException("entity");

            DefaultVisit(entity);
        }

        void IDomNodeVisitor.Visit(DomElement element) {
            VisitElement(element);
        }

        void IDomNodeVisitor.Visit(DomAttribute attribute) {
            VisitAttribute(attribute);
        }

        void IDomNodeVisitor.Visit(DomDocument document) {
            VisitDocument(document);
        }

        void IDomNodeVisitor.Visit(DomCDataSection section) {
            VisitCDataSection(section);
        }

        void IDomNodeVisitor.Visit(DomComment comment) {
            VisitComment(comment);
        }

        void IDomNodeVisitor.Visit(DomText text) {
            VisitText(text);
        }

        void IDomNodeVisitor.Visit(DomProcessingInstruction instruction) {
            VisitProcessingInstruction(instruction);
        }

        void IDomNodeVisitor.Visit(DomNotation notation) {
            VisitNotation(notation);
        }

        void IDomNodeVisitor.Visit(DomDocumentType documentType) {
            VisitDocumentType(documentType);
        }

        void IDomNodeVisitor.Visit(DomEntityReference reference) {
            VisitEntityReference(reference);
        }

        void IDomNodeVisitor.Visit(DomEntity entity) {
            VisitEntity(entity);
        }

        void IDomNodeVisitor.Visit(DomDocumentFragment fragment) {
            VisitDocumentFragment(fragment);
        }

        sealed class ThunkVisitor : DomNodeVisitor {

            private readonly Action<DomObject> _visit;

            public ThunkVisitor(Action<DomObject> v) {
                _visit = v;
            }

            public ThunkVisitor(Action<DomNode> v) {
                _visit = o => {
                    var n = o as DomNode;
                    if (n != null) {
                        v(n);
                    }
                };
            }

            protected override void DefaultVisit(DomObject obj) {
                _visit(obj);
                base.DefaultVisit(obj);
            }
        }
    }
}
