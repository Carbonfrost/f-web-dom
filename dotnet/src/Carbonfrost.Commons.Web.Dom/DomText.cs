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
using System.Text.RegularExpressions;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomText : DomCharacterData {

        protected internal DomText() : base() {
        }

        protected override DomAttributeCollection DomAttributes {
            get {
                return null;
            }
        }

        protected override DomNodeCollection DomChildNodes {
            get {
                return DomNodeCollection.Empty;
            }
        }

        public override DomNodeType NodeType {
            get { return DomNodeType.Text; } }

        public override string NodeName {
            get { return "#text"; } }

        public DomText SetValue(string value) {
            Data = value;
            return this;
        }

        public new DomText SplitText(int index) {
            return (DomText) base.SplitText(index);
        }

        public new DomText SplitText(int index, int length) {
            return (DomText) base.SplitText(index, length);
        }

        public new DomText SplitText(Regex pattern) {
            return (DomText) base.SplitText(pattern);
        }

        protected override DomNode CloneCore() {
            return OwnerDocument.CreateText(Data);
        }

        internal override void AcceptVisitor(IDomNodeVisitor visitor) {
            visitor.Visit(this);
        }

        internal override TResult AcceptVisitor<TArgument, TResult>(IDomNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            return visitor.Visit(this, argument);
        }
    }

}
