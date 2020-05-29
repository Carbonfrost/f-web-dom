//
// Copyright 2013, 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Carbonfrost.Commons.Web.Dom {

    // Facilitates treating DomDocument sort of like an element, but
    // really, all containers are elements

    public abstract partial class DomContainer : DomNode, IDomContainerManipulationApiConventions<DomContainer> {

        protected DomContainer() : this(false) {
            // TODO Replace with linked list (performance)
        }

        internal DomContainer(bool useLL) {
            content = new DomNodeCollectionApi(this, NewNodeStorage(useLL));
        }

        protected override DomNodeCollection DomChildNodes {
            get {
                return (DomNodeCollection) content;
            }
        }

        public virtual DomElementCollection AncestorsAndSelf {
            get {
                return new DefaultDomElementCollection(this, n => n.GetAncestorsCore(true));
            }
        }

        public DomElementCollection Ancestors {
            get {
                return new DefaultDomElementCollection(this, n => n.GetAncestorsCore(false));
            }
        }

        public virtual DomElementCollection DescendantsAndSelf {
            get {
                return new DefaultDomElementCollection(this, n => n.GetDescendantsCore(true));
            }
        }

        public DomElementCollection Descendants {
            get {
                return new DefaultDomElementCollection(this, n => n.GetDescendantsCore(false));
            }
        }

        public override string InnerText {
            get {
                return OuterTextWriter.ConvertToString(ChildNodes);
            }
            set {
                Empty();
                Append(OwnerDocument.CreateText(value));
            }
        }

        public override string InnerXml {
            get {
                return new OuterXmlVisitor().ConvertToString(ChildNodes);
            }
            set {
                var frag = OwnerDocument.CreateDocumentFragment();
                frag.LoadXml(value);
                Empty();
                Append(frag.ChildNodes.ToList());
            }
        }

        public DomElement Descendant(string name) {
            return GetElementsByTagName(name).FirstOrDefault();
        }

        public DomElement Descendant(string name, string xmlns) {
            return GetElementsByTagName(name, xmlns).FirstOrDefault();
        }

        public DomElementCollection Children {
            get {
                return Elements;
            }
        }

        public virtual DomElementCollection Elements {
            get {
                return new DefaultDomElementCollection(
                    this,
                    e => e.ChildNodes.Where(t => t.IsElement).Cast<DomElement>()
                );
            }
        }

        public DomElement Element(int index) {
            return Elements.ElementAtOrDefault(index);
        }

        public DomElement Element(string name) {
            CheckName(name);
            return Elements.FirstOrDefault(t => t.Name == name);
        }

        public DomElement Element(string name, string xmlns) {
            CheckName(name);
            return Elements.FirstOrDefault(t => t.Name == name && t.NamespaceUri == xmlns);
        }

        public virtual DomElementCollection GetElementsByTagName(string name) {
            return new DefaultDomElementCollection(this, n => n.Descendants(name));
        }

        public virtual DomElementCollection GetElementsByTagName(string name, string namespaceUri) {
            return new DefaultDomElementCollection(this, n => n.Descendants(name, namespaceUri));
        }

        public virtual DomElementCollection GetElementsByClassName(string className) {
            if (string.IsNullOrEmpty(className)) {
                return DomElementCollection.Empty;
            }

            var names = DomStringTokenList.Parse(className);
            return new DefaultDomElementCollection(
                this,
                n => n.Descendants.Where(t => DomStringTokenList.StaticContains(t.Attribute("class"), names))
            );
        }

        public bool IsEmpty {
            get {
                return ChildNodes.Count == 0;
            }
        }

        public sealed override bool IsContainer {
            get {
                return true;
            }
        }

        public DomElement Parent {
            get {
                return ParentElement;
            }
        }

        public DomElement FirstChild {
            get {
                return Elements.FirstOrDefault();
            }
        }

        public DomElement LastChild {
            get {
                return Elements.LastOrDefault();
            }
        }

        public DomElement Child(int index) {
            return Elements.ElementAtOrDefault(index);
        }

        public DomContainer AddRange(params object[] content) {
            foreach (var o in content) {
                Add(o);
            }
            return this;
        }

        public DomContainer AddRange(object content1) {
            return Add(content1);
        }

        public DomContainer AddRange(object content1, object content2) {
            return Add(content1).Add(content2);
        }

        public DomContainer AddRange(object content1, object content2, object content3) {
            return Add(content1).Add(content2).Add(content3);
        }

        public DomContainer Add(object content) {
            if (content == null) {
                return this;
            }

            var n = content as DomNode;
            if (n != null) {
                return (DomContainer) Append(n);
            }

            string s = content as string;
            if (s != null) {
                return AddString(s);
            }

            var a = content as DomAttribute;
            if (a != null) {
                return (DomContainer) Append(a);
            }

            var array = content as object[];
            if (array != null) {
                foreach (object item in array)
                    Add(item);
                return this;
            }

            var enumerable = content as IEnumerable;
            if (enumerable != null) {
                foreach (object item in enumerable)
                    Add(item);
                return this;
            }

            return AddString(Utility.ConvertToString(content));
        }

        private DomContainer AddString(string s) {
            if (s.Length == 0) {
                return this;
            }

            if (LastChildNode == null || !LastChildNode.IsCharacterData) {
                AppendText(s);
            } else {
                ((DomCharacterData) LastChildNode).AppendData(s);
            }

            return this;
        }

        public override string ToString() {
            return NodeName;
        }

        internal virtual void AssertCanAppend(DomNode node, DomNode willReplace) {
        }

        internal void CoreLoadXml(XmlReader reader) {
            if (reader == null) {
                throw new ArgumentNullException(nameof(reader));
            }

            DomContainer currentContainer = this;
            DomContainer starting = this;
            while (reader.Read()) {
                switch (reader.NodeType) {
                    case XmlNodeType.None:
                    case XmlNodeType.Attribute:
                    case XmlNodeType.Document:
                    case XmlNodeType.DocumentFragment:
                        // These shouldn't occur - listed for completeness
                        break;

                    case XmlNodeType.Element:
                        var e = currentContainer.AppendElement(reader.Name);
                        CopyAttributes(reader, e);
                        if (!reader.IsEmptyElement) {
                            currentContainer = e;
                        }
                        break;

                    case XmlNodeType.Comment:
                        currentContainer.AppendComment(reader.Value);
                        break;

                    case XmlNodeType.CDATA:
                        currentContainer.AppendCDataSection(reader.Value);
                        break;

                    case XmlNodeType.EntityReference:
                    case XmlNodeType.Entity:
                    case XmlNodeType.DocumentType:
                    case XmlNodeType.Notation:
                        throw new NotImplementedException();

                    case XmlNodeType.ProcessingInstruction:
                        currentContainer.AppendProcessingInstruction(reader.Name, reader.Value);
                        break;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                    case XmlNodeType.Text:
                        currentContainer.AppendText(reader.Value);
                        break;

                    case XmlNodeType.EndElement:
                        currentContainer = currentContainer.ParentElement ?? starting;
                        break;
                    case XmlNodeType.EndEntity:
                        break;

                    case XmlNodeType.XmlDeclaration:
                        break;
                }
            }
        }

        void CopyAttributes(XmlReader reader, DomElement element) {
            if (reader.HasAttributes && reader.MoveToFirstAttribute()) {
                do {
                    element.Attribute(reader.Name, reader.Value);
                } while(reader.MoveToNextAttribute());
                reader.MoveToElement();
            }
        }

        private IEnumerable<DomElement> GetDescendantsCore(bool self) {
            var queue = new Queue<DomElement>();
            var ele = this as DomElement;
            if (self && ele != null) {
                queue.Enqueue(ele);

            } else {
                QueueChildren(queue);
            }

            while (queue.Count > 0) {
                var result = queue.Dequeue();
                yield return result;
                result.QueueChildren(queue);
            }
        }

        private void QueueChildren(Queue<DomElement> queue) {
            foreach (var child in Elements)
                queue.Enqueue(child);
        }

        private IEnumerable<DomElement> GetAncestorsCore(bool self) {
            if (self && this is DomElement) {
                yield return (DomElement) this;
            }

            DomElement current = this.ParentElement;
            while (current != null) {
                yield return current;
                current = current.ParentElement;
            }
        }

        public new DomContainer AddClass(string className) {
            return (DomContainer) base.AddClass(className);
        }

        public new DomContainer Append(DomNode child) {
            return (DomContainer) base.Append(child);
        }

        public new DomContainer Attribute(string name, object value) {
            return (DomContainer) base.Attribute(name, value);
        }

        public new DomContainer Clone() {
            return (DomContainer) base.Clone();
        }

        public new DomContainer Empty() {
            return (DomContainer) base.Empty();
        }

        public new DomContainer RemoveChildNodes() {
            return (DomContainer) base.RemoveChildNodes();
        }

        public new DomContainer Remove() {
            return (DomContainer) base.Remove();
        }

        public new DomContainer RemoveAttribute(string name) {
            return (DomContainer) base.RemoveAttribute(name);
        }

        public new DomContainer RemoveAttributes() {
            return (DomContainer) base.RemoveAttributes();
        }

        public new DomContainer RemoveClass(string className) {
            return (DomContainer) base.RemoveClass(className);
        }

        public new DomContainer RemoveSelf() {
            return (DomContainer) base.RemoveSelf();
        }

        private DomNodeCollection NewNodeStorage(bool useLL) {
            if (useLL) {
                return new LinkedDomNodeList();
            } else {
                return new ListDomNodeCollection();
            }
        }
    }

}
