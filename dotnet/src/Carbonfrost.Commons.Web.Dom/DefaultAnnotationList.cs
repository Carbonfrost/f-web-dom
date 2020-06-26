//
// Copyright 2013, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Reflection;

namespace Carbonfrost.Commons.Web.Dom {

    sealed class DefaultAnnotationList : AnnotationList {

        private object[] _annotations;

        public DefaultAnnotationList(object one, object two) {
            _annotations = new [] { one, two };
        }

        private DefaultAnnotationList(object[] annotations) {
            _annotations = annotations;
        }

        public override IEnumerable<T> OfType<T>() {
            return _annotations.OfType<T>();
        }

        public override bool Contains(object annotation) {
            return _annotations.Contains(annotation);
        }

        public override AnnotationList Clone() {
            if (_annotations.OfType<IDomObjectReferenceLifecycle>().Any()) {
                return new DefaultAnnotationList(CloneSlow().ToArray());
            }
            return this;
        }

        private IEnumerable<object> CloneSlow() {
            foreach (var o in _annotations) {
                if (o is null) {
                    continue;
                }
                else if (o is IDomObjectReferenceLifecycle lc) {
                    yield return lc.Clone();
                }
                else {
                    yield return o;
                }
            }
        }

        public override AnnotationList Add(object annotation) {
            object[] annotations = _annotations;
            int index = 0;

            // Search for an empty space
            while ((index < annotations.Length) && (annotations[index] != null)) {
                index++;
            }

            // Ensure capacity
            if (index == annotations.Length) {
                Array.Resize(ref annotations, index * 2);
                _annotations = annotations;
            }

            annotations[index] = annotation;
            return this;
        }

        public override AnnotationList RemoveOfType(Type type, Action<object> onRemoved) {
            for (int i = 0; i < _annotations.Length; i++) {
                if (type.IsInstanceOfType(_annotations[i])) {
                    onRemoved(_annotations[i]);
                    _annotations[i] = null;
                }
            }
            return this;
        }

        public override AnnotationList Remove(object annotation) {
            for (int i = 0; i < _annotations.Length; i++) {
                if (object.Equals(_annotations[i], annotation)) {
                    _annotations[i] = null;
                }
            }
            return this;
        }

        public override IEnumerable<object> OfType(Type type) {
            return _annotations.Where(type.GetTypeInfo().IsInstanceOfType);
        }
    }
}
