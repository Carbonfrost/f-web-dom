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

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomAttribute : IDomAttributeManipulation<DomAttribute> {

        public DomAttribute After(DomAttribute nextAttribute) {
            if (nextAttribute == null) {
                throw new ArgumentNullException(nameof(nextAttribute));
            }
            SiblingAttributes.Insert(AttributePosition + 1, nextAttribute);
            return this;
        }

        public DomAttribute After(params DomAttribute[] nextAttributes) {
            if (nextAttributes == null) {
                throw new ArgumentNullException(nameof(nextAttributes));
            }
            return After((IEnumerable<DomAttribute>) nextAttributes);
        }

        public DomAttribute After(IEnumerable<DomAttribute> nextAttributes) {
            if (nextAttributes == null) {
                throw new ArgumentNullException(nameof(nextAttributes));
            }

            SiblingAttributes.InsertRange(AttributePosition + 1, nextAttributes);
            return this;
        }

        public DomAttribute Before(DomAttribute previousAttribute) {
            if (previousAttribute == null) {
                throw new ArgumentNullException(nameof(previousAttribute));
            }
            SiblingAttributes.Insert(AttributePosition, previousAttribute);
            return this;
        }

        public DomAttribute Before(params DomAttribute[] previousAttributes) {
            if (previousAttributes == null) {
                throw new ArgumentNullException(nameof(previousAttributes));
            }
            return Before((IEnumerable<DomAttribute>) previousAttributes);
        }

        public DomAttribute Before(IEnumerable<DomAttribute> previousAttributes) {
            if (previousAttributes == null) {
                throw new ArgumentNullException(nameof(previousAttributes));
            }
            SiblingAttributes.InsertRange(AttributePosition, previousAttributes);
            return this;
        }

        public DomAttribute InsertAfter(DomAttribute other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return other.After(this);
        }

        public DomAttribute InsertBefore(DomAttribute other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return other.Before(this);
        }
    }
}
