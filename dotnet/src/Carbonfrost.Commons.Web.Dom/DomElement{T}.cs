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
using System.Collections.Generic;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomElement<TElement> : DomElement, IDomContainerManipulationApiConventions<TElement>
        where TElement : DomElement<TElement>
    {

        protected DomElement() : base() {}
        protected DomElement(string name) : base(name) {}
        protected DomElement(DomName name) : base(name) {}

        public new TElement Add(object content) {
            return (TElement) base.Add(content);
        }

        public new TElement AddAnnotation(object annotation) {
            return (TElement) base.AddAnnotation(annotation);
        }

        public new TElement AddAnnotations(IEnumerable<object> annotations) {
            return (TElement) base.AddAnnotations(annotations);
        }

        public new TElement AddClass(string className) {
            return (TElement) base.AddClass(className);
        }

        public new TElement AddRange(object content1) {
            return (TElement) base.AddRange(content1);
        }

        public new TElement AddRange(object content1, object content2) {
            return (TElement) base.AddRange(content1, content2);
        }

        public new TElement AddRange(object content1, object content2, object content3) {
            return (TElement) base.AddRange(content1, content2, content3);
        }

        public new TElement AddRange(params object[] content) {
            return (TElement) base.AddRange(content);
        }

        public new TElement Append(DomNode child) {
            return (TElement) base.Append(child);
        }

        public new TElement Attribute(string name, object value) {
            return (TElement) base.Attribute(name, value);
        }

        public new TElement Clone() {
            return (TElement) base.Clone();
        }

        public new TElement CompressWhitespace() {
            return (TElement) base.CompressWhitespace();
        }

        public new TElement Empty() {
            return (TElement) base.Empty();
        }

        public new TElement RemoveChildNodes() {
            return (TElement) base.RemoveChildNodes();
        }

        public new TElement Remove() {
            return (TElement) base.Remove();
        }

        public new TElement RemoveAnnotation(object value) {
            return (TElement) base.RemoveAnnotation(value);
        }

        public new TElement RemoveAnnotations(Type type) {
            return (TElement) base.RemoveAnnotations(type);
        }

        public new TElement RemoveAnnotations<T>() where T: class {
            return (TElement) base.RemoveAnnotations<T>();
        }

        public new TElement RemoveAttribute(string name) {
            return (TElement) base.RemoveAttribute(name);
        }

        public new TElement RemoveAttributes() {
            return (TElement) base.RemoveAttributes();
        }

        public new TElement RemoveClass(string className) {
            return (TElement) base.RemoveClass(className);
        }

        public new TElement RemoveSelf() {
            return (TElement) base.RemoveSelf();
        }

        public new TElement SetName(string name) {
            return (TElement) base.SetName(name);
        }

        public new TElement Wrap(DomContainer newParent) {
            return (TElement) base.Wrap(newParent);
        }

        public new TElement Wrap(string element) {
            return (TElement) base.Wrap(element);
        }
    }

}
