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

using System.Collections.Generic;

namespace Carbonfrost.Commons.Web.Dom {

    interface IDomNodeDefinitionCollection<T> : ICollection<T> where T : DomNodeDefinition {
        T this[string name] { get; }
        T this[DomName name] { get; }
        T AddNew(string name);
        T AddNew(DomName name);
        bool Contains(string name);
        bool Contains(DomName name);
    }
}
