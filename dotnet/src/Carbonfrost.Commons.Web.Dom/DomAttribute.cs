//
// Copyright 2013, 2019, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Reflection;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomAttribute : DomObject, IEquatable<DomAttribute> {

        private readonly string _name;

        public int AttributePosition {
            get {
                return SiblingIndex;
            }
        }

        public override DomNode ParentNode {
            get {
                return null; // per spec
            }
        }

        public DomElement OwnerElement {
            get {
                return (SiblingAttributes == null)
                    ? null
                    : SiblingAttributes.OwnerElement;
            }
        }

        public DomAttribute PreviousAttribute {
            get {
                if (SiblingAttributes == null)
                    return null;
                if (AttributePosition == 0)
                    return null;
                else
                    return SiblingAttributes[AttributePosition - 1];
            }
        }

        public DomAttribute NextAttribute {
            get {
                if (SiblingAttributes == null)
                    return null;
                if (AttributePosition == SiblingAttributes.Count - 1)
                    return null;
                else
                    return SiblingAttributes[AttributePosition + 1];
            }
        }

        protected override DomNodeCollection DomChildNodes {
            get {
                return DomNodeCollection.Empty;
            }
        }

        public string Name {
            get {
                return _name;
            }
        }

        public override string NodeName { get { return Name; } }
        public override string NodeValue { get { return Value; } }

        public override DomNodeType NodeType {
            get {
                return DomNodeType.Attribute;
            }
        }

        public override string TextContent {
            get { return Value; }
            set { Value = value; }
        }

        internal IDomValue DomValue {
            get {
                return (IDomValue) this.content;
            }
            set {
                ((IDomValue) this.content).Initialize(null);
                this.content = value;
                value.Initialize(this);
            }
        }

        public string Value {
            get { return DomValue.Value; }
            set { DomValue.Value = value; }
        }

        public DomAttributeDefinition AttributeDefinition {
            get {
                return DomAttributeDefinition;
            }
        }

        protected virtual DomAttributeDefinition DomAttributeDefinition {
            get {
                if (OwnerDocument == null || OwnerDocument.Schema == null) {
                    return new DomAttributeDefinition(Name);
                }
                return OwnerDocument.Schema.GetAttributeDefinition(Name);
            }
        }

        protected internal DomAttribute() {
            _name = RequireFactoryGeneratedName(GetType());
            this.content = Dom.DomValue.Create();
        }

        protected internal DomAttribute(string name) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrEmpty(name)) {
                throw Failure.EmptyString(nameof(name));
            }

            _name = name.Trim();
            this.content = Dom.DomValue.Create();
        }

        public DomAttribute Clone() {
            return (DomAttribute) CloneCore();
        }

        protected virtual DomAttribute CloneCore() {
            var result = this.OwnerDocument.CreateAttribute(Name, Value);
            result.DomValue = DomValue.Clone();
            return result;
        }

        public new DomAttribute RemoveSelf() {
            return (DomAttribute) base.RemoveSelf();
        }

        public DomAttribute ReplaceWith(DomAttribute other) {
            if (other == null) {
                return RemoveSelf();
            }
            OwnerElement.Attributes[SiblingIndex] = other;
            return other;
        }

        public DomAttribute SetValue(object value) {
            if (value is IDomValue dv) {
                DomValue = dv;
                return this;
            }
            if (value != null && DomValue is DomValue.Automatic) {
                // This is the first time it is being set, so we use the type of the
                // value to determine how the DomValue works going forward
                var tdv = Dom.DomValue.Create(value.GetType());
                tdv.TypedValue = value;
                DomValue = tdv;
                return this;
            }
            DomValue.TypedValue = value;
            return this;
        }

        public TValue GetValue<TValue>() {
            if (typeof(TValue) == typeof(string)) {
                object obj = Value;
                return (TValue) obj;
            }
            if (CONVERSION_TYPES.TryGetValue(typeof(TValue), out var method)) {
                return (TValue) method.InvokeWithUnwrap(null, new [] { this });
            }
            if (DomValue is TValue value) {
                return value;
            }
            if (DomValue is IDomValue<TValue> dd) {
                return dd.TypedValue;
            }
            return Activation.FromText<TValue>(Value);
        }

        public DomAttribute AppendValue(object value) {
            DomValue.AppendValue(value);
            return this;
        }

        public bool Equals(DomAttribute other) {
            return StaticEquals(this, other);
        }

        public override bool Equals(object obj) {
            return StaticEquals(this, obj as DomAttribute);
        }

        public override int GetHashCode() {
            int hashCode = 0;
            unchecked {
                hashCode += 37 * Name.GetHashCode();
            }
            return hashCode;
        }

        private static bool StaticEquals(DomAttribute lhs, DomAttribute rhs) {
            if (object.ReferenceEquals(lhs, rhs)) {
                return true;
            }

            if (object.ReferenceEquals(lhs, null) || object.ReferenceEquals(rhs, null)) {
                return false;
            }

            return lhs.Name == rhs.Name && lhs.content == rhs.content;
        }

        internal static DomAttribute[] CloneAll(DomAttribute[] items) {
            return Array.ConvertAll(items, i => i.Clone());
        }

        public override string ToString() {
            return Name;
        }
    }
}
