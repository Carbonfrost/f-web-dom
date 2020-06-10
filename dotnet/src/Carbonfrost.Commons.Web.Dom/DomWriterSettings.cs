//
// Copyright 2013, 2019, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public enum DomIndentCharacter {
        Space,
        Tab,
    }

    partial class Extensions {

        internal static char ToChar(this DomIndentCharacter c) {
            return c == DomIndentCharacter.Space ? ' ' : '\t';
        }
    }

    public enum DomEndOfLineCharacter {
        LF,
        CRLF,
    }

    public enum DomAlignAttributes {
        Unaligned,
        AlignLeft,
        AlignRight,
        BreakEach,
    }

    public enum DomQuoteAttributesCharacter {
        DoubleQuote,
        SingleQuote,
        None,
    }

    public class DomWriterSettings {
        // FIXME Implies DomXmlWriterSettings, DomXmlWriterSettings.core(DomWriterSettings)
        // FIXME DomWriterXmlSettings probably delegates to (similar to how reader is planned)
        // FIXME Probably consolidate OuterXmllWriter

        public static readonly DomWriterSettings Empty;
        private DomEndOfLineCharacter _endOfLineCharacters;
        private DomIndentCharacter _indentCharacter;
        private int _indentWidth;
        private bool _alignAttributes;
        private DomQuoteAttributesCharacter _attributeCharacter;

        static DomWriterSettings() {
            Empty = ReadOnly(new DomWriterSettings());
        }

        public bool IsReadOnly {
            get;
            private set;
        }

        public int IndentWidth {
            get {
                return _indentWidth;
            }
            set {
                WritePreamble();
                _indentWidth = value;
            }
        }

        public bool Indent {
            get;
            set;
        }

        public bool AlignAttributes { // FIXME AlignAttribute
            get {
                return _alignAttributes;
            }
            set {
                WritePreamble();
                _alignAttributes = value;
            }
        }

        public DomQuoteAttributesCharacter AttributeCharacter { // FIXME Name
            get {
                return _attributeCharacter;
            }
            set {
                WritePreamble();
                _attributeCharacter = value;
            }
        }

        public virtual bool PrettyPrint {
            set {
                WritePreamble();

                IndentCharacter = DomIndentCharacter.Space;
                IndentWidth = 2;
                Indent = true;
                AlignAttributes = true;
                AttributeCharacter = DomQuoteAttributesCharacter.DoubleQuote;
            }
        }

        public DomIndentCharacter IndentCharacter {
            get {
                return _indentCharacter;
            }
            set {
                WritePreamble();
                _indentCharacter = value;
            }
        }

        public DomEndOfLineCharacter EndOfLineCharacters {
            get {
                return _endOfLineCharacters;
            }
            set {
                WritePreamble();
                _endOfLineCharacters = value;
            }
        }

        public DomWriterSettings() {
        }

        public static DomWriterSettings ReadOnly(DomWriterSettings settings) {
            if (settings == null) {
                throw new ArgumentNullException(nameof(settings));
            }
            if (settings.IsReadOnly) {
                return settings;
            }

            var result = settings.CloneReadOnly();
            result.IsReadOnly = true;
            return result;
        }

        public DomWriterSettings Clone() {
            return CloneCore();
        }

        protected virtual DomWriterSettings CloneCore() {
            return (DomWriterSettings) MemberwiseClone();
        }

        protected virtual DomWriterSettings CloneReadOnly() {
            var result = CloneCore();
            result.IsReadOnly = true;
            return result;
        }

        protected void ThrowIfReadOnly() {
            if (IsReadOnly) {
                throw Failure.Sealed();
            }
        }

        private void WritePreamble() {
        }
    }
}
