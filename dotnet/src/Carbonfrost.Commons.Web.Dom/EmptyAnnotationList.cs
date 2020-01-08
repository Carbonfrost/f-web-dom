//
// Copyright 2013, 2019 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Collections.Generic;

namespace Carbonfrost.Commons.Web.Dom {

    class EmptyAnnotationList : AnnotationList {

        public override AnnotationList Add(object annotation) {
            return new SingletonAnnotationList(annotation);
        }

        public override IEnumerable<T> OfType<T>() {
            return Array.Empty<T>();
        }

        public override AnnotationList RemoveOfType(Type type) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }

            return this;
        }

        public override bool Contains(object annotation) {
            return false;
        }

        public override AnnotationList Remove(object annotation) {
            if (annotation == null) {
                throw new ArgumentNullException("annotation");
            }

            return this;
        }

        public override IEnumerable<object> OfType(Type type) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }

            return Array.Empty<object>();
        }
    }
}
