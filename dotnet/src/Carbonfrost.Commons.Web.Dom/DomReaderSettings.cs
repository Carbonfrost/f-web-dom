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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public class DomReaderSettings {

        public static readonly DomReaderSettings Empty;
        private int _maxErrors = -1;

        public int MaxErrors {
            get {
                return _maxErrors;
            }
            set {
                ThrowIfReadOnly();
                _maxErrors = value;
            }
        }

        public bool IsReadOnly {
            get;
            private set;
        }

        static DomReaderSettings() {
            Empty = ReadOnly(new DomReaderSettings());
        }

        protected DomReaderSettings() {}

        public static DomReaderSettings ReadOnly(DomReaderSettings settings) {
            if (settings == null) {
                throw new ArgumentNullException(nameof(settings));
            }
            if (settings.IsReadOnly) {
                return settings;
            }
            var s = settings.CloneReadOnly();
            s.IsReadOnly = true;
            return s;
        }

        public DomReaderSettings Clone() {
            return CloneCore();
        }

        protected virtual DomReaderSettings CloneCore() {
            return (DomReaderSettings) MemberwiseClone();
        }

        protected virtual DomReaderSettings CloneReadOnly() {
            var result = CloneCore();
            result.IsReadOnly = true;
            return result;
        }

        protected void ThrowIfReadOnly() {
            if (IsReadOnly) {
                throw Failure.Sealed();
            }
        }
    }
}
