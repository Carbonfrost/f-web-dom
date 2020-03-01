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
using System.Text;

namespace Carbonfrost.Commons.Web.Dom {

    class OuterTextVisitor : DomNodeVisitor, ITextVisitor {

        private StringBuilder _sb;

        public void SetBuffer(StringBuilder stringBuilder) {
            _sb = stringBuilder;
        }

        protected override void VisitElement(DomElement element) {
            if (element == null) {
                throw new ArgumentNullException(nameof(element));
            }
            Visit(element.ChildNodes);
        }

        protected override void VisitDocument(DomDocument document) {
            if (document == null) {
                throw new ArgumentNullException(nameof(document));
            }
            Visit(document.ChildNodes);
        }

        protected override void VisitText(DomText text) {
            if (text == null) {
                throw new ArgumentNullException(nameof(text));
            }

            _sb.Append(DomEscaper.Default.Escape(text.Data));
        }
    }
}
