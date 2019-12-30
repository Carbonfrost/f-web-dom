//
// - DomComment{T}.cs -
//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

    public abstract class DomComment<TComment>
        : DomComment where TComment : DomComment<TComment>
    {

        protected DomComment() {
        }

        internal override void AcceptVisitor(IDomNodeVisitor visitor)
        {
            var a = visitor as IDomCommentVisitor<TComment>;
            if (a == null)
                base.AcceptVisitor(visitor);
            else
                a.Dispatch((TComment) this);
        }

        internal override TResult AcceptVisitor<TArgument, TResult>(IDomNodeVisitor<TArgument, TResult> visitor, TArgument argument) {
            var a = visitor as IDomCommentVisitor<TComment, TArgument, TResult>;
            if (a == null)
                return base.AcceptVisitor(visitor, argument);
            else
                return a.Dispatch((TComment) this, argument);
        }
    }
}
