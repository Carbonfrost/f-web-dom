//
// Copyright 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.IO;
using System.Text;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Spec;
using Carbonfrost.Commons.Web.Dom;

namespace Carbonfrost.UnitTests.Web.Dom {

    [DomProviderFactoryUsage(Extensions = ".custom")]
    public class CustomDomProviderFactory : DomProviderFactory {

        public override IDomNodeTypeProvider NodeTypeProvider {
            get {
                return new RDomNodeTypeProvider();
            }
        }

        class RDomNodeTypeProvider : DomNodeTypeProvider {
            public override DomName GetAttributeName(Type attributeType) {
                return DomName.Create("hey:" + attributeType.Name);
            }

            public override DomName GetElementName(Type elementType) {
                return DomName.Create("hey:" + elementType.Name);
            }
        }

        protected override DomWriter CreateDomWriter(TextWriter textWriter, DomWriterSettings settings) {
            return new CustomWriter();
        }

        protected override DomReader CreateDomReader(TextReader textReader, DomReaderSettings settings) {
            return new CustomReader();
        }

        public override bool IsProviderObject(Type providerObjectType) {
            // Notice that `DerivedCustomAttribute' isn't here, but its base class is
            // which will allow default name to be generated
            return providerObjectType == typeof(CustomAttribute);
        }

    }

    class CustomWriter : DomWriter {

        public override DomWriteState WriteState {
            get {
                return DomWriteState.Content;
            }
        }

        public override void WriteStartElement(DomName name) {}
        public override void WriteStartAttribute(DomName name) {}
        public override void WriteEndAttribute() {}
        public override void WriteValue(string value) {}
        public override void WriteEndDocument() {}
        public override void WriteDocumentType(string name, string publicId, string systemId) {}
        public override void WriteEntityReference(string name) {}
        public override void WriteProcessingInstruction(string target, string data) {}
        public override void WriteNotation() {}
        public override void WriteComment(string data) {}
        public override void WriteCDataSection(string data) {}
        public override void WriteText(string data) {}
        public override void WriteStartDocument() {}
        public override void WriteEndElement() {}
    }

    class CustomReader : DomReader {
        public override DomNodeType NodeType { get; }
        public override string Name { get; }
        public override string NamespaceUri { get; }
        public override string Value { get; }
        public override int AttributeCount { get; }
        public override string BaseUri { get; }
        public override int Depth { get; }
        public override bool EOF { get; }
        public override bool HasValue { get; }
        public override bool IsEmptyElement { get; }
        public override string LocalName { get; }
        public override string Prefix { get; }
        public override DomReadState ReadState { get; }

        public override string GetAttribute(DomName name) {
            throw new NotImplementedException();
        }

        public override string GetAttribute(string name) {
            throw new NotImplementedException();
        }

        public override string GetAttribute(int index) {
            throw new NotImplementedException();
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
            Assert.Equal("hey:CustomAttribute", attr.Name.LocalName);
        }

        [Fact]
        public void Constructor_for_derived_default_attribute_should_invoke_provider() {
            var attr = new DerivedCustomAttribute();
            Assert.Equal("hey:DerivedCustomAttribute", attr.Name.LocalName);
        }

        [Fact]
        public void Constructor_for_unknown_should_cause_error() {
            var ex = Record.Exception(() => new UnknownCustomAttribute());
            Assert.NotNull(ex);
        }

        [Fact]
        public void DomWriter_Create_should_create_writer_for_custom_file_name() {
            Assert.IsInstanceOf<CustomWriter>(
                DomWriter.Create(new FSStreamContext(new Uri("hello.custom", UriKind.Relative)), DomWriterSettings.Empty)
            );
        }

        [Fact]
        public void DomReader_Create_should_create_reader_for_custom_file_name() {
            Assert.IsInstanceOf<CustomReader>(
                DomReader.Create(new FSStreamContext(new Uri("hello.custom", UriKind.Relative)), DomReaderSettings.Empty)
            );
        }

        // TODO This is a workaround for f-core not supporting AppendText correctly

        class FSStreamContext : StreamContext {
            private readonly Uri _uri;

            public FSStreamContext(Uri uri) {
                _uri = uri;
            }

            public override Uri Uri {
                get {
                    return _uri;
                }
            }

            public override StreamContext ChangePath(string relativePath) {
                throw new NotImplementedException();
            }

            public override Stream Open() {
                return Stream.Null;
            }

            public override StreamWriter AppendText(Encoding encoding) {
                return StreamWriter.Null;
            }
        }
    }
}
