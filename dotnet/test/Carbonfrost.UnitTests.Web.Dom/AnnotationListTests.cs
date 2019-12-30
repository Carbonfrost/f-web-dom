//
// - AnnotationListTests.cs -
//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Web.Dom;
using Carbonfrost.Commons.Spec;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class AnnotationListTests {

        class AnnotationClass {
            internal static readonly AnnotationClass Value = new AnnotationClass();
        }

        [Fact]
        public void test_add_annotation_nominal() {
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
    }
}


