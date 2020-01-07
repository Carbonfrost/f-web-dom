//
// Copyright 2013, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

    public class DomNodeSelectTests {

        [Fact]
        public void Select_elements() {
            DomDocument doc = new DomDocument();
            doc.LoadXml("<section> <div /> </section>");
            Assert.Equal(1, doc.DocumentElement.Select("div").Count());
        }

        [Fact]
        public void Select_class() {
            DomDocument doc = new DomDocument();
            doc.LoadXml("<section> <div class='a v' /> <div /> </section>");
            Assert.Equal(1, doc.DocumentElement.Select(".a").Count());
        }

        [Fact]
        public void Select_classes() {
            DomDocument doc = new DomDocument();
            doc.LoadXml("<section> <div class='a v' /> <div /> </section>");
            Assert.Equal(1, doc.DocumentElement.Select(".a.v").Count());
        }

        [Fact]
        public void Select_attribute() {
            DomDocument doc = new DomDocument();
            doc.LoadXml("<section> <div style='a v' /> <div /> </section>");
            Assert.Equal(1, doc.DocumentElement.Select("[style=a v]").Count());
        }

        [Fact]
        public void Select_attribute_starts_with() {
            DomDocument doc = new DomDocument();
            doc.LoadXml("<section> <div style='a v' /> <div /> </section>");
            Assert.Equal(1, doc.DocumentElement.Select("[style^=a]").Count());
        }

        [Fact]
        public void Select_attribute_exists() {
            DomDocument doc = new DomDocument();
            doc.LoadXml("<section> <div style='a v' /> <div /> </section>");
            Assert.Equal(1, doc.DocumentElement.Select("[style]").Count());
        }

        [Fact]
        public void Select_id() {
            DomDocument doc = new DomDocument();
            doc.LoadXml("<section> <div id='m' /> <div /> </section>");
            Assert.Equal(1, doc.DocumentElement.Select("#m").Count());
        }
    }
}