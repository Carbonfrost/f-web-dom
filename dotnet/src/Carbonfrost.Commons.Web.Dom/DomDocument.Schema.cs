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

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomDocument {

        public DomSchema Schema {
            get;
            private set;
        }

        public DomDocument WithSchema(DomSchema schema) {
            var result = this; // TODO Should clone the instance
            result.Schema = schema;
            return result;
        }

        private DomAttribute ApplySchema(DomAttribute attr) {
            if (Schema == null) {
                return attr;
            }
            var def = Schema.AttributeDefinitions[attr.Name];
            if (def != null && def.ValueType != null) {
                attr.DomValue = DomValue.Create(def.ValueType);
            }
            return attr;
        }

        private DomElement ApplySchema(DomElement attr) {
            return attr;
        }
    }

}
