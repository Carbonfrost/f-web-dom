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

using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    [Composable]
    public interface IDomNodeFactory : IDomNodeTypeProvider {
        DomAttribute CreateAttribute(string name);
        DomCDataSection CreateCDataSection();
        DomComment CreateComment();
        DomDocumentFragment CreateDocumentFragment();
        DomDocumentType CreateDocumentType(string name);
        DomElement CreateElement(string name);
        DomEntity CreateEntity(string name);
        DomNotation CreateNotation(string name);
        DomEntityReference CreateEntityReference(string name);
        DomProcessingInstruction CreateProcessingInstruction(string target);
        DomText CreateText();
    }

    interface IDomNodeFactoryApiConventions : IDomNodeFactory {
        // Provides glue for contentions on the IDomNodeFactory implemented in this
        // assembly

        DomAttribute CreateAttribute(string name, IDomValue value);
        DomAttribute CreateAttribute(string name, string value);
        DomCDataSection CreateCDataSection(string data);
        DomComment CreateComment(string data);
        DomDocumentType CreateDocumentType(string name, string publicId, string systemId);
        DomProcessingInstruction CreateProcessingInstruction(string target, string data);
        DomText CreateText(string data);
    }
}
