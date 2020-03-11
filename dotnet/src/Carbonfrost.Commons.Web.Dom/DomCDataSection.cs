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

    public partial class DomCDataSection : DomCharacterData {

        protected internal DomCDataSection() : base() {
        }

        protected override DomNodeCollection DomChildNodes {
            get {
                return DomNodeCollection.Empty;
            }
        }

        protected override DomAttributeCollection DomAttributes {
            get {
                return null;
            }
        }

        public override DomNodeType NodeType {
            get {
                return DomNodeType.CDataSection;
            }
        }

        public override string NodeName {
            get {
                return "#cdata-section";
            }
        }

        public new DomCDataSection SplitText(int index) {
            return (DomCDataSection) base.SplitText(index);
        }

        public new DomCDataSection SplitText(int index, int length) {
            return (DomCDataSection) base.SplitText(index, length);
        }

        public new DomCDataSection SplitText(Regex pattern) {
            return (DomCDataSection) base.SplitText(pattern);
        }
    }
}
