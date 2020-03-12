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

namespace Carbonfrost.Commons.Web.Dom {

    public class DomError {

        public string Message {
            get;
            set;
        }

        public Exception Exception {
            get;
            set;
        }

        public string ErrorCode {
            get;
            set;
        }

        public int Position {
            get;
            set;
        }

        public string FileName {
            get;
            set;
        }

        public int LineNumber {
            get;
            set;
        }

        public int LinePosition {
            get;
            set;
        }

        public DomError() {
            LineNumber = -1;
            LinePosition = -1;
            Position = -1;
        }
    }
}
