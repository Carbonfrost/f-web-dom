//
// - DefaultAnnotationList.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using System.Reflection;

namespace Carbonfrost.Commons.Web.Dom {

    class DefaultAnnotationList : AnnotationList {

        private object[] annotations;

        public DefaultAnnotationList(object one, object two) {
            this.annotations = new [] { one, two };
        }

        public override IEnumerable<T> OfType<T>() {
            return annotations.OfType<T>();
        }

        public override bool Contains(object annotation) {
            return annotations.Contains(annotation);
        }

        public override AnnotationList Add(object annotation) {
            object[] annotations = this.annotations;
            int index = 0;

            // Search for an empty space
            while ((index < annotations.Length) && (annotations[index] != null)) {
                index++;
            }

            // Ensure capacity
            if (index == annotations.Length) {
                Array.Resize(ref annotations, index * 2);
                this.annotations = annotations;
            }

            annotations[index] = annotation;
            return this;
        }

        public override AnnotationList RemoveOfType(Type type) {
            for (int i = 0; i < annotations.Length; i++) {
                if (type.IsInstanceOfType(annotations[i]))
                    annotations[i] = null;
            }
            return this;
        }

        public override AnnotationList Remove(object annotation) {
            for (int i = 0; i < annotations.Length; i++) {
                if (object.Equals(annotations[i], annotation))
                    annotations[i] = null;
            }
            return this;
        }

        public override IEnumerable<object> OfType(Type type) {
            return this.annotations.Where(type.GetTypeInfo().IsInstanceOfType);
        }
    }
}
