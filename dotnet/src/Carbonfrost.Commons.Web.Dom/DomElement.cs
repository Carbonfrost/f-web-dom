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
using System.ComponentModel;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomElement : DomContainer {

        private readonly DomAttributeCollection attributes;
        private readonly string name;

        public bool HasElements {
            get {
                return this.Elements.Any();
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
                return this.attributes;
            }
        }

        // TODO Set up element position

        public DomElement PreviousSibling {
            get {
                var answer = this.PreviousSiblingNode;
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
                if (this.ParentElement == null)
                    return -1;

                int index = 0;
                foreach (var e in ParentElement.Elements) {
                    if (e == this)
                        return index;
                    index++;
                }

                return -1;
            }
        }

        public string Name {
            get {
                return name;
            }
        }

        internal string OwnText {
            get {
                // TODO Technically, should use Html StringUtil.GetOwnText, which
                // needs to be added here to preserve ws rules
                return this.InnerText;
            }
        }

        public IEnumerable<DomElement> PrecedingSiblings {
            get {
                return PrecedingNodes.OfType<DomElement>();
            }
        }

        public IEnumerable<DomElement> FollowingSiblings {
            get {
                return FollowingNodes.OfType<DomElement>();
            }
        }

        protected internal DomElement() {
            this.name = CheckName(RequireFactoryGeneratedName(GetType()));
            this.attributes = new DomAttributeCollection(this);
        }

        protected internal DomElement(string name) {
            this.name = CheckName(name);
            this.attributes = new DomAttributeCollection(this);
        }

        public override IEnumerable<DomElement> DescendantsAndSelf {
            get {
                return Utility.Cons(this, base.Descendants);
            }
        }

        public override string NodeName {
            get {
                return this.name;
            }
        }

        public DomElementDefinition ElementDefinition {
            get {
                return this.OwnerDocument.GetElementDefinition(this.Name);
            }
        }

        protected override DomNode CloneCore() {
            return Clone();
        }

        public new DomElement Clone() {
            DomElement result = OwnerDocument.CreateElement(this.Name);
            foreach (var m in this.Attributes)
                result.Attributes.Add(m.Clone());

            foreach (var m in ChildNodes) {
                result.Append(m.Clone());
            }

            return result;
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

        public new DomElement Attribute(string name, object value) {
            return (DomElement) base.Attribute(name, value);
        }

        public new DomElement Append(DomNode child) {
            return (DomElement) base.Append(child);
        }

        public new DomElement RemoveAttribute(string name) {
            return (DomElement) base.RemoveAttribute(name);
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

        public new DomElement RemoveClass(string className) {
            return (DomElement) base.RemoveClass(className);
        }

        protected override DomObject SetNameCore(string name) {
            var newElement = OwnerDocument.CreateElement(name);
            newElement.Append(ChildNodes.ToList());
            newElement.Attributes.AddRange(Attributes.ToList());
            return ReplaceWith(newElement);
        }

        protected override DomNode UnwrapCore() {
            if (IsDocumentElement && Elements.Count > 1) {
                throw DomFailure.CannotUnwrapWouldCreateMalformedDocument();
            }
            return base.UnwrapCore();
        }

        internal override void AcceptVisitor(IDomNodeVisitor visitor) {
            visitor.Visit(this);
        }

        internal override TResult AcceptVisitor<TArgument, TResult>(IDomNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            return visitor.Visit(this, argument);
        }

        public override DomNodeType NodeType {
            get { return DomNodeType.Element; }
        }

    }
}

