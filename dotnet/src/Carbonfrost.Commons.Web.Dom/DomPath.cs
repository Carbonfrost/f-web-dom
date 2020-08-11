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
using System.Text;
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract partial class DomPath : DomSelector {

        private static readonly DomPath _root = new DomPathLeaf(null, DomPathExpression.Root);

        public static readonly DomPath Empty;

        public abstract DomPath Parent {
            get;
        }

        public static DomPath Root {
            get {
                return _root;
            }
        }

        public abstract IReadOnlyList<DomPathExpression> Expressions {
            get;
        }

        public static new DomPath Parse(string text) {
            DomPath result;
            if (TryParse(text, out result)) {
                return result;
            }
            throw Failure.NotParsable(nameof(text), typeof(DomPath));
        }

        public static bool TryParse(string text, out DomPath result) {
            var parser = new DomPathParser(text);
            return parser.Parse(out result);
        }

        public DomPath Element(DomName name) {
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }
            return Append(DomPathExpression.Element(name, -1));
        }

        public DomPath Element(string name) {
            return Element(name, -1);
        }

        public DomPath Element(DomName name, int index) {
            return Append(DomPathExpression.Element(name, index));
        }

        public DomPath Element(string name, int index) {
            return Element(CreateName(name), index);
        }

        public DomPath Attribute(DomName name) {
            return Append(DomPathExpression.Attribute(name));
        }

        public DomPath Attribute(string name) {
            return Attribute(CreateName(name));
        }

        public DomPath DescendantHasAttribute(DomName name) {
            return Append(DomPathExpression.DescendantHasAttribute(name));
        }

        public DomPath DescendantHasAttribute(string name) {
            return DescendantHasAttribute(CreateName(name));
        }

        public DomPath DescendantHasAttributeValue(DomName name, string value) {
            return Append(DomPathExpression.DescendantHasAttributeValue(name, value));
        }

        public DomPath DescendantHasAttributeValue(string name, string value) {
            return DescendantHasAttributeValue(CreateName(name), value);
        }

        public DomPath Id(string id) {
            return Append(DomPathExpression.ById(id));
        }

        private DomPath Append(DomPathExpression append) {
            return new DomPathLeaf(this, append);
        }

        public sealed override string ToString() {
            var sb = new StringBuilder();
            Append(sb);
            return sb.ToString();
        }

        private DomName CreateName(string name) {
            return DomName.Create(name);
        }

        internal abstract void Append(StringBuilder sb);
    }
}
