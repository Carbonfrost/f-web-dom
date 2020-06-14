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
using System.Reflection;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomNodeFactoryTests {

        public IEnumerable<MethodInfo> ApiConventionMethods {
            get {
                return typeof(DomNodeFactory).GetInterfaceMap(
                    typeof(IDomNodeFactoryApiConventions)
                ).TargetMethods;
            }
        }

        [Theory]
        [PropertyData(nameof(ApiConventionMethods))]
        public void ApiConventions_are_nonvirtual(MethodInfo convention) {
            Assert.True(convention.IsFinal);
        }

        class PAttribute : DomAttribute {
            public PAttribute() : base("myname") {}
        }

        class PAttributeWithName : DomAttribute {
            public PAttributeWithName(string name) : base(name) {}
        }

        [Fact]
        public void CreateAttribute_using_late_bound_instancing() {
            var fac = new DomNodeFactory(
                new FDomNodeTypeProvider((name) => typeof(PAttribute))
            );
            Assert.IsInstanceOf<PAttribute>(fac.CreateAttribute("myname"));
        }

        [Fact]
        public void CreateAttribute_using_late_bound_instancing_has_name() {
            var fac = new DomNodeFactory(
                new FDomNodeTypeProvider((name) => typeof(PAttributeWithName))
            );
            Assert.IsInstanceOf<PAttributeWithName>(fac.CreateAttribute("expected"));
            Assert.Equal("expected", fac.CreateAttribute("expected").LocalName);

        }

        private class FDomNodeTypeProvider : DomNodeTypeProvider {
            private readonly Func<DomName, Type> _thunk;

            public FDomNodeTypeProvider(Func<DomName, Type> p) {
                _thunk = p;
            }

            public override Type GetAttributeNodeType(DomName name) {
                return _thunk(name);
            }
        }
    }
}
