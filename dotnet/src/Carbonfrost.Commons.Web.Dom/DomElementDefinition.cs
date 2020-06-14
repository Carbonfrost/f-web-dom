//
// Copyright 2013, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

namespace Carbonfrost.Commons.Web.Dom {

    public class DomElementDefinition : DomContainerDefinition {

        private DomWhitespaceMode whitespaceMode;
        private Flags _flags;
        private Type _elementNodeType;

        public DomWhitespaceMode WhitespaceMode {
            get {
                return whitespaceMode;
            }
            set {
                ThrowIfReadOnly();
                whitespaceMode = value;
            }
        }

        public bool IsEmpty {
            get {
                return _flags.HasFlag(Flags.Empty);
            }
            set {
                ThrowIfReadOnly();
                SetFlags(Flags.Empty, value);
            }
        }

        public bool IsSelfClosing {
            get {
                return _flags.HasFlag(Flags.SelfClosing);
            }
            set {
                ThrowIfReadOnly();
                SetFlags(Flags.SelfClosing, value);
            }
        }

        public bool PreserveWhitespace {
            get {
                return WhitespaceMode == DomWhitespaceMode.Preserve;
            }
        }

        public Type ElementNodeType {
            get {
                return _elementNodeType;
            }
            set {
                ThrowIfReadOnly();
                _elementNodeType = value;
            }
        }

        protected internal DomElementDefinition(DomName name) : base(name) {
        }

        public new DomElementDefinition Clone() {
            return (DomElementDefinition) base.Clone();
        }

        protected override DomNodeDefinition CloneCore() {
            return new DomElementDefinition(Name) {
                _flags = _flags,
                WhitespaceMode = WhitespaceMode,
                ElementNodeType = ElementNodeType,
            };
        }

        private void SetFlags(Flags flag, bool value) {
            _flags = value ? (_flags | flag) : (_flags & ~flag);
        }

        [Flags]
        enum Flags {
            None,
            SelfClosing = 1 << 0,
            Empty = 1 << 1,
        }
    }
}
