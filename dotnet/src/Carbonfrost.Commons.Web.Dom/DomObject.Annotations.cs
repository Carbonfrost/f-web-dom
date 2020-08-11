//
// Copyright 2014, 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomObject {

        private FrugalList<object> _annotations = FrugalList<object>.Empty;

        // Only for tests
        internal FrugalList<object> AnnotationList {
            get {
                return _annotations;
            }
        }

        public DomObject AddAnnotation(object annotation) {
            if (annotation == null) {
                throw new ArgumentNullException(nameof(annotation));
            }
            if (annotation is IDomObjectReferenceLifecycle cl) {
                cl.Attaching(this);
            }
            _annotations = _annotations.Add(annotation);
            return this;
        }

        public bool HasAnnotation<T>() where T : class {
            return _annotations.OfType<T>().Any();
        }

        public bool HasAnnotation(object instance) {
            return _annotations.Contains(instance);
        }

        public T Annotation<T>() where T : class {
            return _annotations.OfType<T>().FirstOrDefault();
        }

        public object Annotation(Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            return _annotations.OfType(type).FirstOrDefault();
        }

        public DomObject AddAnnotations(IEnumerable<object> annotations) {
            if (annotations != null) {
                foreach (var anno in annotations) {
                    AddAnnotation(anno);
                }
            }
            return this;
        }

        public IEnumerable<object> Annotations() {
            return Annotations<object>();
        }

        public IEnumerable<T> Annotations<T>() where T : class {
            return _annotations.OfType<T>();
        }

        public IEnumerable<object> Annotations(Type type) {
            return _annotations.OfType(type);
        }

        public DomObject RemoveAnnotations<T>() where T : class {
            _annotations = _annotations.RemoveOfType(typeof(T), DetachLifecycle);
            return this;
        }

        public DomObject RemoveAnnotations(Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            _annotations = _annotations.RemoveOfType(type, DetachLifecycle);
            return this;
        }

        public DomObject RemoveAnnotation(object value) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            if (value is IDomObjectReferenceLifecycle cl) {
                cl.Detaching();
            }
            _annotations = _annotations.Remove(value);
            return this;
        }

        internal void CopyAnnotationsFrom(FrugalList<object> other) {
            _annotations = other.Clone();
            foreach (var anno in _annotations.OfType<IDomObjectReferenceLifecycle>()) {
                anno.Attaching(this);
            }
        }

        internal T AnnotationRecursive<T>(T defaultValue) where T: class {
            var uc = Annotation<T>();
            if (uc == null) {
                return OwnerNode == null
                    ? defaultValue
                    : OwnerNode.AnnotationRecursive<T>(defaultValue);
            }
            return uc;
        }

        private static void DetachLifecycle(object anno) {
            if (anno is IDomObjectReferenceLifecycle cl) {
                cl.Detaching();
            }
        }

    }
}
