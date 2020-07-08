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
using Carbonfrost.Commons.Web.Dom;
using Carbonfrost.Commons.Spec;
using System.Collections.Generic;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class FrugalListTests {

        [Theory]
        [InlineData(0, typeof(FrugalList<string>.EmptyState))]
        [InlineData(1, typeof(FrugalList<string>.SingletonState))]
        [InlineData(2, typeof(FrugalList<string>.ArrayState))]
        public void State_should_have_correct_type(int count, Type expected) {
            var list = FrugalList<string>.Empty;
            for (int i = 0; i < count; i++) {
                list = list.Add(i.ToString());
            }

            Assert.IsInstanceOf(expected, list);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 4)]
        [InlineData(4, 4)]
        [InlineData(5, 8)]
        [InlineData(8, 8)]
        [InlineData(9, 16)]
        public void ArrayState_should_grow_capacity(int count, int expectedCapacity) {
            var list = FrugalList<string>.Empty;
            var expected = new List<string>();
            for (int i = 0; i < count; i++) {
                list = list.Add(i.ToString());
                expected.Add(i.ToString());
            }

            Assert.Equal(expected, list);
            Assert.Equal(count, list.Count);

            var array = list as FrugalList<string>.ArrayState;
            if (array != null) {
                Assert.Equal(expectedCapacity, array.Capacity);
            }
        }

    }
}


