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

using System;
using System.Collections;
using System.Collections.Generic;

namespace Carbonfrost.Commons.Web.Dom {

    class CompositeDomNodeTypeProvider : IDomNodeTypeProvider, IEnumerable<IDomNodeTypeProvider> {

        readonly IDomNodeTypeProvider[] _items;

        public CompositeDomNodeTypeProvider(IDomNodeTypeProvider[] items) {
            _items = items;
        }

        public Type GetAttributeNodeType(DomName name) {
            return _items.FirstNonNull(t => t.GetAttributeNodeType(name));
        }

        public Type GetElementNodeType(DomName name) {
            return _items.FirstNonNull(t => t.GetElementNodeType(name));
        }

        public Type GetProcessingInstructionNodeType(string target) {
            return _items.FirstNonNull(t => t.GetProcessingInstructionNodeType(target));
        }

        public IEnumerator<IDomNodeTypeProvider> GetEnumerator() {
            return ((IEnumerable<IDomNodeTypeProvider>) _items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public DomName GetAttributeName(Type attributeType) {
            return _items.FirstNonNull(t => t.GetAttributeName(attributeType));
        }

        public DomName GetElementName(Type elementType) {
            return _items.FirstNonNull(t => t.GetElementName(elementType));
        }

        public string GetProcessingInstructionTarget(Type processingInstructionType) {
            return _items.FirstNonNull(t => t.GetProcessingInstructionTarget(processingInstructionType));
        }
    }
}
