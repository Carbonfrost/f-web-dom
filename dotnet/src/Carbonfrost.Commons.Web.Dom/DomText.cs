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

using System.Text.RegularExpressions;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomText : DomCharacterData, IDomCharacterData<DomText> {

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

        public sealed override DomNodeType NodeType {
            get {
                return DomNodeType.Text;
            }
        }

        public sealed override string NodeName {
            get {
                return "#text";
            }
        }

        public new DomText AppendData(string data) {
            return (DomText) base.AppendData(data);
        }

        public new DomText SetData(string value) {
            return (DomText) base.SetData(value);
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
            var result = OwnerDocument.CreateText(Data);
            result.CopyAnnotationsFrom(AnnotationList);
            return result;
        }
    }
}
