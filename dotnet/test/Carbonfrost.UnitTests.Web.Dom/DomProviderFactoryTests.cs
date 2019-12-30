//
// Copyright 2014, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomProviderFactoryTests {

        public IEnumerable<Type> ProviderObjectTypes {
            get {
                return new [] {
                    typeof(DomNode),
                    typeof(DomElement),
                    typeof(DomProcessingInstruction),
                    typeof(DomDocument),
                    typeof(DomDocumentFragment),
                    typeof(DomText),
                    typeof(DomCharacterData),
                    typeof(DomCDataSection),
                    typeof(DomEntity),
                    typeof(DomEntityReference),
                    typeof(DomNotation),

                    typeof(DomWriter),
                    typeof(DomReader),
                };
            }
        }

        public IEnumerable<Type> NonProviderObjectTypes {
            get {
                return new [] {
                    typeof(DomNodeType),
                    typeof(DomStringTokenList),
                };
            }
        }

        [Fact]
        public void FromName_should_obtain_default_by_name() {
            Assert.Equal(DomProviderFactory.Default, DomProviderFactory.FromName("Default"));
        }

        [Theory]
        [PropertyData("ProviderObjectTypes")]
        public void IsProviderObject_should_be_true(Type type) {
            Assert.True(DomProviderFactory.Default.IsProviderObject(type));
        }

        [Theory]
        [PropertyData("NonProviderObjectTypes")]
        public void IsProviderObject_should_be_false_for_non(Type type) {
            Assert.False(DomProviderFactory.Default.IsProviderObject(type));
        }
    }
}
