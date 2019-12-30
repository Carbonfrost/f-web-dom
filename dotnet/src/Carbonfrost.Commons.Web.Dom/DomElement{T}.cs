//
// - DomElement{T}.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomElement<TElement> : DomElement
        where TElement : DomElement<TElement>
    {

        protected internal DomElement() : base() {}
        protected internal DomElement(string name) : base(name) {}

        internal override void AcceptVisitor(IDomNodeVisitor visitor) {
            var a = visitor as IDomElementVisitor<TElement>;
            if (a == null)
                base.AcceptVisitor(visitor);
            else
                a.Dispatch((TElement) this);
        }

        internal override TResult AcceptVisitor<TArgument, TResult>(IDomNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            var a = visitor as IDomElementVisitor<TElement, TArgument, TResult>;
            if (a == null)
                return base.AcceptVisitor(visitor, argument);
            else
                return a.Dispatch((TElement) this, argument);
        }
    }

}
