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

using System;

namespace Carbonfrost.Commons.Web.Dom {

    class NullDomNodeFactory : IDomNodeFactory {

        DomComment IDomNodeFactory.CreateComment() {
            return null;
        }

        DomText IDomNodeFactory.CreateText() {
            return null;
        }

        DomAttribute IDomNodeFactory.CreateAttribute(DomName name) {
            return null;
        }

        DomCDataSection IDomNodeFactory.CreateCDataSection() {
            return null;
        }

        DomDocumentType IDomNodeFactory.CreateDocumentType(string name) {
            return null;
        }

        DomElement IDomNodeFactory.CreateElement(DomName name) {
            return null;
        }

        DomEntityReference IDomNodeFactory.CreateEntityReference(string data) {
            return null;
        }

        DomProcessingInstruction IDomNodeFactory.CreateProcessingInstruction(string target) {
            return null;
        }

        DomDocumentFragment IDomNodeFactory.CreateDocumentFragment() {
            return null;
        }

        DomEntity IDomNodeFactory.CreateEntity(string name) {
            return null;
        }

        DomNotation IDomNodeFactory.CreateNotation(string name) {
            return null;
        }

        Type IDomNodeTypeProvider.GetAttributeNodeType(DomName name) {
            return null;
        }

        Type IDomNodeTypeProvider.GetElementNodeType(DomName name) {
            return null;
        }

        Type IDomNodeTypeProvider.GetProcessingInstructionNodeType(string name) {
            return null;
        }

        DomName IDomNodeTypeProvider.GetAttributeName(Type attributeType) {
            return null;
        }

        DomName IDomNodeTypeProvider.GetElementName(Type elementType) {
            return null;
        }

        string IDomNodeTypeProvider.GetProcessingInstructionTarget(Type processingInstructionType) {
            return null;
        }
    }
}


