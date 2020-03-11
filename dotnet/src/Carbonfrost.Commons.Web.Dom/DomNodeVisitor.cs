//
// Copyright 2013, 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomNodeVisitor : IDomNodeVisitor, IDomNodeVisitDispatcher {

        public static readonly DomNodeVisitor Null = new NullNodeVisitor();

        private readonly IDomNodeVisitDispatcher _dispatcher;

        protected DomNodeVisitor() {
            _dispatcher = new DomNodeVisitDispatcher(this);
        }

        public static void Visit(DomNode node, Action<DomNode> visitor) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }
            if (visitor == null) {
                throw new ArgumentNullException(nameof(visitor));
            }
            new ThunkVisitor(visitor).Visit(node);
        }

        public static void VisitAll(IEnumerable<DomObject> objs, Action<DomNode> visitor) {
            if (objs == null) {
                return;
            }
            if (visitor == null) {
                throw new ArgumentNullException(nameof(visitor));
            }
            new ThunkVisitor(visitor).VisitAll(objs);
        }

        public static void Visit(DomObject obj, Action<DomObject> visitor) {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }
            if (visitor == null) {
                throw new ArgumentNullException(nameof(visitor));
            }
            new ThunkVisitor(visitor).Visit(obj);
        }

        public static void VisitAll(IEnumerable<DomObject> objs, Action<DomObject> visitor) {
            if (objs == null) {
                return;
            }
            if (visitor == null) {
                throw new ArgumentNullException(nameof(visitor));
            }
            new ThunkVisitor(visitor).VisitAll(objs);
        }

        public static void Visit(DomObject obj, IDomNodeVisitor visitor) {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }
            if (visitor == null) {
                throw new ArgumentNullException(nameof(visitor));
            }
            DomNodeVisitDispatcher.Create(visitor).Dispatch(obj);
        }

        public static void VisitAll(IEnumerable<DomObject> objs, IDomNodeVisitor visitor) {
            if (objs == null || visitor == null) {
                return;
            }
            var d = DomNodeVisitDispatcher.Create(visitor);
            foreach (var obj in objs) {
                d.Dispatch(obj);
            }
        }

        public void Visit(DomObject obj) {
            ((IDomNodeVisitDispatcher) this).Dispatch(obj);
        }

        public void VisitAll(IEnumerable<DomObject> objects) {
            if (objects == null) {
                return;
            }

            foreach (var obj in objects) {
                Visit(obj);
            }
        }

        protected virtual void DefaultVisit(DomObject obj) {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            if (!obj.IsAttribute) {
                VisitAll(((DomNode) obj).ChildNodes);
            }
        }

        protected virtual void VisitElement(DomElement element) {
            if (element == null) {
                throw new ArgumentNullException(nameof(element));
            }

            DefaultVisit(element);
            VisitAll(element.Attributes);
        }

        protected virtual void VisitAttribute(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException(nameof(attribute));
            }

            DefaultVisit(attribute);
        }

        protected virtual void VisitDocument(DomDocument document) {
            if (document == null) {
                throw new ArgumentNullException(nameof(document));
            }

            DefaultVisit(document);
        }

        protected virtual void VisitCDataSection(DomCDataSection section) {
            if (section == null) {
                throw new ArgumentNullException(nameof(section));
            }

            DefaultVisit(section);
        }

        protected virtual void VisitComment(DomComment comment) {
            if (comment == null) {
                throw new ArgumentNullException(nameof(comment));
            }

            DefaultVisit(comment);
        }

        protected virtual void VisitText(DomText text) {
            if (text == null) {
                throw new ArgumentNullException(nameof(text));
            }

            DefaultVisit(text);
        }

        protected virtual void VisitProcessingInstruction(DomProcessingInstruction instruction) {
            if (instruction == null) {
                throw new ArgumentNullException(nameof(instruction));
            }

            DefaultVisit(instruction);
        }

        protected virtual void VisitNotation(DomNotation notation) {
            if (notation == null) {
                throw new ArgumentNullException(nameof(notation));
            }

            DefaultVisit(notation);
        }

        protected virtual void VisitDocumentType(DomDocumentType documentType) {
            if (documentType == null) {
                throw new ArgumentNullException(nameof(documentType));
            }

            DefaultVisit(documentType);
        }

        protected virtual void VisitDocumentFragment(DomDocumentFragment fragment) {
            if (fragment == null) {
                throw new ArgumentNullException(nameof(fragment));
            }

            DefaultVisit(fragment);
        }

        protected virtual void VisitEntityReference(DomEntityReference reference) {
            if (reference == null) {
                throw new ArgumentNullException(nameof(reference));
            }

            DefaultVisit(reference);
        }

        protected virtual void VisitEntity(DomEntity entity) {
            if (entity == null) {
                throw new ArgumentNullException(nameof(entity));
            }

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

        void IDomNodeVisitDispatcher.Dispatch(DomObject obj) {
            _dispatcher.Dispatch(obj);
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
