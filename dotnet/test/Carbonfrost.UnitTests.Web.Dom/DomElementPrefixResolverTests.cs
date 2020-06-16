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

    public class DomElementPrefixResolverTests : TestClass {

        [Fact]
        public void GetNamespace_should_obtain_namespace_from_prefix_nominal() {
            var doc = new DomDocument();
            doc.AppendElement("root").Attribute("xmlns:a", "https://example.com/a");

            IDomNamePrefixResolver subject = new DomElementPrefixResolver(doc.DocumentElement);
            Assert.Equal("https://example.com/a", subject.GetNamespace("a", DomScope.TargetAndAncestors));
        }

        [Fact]
        public void GetPrefixes_should_obtain_prefixes_from_namespace() {
            var doc = new DomDocument();
            doc.AppendElement("root").Attribute("xmlns:a", "https://example.com/a")
                                     .Attribute("xmlns:also", "https://example.com/a");

            IDomNamePrefixResolver subject = new DomElementPrefixResolver(doc.DocumentElement);
            Assert.Equal(new [] { "a", "also" }, subject.GetPrefixes(DomNamespace.Create("https://example.com/a"), DomScope.Target));
        }

        [Fact]
        public void GetPrefixes_should_obtain_prefixes_from_namespace_recursive() {
            var doc = new DomDocument();
            doc.AppendElement("root").Attribute("xmlns:a", "https://example.com/a")
                .AppendElement("e").Attribute("xmlns:also", "https://example.com/a");

            IDomNamePrefixResolver subject = new DomElementPrefixResolver(doc.DocumentElement.FirstChild);
            Assert.Equal(new [] { "also", "a" }, subject.GetPrefixes(DomNamespace.Create("https://example.com/a"), DomScope.TargetAndAncestors));

            subject = new DomElementPrefixResolver(doc.DocumentElement.FirstChild);
            Assert.Equal(new [] { "also" }, subject.GetPrefixes(DomNamespace.Create("https://example.com/a"), DomScope.Target));
        }

        [Fact]
        public void RegisterPrefix_should_assign_prefixes_to_attributes() {
            var doc = new DomDocument();
            doc.AppendElement("root");

            IDomNamePrefixResolver subject = new DomElementPrefixResolver(doc.DocumentElement);
            subject.RegisterPrefix(DomNamespace.Create("https://example.com/a"), "a");
            Assert.Equal("https://example.com/a", doc.DocumentElement.Attribute("xmlns:a"));
        }

    }
}
