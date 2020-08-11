//
// Copyright 2014, 2019-2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Web.Dom;
using Carbonfrost.Commons.Spec;
using System.Linq;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomObjectAnnotationTests {

        class AnnotationClass {
            internal static readonly AnnotationClass Value = new AnnotationClass();
        }

        [Fact]
        public void AddAnnotation_nominal() {
            DomDocument d = new DomDocument();
            d.AddAnnotation(AnnotationClass.Value);

            Assert.True(d.HasAnnotation<AnnotationClass>());
            Assert.NotEmpty(d.Annotations<AnnotationClass>());
            Assert.Empty(d.Annotations<Uri>());
            Assert.Equal(AnnotationClass.Value, d.Annotation<AnnotationClass>());
        }

        [Fact]
        public void AddAnnotations_should_add_all_nominal() {
            DomDocument d = new DomDocument();
            d.AddAnnotations(new object [] { AnnotationClass.Value, Glob.Anything, string.Empty });
            Assert.SetEqual(new object [] { AnnotationClass.Value, Glob.Anything, string.Empty }, d.Annotations());
        }

        [Fact]
        public void DomObject_Clone_will_clone_annotations_copied_to_new_node() {
            var doc = new DomDocument();
            var anno = new FLifecycleAnnotation();

            var element = doc.CreateElement("a").AddAnnotation(anno);
            var clone = element.Clone();

            Assert.NotNull(clone.Annotation<FLifecycleAnnotation>());
            Assert.NotSame(anno, clone.Annotation<FLifecycleAnnotation>());
        }

        [Fact]
        public void CopyAnnotationsFrom_will_process_lifecycle_events_on_clones() {
            var doc = new DomDocument();
            var anno = new FLifecycleAnnotation();

            var element = doc.CreateElement("a").AddAnnotation(anno);
            var clone = element.Clone();

            var clonedAnnotation = clone.Annotation<FLifecycleAnnotation>();
            Assume.NotNull(clonedAnnotation);
            Assert.Equal(1, clonedAnnotation.AttachingCallCount);
            Assert.Same(clone, clonedAnnotation.LastAttachingArguments);
        }

        [Fact]
        public void AddAnnotation_will_attach_node() {
            var doc = new DomDocument();
            var anno = new FLifecycleAnnotation();

            var element = doc.CreateElement("a").AddAnnotation(anno);
            Assert.Same(element, anno.LastAttachingArguments);
            Assert.Equal(1, anno.AttachingCallCount);
        }

        [Fact]
        public void RemoveAnnotation_will_detach_node() {
            var doc = new DomDocument();
            var anno = new FLifecycleAnnotation();
            doc.CreateElement("a").RemoveAnnotation(anno);

            Assert.Equal(1, anno.DetachingCallCount);
        }

        [Fact]
        public void RemoveAnnotationsOfT_will_detach_node() {
            var doc = new DomDocument();
            var anno = new FLifecycleAnnotation();
            doc.CreateElement("a")
                .AddAnnotation(anno)
                .RemoveAnnotations<FLifecycleAnnotation>();

            Assert.Equal(1, anno.DetachingCallCount);
        }

        class FLifecycleAnnotation : IDomObjectReferenceLifecycle {
            public int AttachingCallCount {
                get;
                private set;
            }

            public int DetachingCallCount {
                get;
                private set;
            }

            public DomObject LastAttachingArguments {
                get;
                private set;
            }

            public void Attaching(DomObject instance) {
                LastAttachingArguments = instance;
                AttachingCallCount ++;
            }

            public object Clone() {
                return new FLifecycleAnnotation();
            }

            public void Detaching() {
                DetachingCallCount ++;
            }
        }
    }
}


