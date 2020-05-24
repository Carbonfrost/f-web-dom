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
using System.Xml;
using System.Xml.Schema;

namespace Carbonfrost.Commons.Web.Dom {

    public class DomXmlReaderSettings : DomReaderSettings {

        private XmlReaderSettings _xmlReaderSettings;

        public XmlReaderSettings XmlReaderSettings {
            get {
                return _xmlReaderSettings;
            }
            set {
                ThrowIfReadOnly();
                _xmlReaderSettings = value;
            }
        }

        public ValidationType ValidationType {
            get {
                return XmlReaderSettings.ValidationType;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.ValidationType = value;
            }
        }

        public XmlSchemaValidationFlags ValidationFlags {
            get {
                return XmlReaderSettings.ValidationFlags;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.ValidationFlags = value;
            }
        }

        public XmlSchemaSet Schemas {
            get {
                return XmlReaderSettings.Schemas;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.Schemas = value;
            }
        }

        public XmlNameTable NameTable {
            get {
                return XmlReaderSettings.NameTable;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.NameTable = value;
            }
        }

        public long MaxCharactersInDocument {
            get {
                return XmlReaderSettings.MaxCharactersInDocument;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.MaxCharactersInDocument = value;
            }
        }

        public long MaxCharactersFromEntities {
            get {
                return XmlReaderSettings.MaxCharactersFromEntities;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.MaxCharactersFromEntities = value;
            }
        }

        public int LinePositionOffset {
            get {
                return XmlReaderSettings.LinePositionOffset;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.LinePositionOffset = value;
            }
        }

        public int LineNumberOffset {
            get {
                return XmlReaderSettings.LineNumberOffset;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.LineNumberOffset = value;
            }
        }

        public bool IgnoreWhitespace {
            get {
                return XmlReaderSettings.IgnoreWhitespace;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.IgnoreWhitespace = value;
            }
        }

        public bool IgnoreProcessingInstructions {
            get {
                return XmlReaderSettings.IgnoreProcessingInstructions;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.IgnoreProcessingInstructions = value;
            }
        }

        public bool IgnoreComments {
            get {
                return XmlReaderSettings.IgnoreComments;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.IgnoreComments = value;
            }
        }

        public DtdProcessing DtdProcessing {
            get {
                return XmlReaderSettings.DtdProcessing;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.DtdProcessing = value;
            }
        }

        public ConformanceLevel ConformanceLevel {
            get {
                return XmlReaderSettings.ConformanceLevel;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.ConformanceLevel = value;
            }
        }

        public bool CloseInput {
            get {
                return XmlReaderSettings.CloseInput;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.CloseInput = value;
            }
        }

        public bool CheckCharacters {
            get {
                return XmlReaderSettings.CheckCharacters;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.CheckCharacters = value;
            }
        }

        public bool Async {
            get {
                return XmlReaderSettings.Async;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.Async = value;
            }
        }

        public XmlResolver XmlResolver {
            get {
                // API for the reader settings has no getter
                return null;
            }
            set {
                ThrowIfReadOnly();
                XmlReaderSettings.XmlResolver = value;
            }
        }

        public event ValidationEventHandler ValidationEventHandler {
            add {
                ThrowIfReadOnly();
                XmlReaderSettings.ValidationEventHandler += value;
            }
            remove {
                ThrowIfReadOnly();
                XmlReaderSettings.ValidationEventHandler -= value;
            }
        }

        public static DomXmlReaderSettings ReadOnly(DomXmlReaderSettings settings) {
            if (settings == null) {
                throw new ArgumentNullException(nameof(settings));
            }
            if (settings.IsReadOnly) {
                return settings;
            }
            return (DomXmlReaderSettings) settings.CloneReadOnly();
        }

        public new DomXmlReaderSettings Clone() {
            return (DomXmlReaderSettings) CloneCore();
        }

        protected override DomReaderSettings CloneReadOnly() {
            var result = (DomXmlReaderSettings) base.CloneReadOnly();

            // TODO Should make XmlReaderSettings read-only
            return result;
        }

        protected override DomReaderSettings CloneCore() {
            var result = (DomXmlReaderSettings) MemberwiseClone();
            result.XmlReaderSettings = XmlReaderSettings.Clone();
            return result;
        }
    }
}
