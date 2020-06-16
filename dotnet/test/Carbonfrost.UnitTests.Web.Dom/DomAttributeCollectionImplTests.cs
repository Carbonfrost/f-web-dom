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
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public class DomAttributeCollectionImplTests {

        [Fact]
        public void Add_will_detect_duplicates_on_map_key_equality() {
            var list = new List<DomAttribute>();
            var subject = new DomAttributeCollectionImpl(list, DomNameComparer.IgnoreCase);
            subject.Add(new DomAttribute(DomName.Create("a")));

            var ex = Record.Exception(() => subject.Add(new DomAttribute(DomName.Create("A"))));
            Assert.Contains("already exists", ex.Message);
        }

        [Fact]
        public void ReadOnly_is_read_only() {
            Assert.True(DomAttributeCollectionImpl.ReadOnly.IsReadOnly);
        }

        [Fact]
        public void ReadOnly_cannot_add_or_clear() {
            DomDocument doc = new DomDocument();
            var html = doc.AppendElement("html");
            var attr = html.AppendAttribute("lang", "en");

            Assert.Throws<NotSupportedException>(() => {
                DomAttributeCollectionImpl.ReadOnly.Add(attr);
            });

            Assert.Throws<NotSupportedException>(() => {
                DomAttributeCollectionImpl.ReadOnly.Clear();
            });
        }

    }
}
