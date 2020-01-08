//
// Copyright 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

    public class DomAttributeExplicitOperatorTests {

        public IEnumerable<Func<DomAttribute, object>> NullableOperators {
            get {
                yield return (attr) => (DateTime?) attr;
                yield return (attr) => (bool?) attr;
                yield return (attr) => (Guid?) attr;
                yield return (attr) => (int?) attr;
                yield return (attr) => (TimeSpan?) attr;
                yield return (attr) => (DateTimeOffset?) attr;
                yield return (attr) => (long?) attr;
                yield return (attr) => (ulong?) attr;
                yield return (attr) => (float?) attr;
                yield return (attr) => (double?) attr;
                yield return (attr) => (decimal?) attr;
                yield return (attr) => (uint?) attr;

                // String is a special case where (string) ((DomAttribute) null) ==> null
                yield return (attr) => (string) attr;
            }
        }

        public IEnumerable<Func<DomAttribute, object>> NonNullableOperators {
            get {
                yield return (attr) => (bool) attr;
                yield return (attr) => (Guid) attr;
                yield return (attr) => (int) attr;
                yield return (attr) => (DateTime) attr;
                yield return (attr) => (DateTimeOffset) attr;
                yield return (attr) => (decimal) attr;
                yield return (attr) => (double) attr;
                yield return (attr) => (long) attr;
                yield return (attr) => (float) attr;
                yield return (attr) => (TimeSpan) attr;
                yield return (attr) => (uint) attr;
                yield return (attr) => (ulong) attr;
            }
        }

        [Theory]
        [PropertyData(nameof(NonNullableOperators))]
        public void Operator_cannot_take_null_for_argument_if_nonnullable(Func<DomAttribute, object> op) {
            Assert.Throws<ArgumentNullException>(() => op(null));
        }

        [Theory]
        [PropertyData(nameof(NullableOperators))]
        public void Operator_returns_null_if_nullable(Func<DomAttribute, object> op) {
            Assert.Null(op(null));
        }

        [Fact]
        public void DateTime_explicit_operator_will_convert() {
            Assert.Equal(DateTime.Parse("2020-01-01 15:00:00"), (DateTime) Attribute("2020-01-01 15:00:00"));
        }

        [Fact]
        public void Nullable_DateTime_explicit_operator_will_convert() {
            Assert.Equal(DateTime.Parse("2020-01-01 15:00:00"), (DateTime?) Attribute("2020-01-01 15:00:00"));
        }

        [Fact]
        public void Bool_explicit_operator_will_convert() {
            Assert.Equal(true, (Boolean) Attribute("true"));
        }

        [Fact]
        public void Nullable_bool_explicit_operator_will_convert() {
            Assert.Equal(true, (Boolean?) Attribute("true"));
        }

        [Fact]
        public void Int_explicit_operator_will_convert() {
            Assert.Equal(300, (int) Attribute("300"));
        }

        [Fact]
        public void Nullable_int_explicit_operator_will_convert() {
            Assert.Equal(300, (int?) Attribute("300"));
        }

        static DomAttribute Attribute(string text) {
            var doc = new DomDocument();
            var attr = doc.CreateAttribute("hello");
            attr.Value = text;
            return attr;
        }
    }
}
