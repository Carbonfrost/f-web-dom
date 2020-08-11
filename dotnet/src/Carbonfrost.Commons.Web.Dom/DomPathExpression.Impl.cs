//
// Copyright 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Linq;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomPathExpression {

        internal static readonly DomPathExpression Root = new RootExpr();

        internal static DomPathExpression Element(DomName name, int index) {
            return new ElementExpr(index, name);
        }

        internal static DomPathExpression ById(string id) {
            if (string.IsNullOrWhiteSpace(id)) {
                throw Failure.AllWhitespace(nameof(id));
            }

            return new IdExpr(id);
        }

        internal static DomPathExpression Attribute(DomName name) {
            return new AttributeExpr(name);
        }

        internal static DomPathExpression DescendantHasAttribute(DomName name) {
            return new DescendantHasAttributeExpr(name);
        }

        internal static DomPathExpression DescendantHasAttributeValue(DomName name, string value) {
            return new DescendantHasAttributeValueExpr(name, value);
        }

        sealed class ElementExpr : DomPathExpression {

            private readonly int _index;
            private readonly DomName _element;

            public ElementExpr(int index, DomName element) {
                _index = index;
                _element = element;
            }

            public override int Index {
                get {
                    return _index;
                }
            }

            public override DomName Name {
                get {
                    return _element;
                }
            }

            public override DomPathExpressionType Type {
                get {
                    return DomPathExpressionType.Element;
                }
            }

            internal override DomObjectQuery Apply(DomObjectQuery root, DomObjectQuery q) {
                IEnumerable<DomElement> result;
                if (Index >= 0) {
                    result = FilterChildren(q, e => e.Name == Name && e.ElementPosition == Index);
                } else {
                    result = FilterChildren(q, e => e.Name == Name);
                }
                return new DomObjectQuery(result);
            }

            public override string ToString() {
                if (Index >= 0) {
                    return $"/{Name}[{Index}]";
                }
                return $"/{Name}";
            }
        }

        sealed class RootExpr : DomPathExpression {

            public override DomPathExpressionType Type {
                get {
                    return DomPathExpressionType.Root;
                }
            }

            public override string ToString() {
                return "";
            }

            internal override DomObjectQuery Apply(DomObjectQuery root, DomObjectQuery q) {
                return root;
            }
        }

        sealed class IdExpr : DomPathExpression {

            private readonly string _id;

            public IdExpr(string id) {
                _id = id;
            }

            public override string Id {
                get {
                    return _id;
                }
            }

            public override DomPathExpressionType Type {
                get {
                    return DomPathExpressionType.Id;
                }
            }

            public override string ToString() {
                return $"//*[@id=\"{Id}\"]";
            }

            internal override DomObjectQuery Apply(DomObjectQuery root, DomObjectQuery q) {
                return new DomObjectQuery(
                    FilterDescendants(q, e => e.Attribute("id") == Id)
                );
            }
        }

        sealed class AttributeExpr : DomPathExpression {

            private readonly DomName _name;

            public AttributeExpr(DomName name) {
                _name = name;
            }

            public override DomName Name {
                get {
                    return _name;
                }
            }

            public override DomPathExpressionType Type {
                get {
                    return DomPathExpressionType.Attribute;
                }
            }

            public override string ToString() {
                return $"@{Name}";
            }

            internal override DomObjectQuery Apply(DomObjectQuery root, DomObjectQuery q) {
                return new DomObjectQuery(
                    q.OfType<DomContainer>().Select(ele => ele.Attributes[Name]).NonNull()
                );
            }
        }

        sealed class DescendantHasAttributeExpr : DomPathExpression {

            private readonly DomName _name;

            public DescendantHasAttributeExpr(DomName name) {
                _name = name;
            }

            public override DomName Name {
                get {
                    return _name;
                }
            }

            public override DomPathExpressionType Type {
                get {
                    return DomPathExpressionType.DescendantHasAttribute;
                }
            }

            public override string ToString() {
                return $"//*[@{Name}]";
            }

            internal override DomObjectQuery Apply(DomObjectQuery root, DomObjectQuery q) {
                return new DomObjectQuery(FilterDescendants(q, e => e.HasAttribute(Name)));
            }
        }

        sealed class DescendantHasAttributeValueExpr : DomPathExpression {

            private readonly DomName _name;
            private readonly string _value;

            public DescendantHasAttributeValueExpr(DomName name, string value) {
                if (value[0] == '"') {
                    throw new ArgumentException();
                }
                _name = name;
                _value = value;
            }

            public override string Value {
                get {
                    return _value;
                }
            }

            public override DomName Name {
                get {
                    return _name;
                }
            }

            public override DomPathExpressionType Type {
                get {
                    return DomPathExpressionType.DescendantHasAttributeValue;
                }
            }

            internal override DomObjectQuery Apply(DomObjectQuery root, DomObjectQuery q) {
                return new DomObjectQuery(FilterDescendants(q, e => e.Attribute(Name) == Value));
            }

            public override string ToString() {
                return $"//*[@{Name}=\"{Value}\"]";
            }
        }
    }
}
