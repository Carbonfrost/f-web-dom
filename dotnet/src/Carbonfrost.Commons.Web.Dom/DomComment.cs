//
// Copyright 2013, 2019-2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomComment : DomCharacterData {

        public string Text {
            get {
                return Data;
            }
            set {
                Data = value;
            }
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

        public override string TextContent {
            get { return Data; }
            set {
                Data = value;
            }
        }

        public override string NodeName {
            get {
                return "#comment";
            }
        }

        protected internal DomComment() : base() {
        }

        public override DomNodeType NodeType {
            get {
                return DomNodeType.Comment;
            }
        }

        protected override DomNode CloneCore() {
            var result = OwnerDocument.CreateComment(Data);
            result.CopyAnnotationsFrom(AnnotationList);
            return result;
        }
    }
}
