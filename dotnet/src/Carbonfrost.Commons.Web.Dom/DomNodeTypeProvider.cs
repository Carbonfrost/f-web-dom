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
using System.Collections.Generic;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    [Providers, Composable]
    public class DomNodeTypeProvider : IDomNodeTypeProvider {

        public static readonly IDomNodeTypeProvider Default = new DomNodeTypeProvider();
        public static readonly IDomNodeTypeProvider Null = new NullImpl();

        [DomNodeTypeProviderUsage]
        public static IDomNodeTypeProvider Compose(params IDomNodeTypeProvider[] items) {
            return Compose((IEnumerable<IDomNodeTypeProvider>) items);
        }

        public static IDomNodeTypeProvider Compose(IEnumerable<IDomNodeTypeProvider> items) {
            return Utility.OptimalComposite(
                items, m => new CompositeDomNodeTypeProvider(m), Null
            );
        }

        public virtual Type GetAttributeNodeType(string name) {
            return typeof(DomAttribute);
        }

        public virtual Type GetElementNodeType(string name) {
            return typeof(DomElement);
        }

        public virtual Type GetProcessingInstructionNodeType(string name) {
            return typeof(DomProcessingInstruction);
        }

        public class NullImpl : IDomNodeTypeProvider {
            Type IDomNodeTypeProvider.GetAttributeNodeType(string name) {
                return null;
            }

            Type IDomNodeTypeProvider.GetElementNodeType(string name) {
                return null;
            }

            Type IDomNodeTypeProvider.GetProcessingInstructionNodeType(string target) {
                return null;
            }
        }
    }
}
