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
using System.Collections.ObjectModel;

namespace Carbonfrost.Commons.Web.Dom {

    public class DomErrorCollection : Collection<DomError> {

        private readonly int _maxErrors;

        public DomErrorCollection(int maxErrors) {
            _maxErrors = maxErrors;
        }

        public DomErrorCollection() {
            _maxErrors = -1;
        }

        public DomError AddNew(
            string message,
            Exception exception = null,
            string errorCode = null,
            int? position = null,
            string fileName = null,
            int? lineNumber = null,
            int? linePosition = null
        ) {
            var result = new DomError {
                Message = message,
                Exception = exception,
                ErrorCode = errorCode,
                Position = position.GetValueOrDefault(-1),
                FileName = fileName,
                LineNumber = lineNumber.GetValueOrDefault(-1),
                LinePosition = linePosition.GetValueOrDefault(-1),
            };
            Add(result);
            return result;
        }

        protected override void InsertItem(int index, DomError item) {
            if (CanAddError()) {
                base.InsertItem(index, item);
            }
        }

        private bool CanAddError() {
            if (_maxErrors == -1) {
                return true;
            }

            return Count < _maxErrors;
        }
    }

}
