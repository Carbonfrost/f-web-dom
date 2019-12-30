//
// Copyright 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    [DomProviderFactoryUsage]
    public class CustomDomProviderFactory : DomProviderFactory {

        public override string GenerateDefaultName(Type providerObjectType) {
            return "hey:" + providerObjectType.Name;
        }

        public override bool IsProviderObject(Type providerObjectType) {
            // Notice that `DerivedCustomAttribute' isn't here, but its base class is
            // which will allow GenerateDefaultName to be invoked
            return providerObjectType == typeof(CustomAttribute);
        }

    }

    public class CustomAttribute : DomAttribute<CustomAttribute> {
    }

    public class DerivedCustomAttribute : CustomAttribute {
    }

    public class UnknownCustomAttribute : DomAttribute<UnknownCustomAttribute> {
    }

    public class CustomDomProviderFactoryTests {

        [Fact]
        public void Constructor_for_default_attribute_should_invoke_provider() {
            var attr = new CustomAttribute();
            Assert.Equal("hey:CustomAttribute", attr.Name);
        }

        [Fact]
        public void Constructor_for_derived_default_attribute_should_invoke_provider() {
            var attr = new DerivedCustomAttribute();
            Assert.Equal("hey:DerivedCustomAttribute", attr.Name);
        }

        [Fact]
        public void Constructor_for_unknown_should_cause_error() {
            var ex = Record.Exception(() => new UnknownCustomAttribute());
            Assert.NotNull(ex);
            // Assert.IsAssignableFrom<ArgumentException>(ex);
        }

    }

}
