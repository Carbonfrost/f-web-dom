//
// Copyright 2013, 2019 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public class DomWriterSettings {

        public static readonly DomWriterSettings Empty;

        static DomWriterSettings() {
            Empty = ReadOnly(new DomWriterSettings());
        }

        public bool IsReadOnly {
            get;
            private set;
        }

        protected DomWriterSettings() {}

        public static DomWriterSettings ReadOnly(DomWriterSettings other) {
            var result = other.Clone();
            result.IsReadOnly = true;
            return result;
        }

        public DomWriterSettings Clone() {
            return CloneCore();
        }

        protected virtual DomWriterSettings CloneCore() {
            return (DomWriterSettings) MemberwiseClone();
        }

        protected void ThrowIfReadOnly() {
            if (this.IsReadOnly) {
                throw Failure.Sealed();
            }
        }
    }
}
