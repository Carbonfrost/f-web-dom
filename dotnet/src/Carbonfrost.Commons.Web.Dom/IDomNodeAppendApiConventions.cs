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

namespace Carbonfrost.Commons.Web.Dom {

    interface IDomNodeAppendApiConventions {
        // Helps identify API conventions for DomNode and DomObjectQuery creating and appending elements.
        // Can append/prepend all except notation, entity, entity reference, fragment

        DomElement AppendElement(string name);
        DomAttribute AppendAttribute(string name, object value);
        DomText AppendText(string data);
        DomCDataSection AppendCDataSection(string data);
        DomProcessingInstruction AppendProcessingInstruction(string target, string data);
        DomComment AppendComment(string data);
        DomDocumentType AppendDocumentType(string name);

        DomElement PrependElement(string name);
        DomAttribute PrependAttribute(string name, object value);
        DomText PrependText(string data);
        DomCDataSection PrependCDataSection(string data);
        DomProcessingInstruction PrependProcessingInstruction(string target, string data);
        DomComment PrependComment(string data);
        DomDocumentType PrependDocumentType(string name);
    }
}
