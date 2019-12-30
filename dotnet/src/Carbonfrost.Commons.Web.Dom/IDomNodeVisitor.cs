//
// - IDomNodeVisitor.cs -
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

    public interface IDomNodeVisitor {
        void Visit(DomElement element);
        void Visit(DomAttribute attribute);
        void Visit(DomDocument document);
        void Visit(DomCDataSection section);
        void Visit(DomComment comment);
        void Visit(DomText text);
        void Visit(DomProcessingInstruction instruction);
        void Visit(DomNotation notation);
        void Visit(DomDocumentType documentType);
        void Visit(DomEntityReference reference);
        void Visit(DomEntity entity);
        void Visit(DomDocumentFragment fragment);
    }

    public interface IDomNodeVisitor<TArgument, TResult> {
        TResult Visit(DomElement element, TArgument argument);
        TResult Visit(DomAttribute attribute, TArgument argument);
        TResult Visit(DomDocument document, TArgument argument);
        TResult Visit(DomCDataSection section, TArgument argument);
        TResult Visit(DomComment comment, TArgument argument);
        TResult Visit(DomText text, TArgument argument);
        TResult Visit(DomProcessingInstruction instruction, TArgument argument);
        TResult Visit(DomNotation notation, TArgument argument);
        TResult Visit(DomDocumentType documentType, TArgument argument);
        TResult Visit(DomEntityReference reference, TArgument argument);
        TResult Visit(DomEntity entity, TArgument argument);
        TResult Visit(DomDocumentFragment fragment, TArgument argument);
    }
}
