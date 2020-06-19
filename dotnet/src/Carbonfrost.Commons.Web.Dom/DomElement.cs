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
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomElement : DomContainer, IDomContainerManipulationApiConventions<DomElement> {

        private readonly DomAttributeCollectionApi _attributes;
        private DomName _name;

        public bool HasElements {
            get {
                return Elements.Any();
            }
        }

        public bool IsFirstChild {
            get {
                return PreviousSibling == null;
            }
        }

        public bool IsDocumentElement {
            get {
                return OwnerDocument.DocumentElement == this;
            }
        }

        public bool IsLastChild {
            get {
                return NextSibling == null;
            }
        }

        public string Id {
            get {
                return Attribute("id");
            }
            set {
                Attribute("id", value);
            }
        }

        protected override DomAttributeCollection DomAttributes {
            get {
                return _attributes;
            }
        }

        public DomElement PreviousSibling {
            get {
                var answer = PreviousSiblingNode;
                while (answer != null) {
                    if (answer.IsElement) {
                        return (DomElement) answer;
                    }

                    answer = answer.PreviousSiblingNode;
                }

                return null;
            }
        }

        public DomElement NextSibling {
            get {
                var answer = NextSiblingNode;
                while (answer != null) {
                    if (answer.IsElement) {
                        return (DomElement) answer;
                    }

                    answer = answer.NextSiblingNode;
                }

                return null;
            }
        }

        // TODO Memoize this lookup (performance)

        public int ElementPosition {
            get {
                if (ParentElement == null) {
                    return -1;
                }

                int index = 0;
                foreach (var e in ParentElement.Elements) {
                    if (e == this) {
                        return index;
                    }
                    index++;
                }

                return -1;
            }
        }

        public override DomName Name {
            get {
                return _name.Resolve(NameContext);
            }
        }

        internal string OwnText {
            get {
                // TODO Technically, should use Html StringUtil.GetOwnText, which
                // needs to be added here to preserve ws rules
                return InnerText;
            }
        }

        public DomElementCollection PrecedingSiblings {
            get {
                return new DefaultDomElementCollection(this, e => e.PrecedingNodes.OfType<DomElement>());
            }
        }

        public DomElementCollection FollowingSiblings {
            get {
                return new DefaultDomElementCollection(this, e => e.FollowingNodes.OfType<DomElement>());
            }
        }

        protected internal DomElement() {
            _name = CheckName(RequireFactoryGeneratedName(
                GetType(),
                (e, t) => e.GetElementName(t)
            ));
            _attributes = new DomAttributeCollectionApi(this);
        }

        protected internal DomElement(string name) : this(DomName.Create(name)) {
        }

        protected internal DomElement(DomName name) {
            _name = CheckName(name);
            _attributes = new DomAttributeCollectionApi(this);
        }

        public override DomElementCollection DescendantsAndSelf {
            get {
                return new DefaultDomElementCollection(
                    this,
                    e => Utility.Cons((DomElement) e, e.Descendants)
                );
            }
        }

        public override string NodeName {
            get {
                return Name.LocalName;
            }
        }

        public DomElementDefinition ElementDefinition {
            get {
                return DomElementDefinition;
            }
        }

        protected virtual DomElementDefinition DomElementDefinition {
            get {
                if (OwnerDocument == null || OwnerDocument.Schema == null) {
                    return new DomElementDefinition(Name);
                }
                return OwnerDocument.Schema.GetElementDefinition(Name);
            }
        }

        protected override DomNode CloneCore() {
            DomElement result = OwnerDocument.CreateElement(Name);
            foreach (var m in Attributes) {
                result.Attributes.Add(m.Clone());
            }
            foreach (var m in ChildNodes) {
                result.Append(m.Clone());
            }
            result.CopyAnnotationsFrom(AnnotationList);

            return result;
        }

        public DomElementQuery AsQuery() {
            return new DomElementQuery(this);
        }

        public new DomElement Clone() {
            return (DomElement) base.Clone();
        }

        public new DomElement Add(object content) {
            return (DomElement) base.Add(content);
        }

        public new DomElement AddRange(params object[] content) {
            return (DomElement) base.AddRange(content);
        }

        public new DomElement AddRange(object content1) {
            return (DomElement) base.AddRange(content1);
        }

        public new DomElement AddRange(object content1, object content2) {
            return (DomElement) base.AddRange(content1, content2);
        }

        public new DomElement AddRange(object content1, object content2, object content3) {
            return (DomElement) base.AddRange(content1, content2, content3);
        }

        public new DomElement Empty() {
            return (DomElement) base.Empty();
        }

        public new DomElement RemoveChildNodes() {
            return (DomElement) base.RemoveChildNodes();
        }

        public new DomElement Attribute(string name, object value) {
            return (DomElement) base.Attribute(name, value);
        }

        public new DomElement Attribute(DomName name, object value) {
            return (DomElement) base.Attribute(name, value);
        }

        public new DomElement Append(DomNode child) {
            return (DomElement) base.Append(child);
        }

        public new DomElement RemoveAttribute(string name) {
            return (DomElement) base.RemoveAttribute(name);
        }

        public new DomElement RemoveAttribute(DomName name) {
            return (DomElement) base.RemoveAttribute(name);
        }

        public new DomElement RemoveAttributes() {
            return (DomElement) base.RemoveAttributes();
        }

        public new DomElement RemoveSelf() {
            return (DomElement) base.RemoveSelf();
        }

        public new DomElement Remove() {
            return (DomElement) base.Remove();
        }

        public new DomElement AddClass(string className) {
            return (DomElement) base.AddClass(className);
        }

        public new DomElement AddClass(params string[] classNames) {
            return (DomElement) base.AddClass(classNames);
        }

        public new DomElement RemoveClass(string className) {
            return (DomElement) base.RemoveClass(className);
        }

        public new DomElement RemoveClass(params string[] classNames) {
            return (DomElement) base.RemoveClass(classNames);
        }

        protected override DomObject SetNameCore(DomName name) {
            var newElement = OwnerDocument.CreateElement(name);
            newElement.Append(ChildNodes.ToList());
            newElement.Attributes.AddRange(Attributes.ToList());
            newElement.CopyAnnotationsFrom(AnnotationList);
            return ReplaceWith(newElement);
        }

        internal override void AssertCanUnwrap() {
            if (IsDocumentElement && Elements.Count > 1) {
                throw DomFailure.CannotUnwrapWouldCreateMalformedDocument();
            }
        }

        internal override void NotifyNameContextChanged() {
            _attributes.SetComparer(NameContext.Comparer);
            base.NotifyNameContextChanged();
        }

        public override DomNodeType NodeType {
            get {
                return DomNodeType.Element;
            }
        }
    }
}
