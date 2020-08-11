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

using System.IO;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    public partial class DomWriterTests {

        [Fact]
        public void Create_should_create_writer_for_XML_by_default() {
            Assert.IsInstanceOf<XmlDomWriter>(DomWriter.Create(TextWriter.Null, null));
        }

        [Fact]
        public void Create_should_create_writer_for_default_settings_type() {
            Assert.IsInstanceOf<DefaultDomWriter>(DomWriter.Create(TextWriter.Null, DomWriterSettings.Empty));
        }
    }
}
