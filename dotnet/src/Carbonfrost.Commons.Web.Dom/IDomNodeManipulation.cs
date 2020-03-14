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

using System.Collections.Generic;

namespace Carbonfrost.Commons.Web.Dom {

    // We're using the JQuery style naming here.
    // .NET Framework is inconsistent between XML, XPath, and XLinq,
    // which makes it difficult to choose which is "best".

    interface IDomNodeManipulation<TNode> {
        // Add the specified node after the current node
        // InsertAfter is equivalent to other.After(this);
        TNode After(DomNode nextSibling);
        TNode After(params DomNode[] nextSiblings);
        TNode After(IEnumerable<DomNode> nextSiblings);
        TNode After(string markup);
        TNode InsertAfter(DomNode other);

        // -- same
        TNode Before(DomNode previousSibling);
        TNode Before(params DomNode[] previousSiblings);
        TNode Before(IEnumerable<DomNode> previousSiblings);
        TNode Before(string markup);
        TNode InsertBefore(DomNode other);

        DomWriter Append();
        TNode Append(DomNode child);
        TNode Append(params DomNode[] children);
        TNode Append(IEnumerable<DomNode> children);
        TNode Append(string markup);
        TNode AppendTo(DomNode parent);

        DomWriter Prepend();
        TNode Prepend(DomNode child);
        TNode Prepend(params DomNode[] children);
        TNode Prepend(IEnumerable<DomNode> children);
        TNode Prepend(string markup);
        TNode PrependTo(DomNode parent);

        // Remove child nodes
        TNode Empty();
        TNode RemoveChildNodes();

        TNode Remove();
        TNode RemoveSelf();
        TNode RemoveAttributes();

        TNode SetName(string name);

        TNode Attribute(string name, object value);
        TNode RemoveAttribute(string name);
        TNode AddClass(string name);
        TNode RemoveClass(string name);

        TNode Wrap(string element);
        TNode Wrap(DomNode newParent);
    }
}
