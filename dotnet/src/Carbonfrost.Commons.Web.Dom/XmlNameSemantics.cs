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

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public struct XmlNameSemantics {

        public string Prefix {
            get;
        }

        public string LocalName {
            get;
        }

        public static XmlNameSemantics FromName(string name) {
            string[] parts = name.Split(new [] { ':' }, 2);
            if (parts.Length == 2) {
                return new XmlNameSemantics(parts[0], parts[1]);
            }
            return new XmlNameSemantics(null, parts[0]);
        }

        public XmlNameSemantics(string prefix, string localName) {
            if (string.IsNullOrWhiteSpace(localName)) {
                throw Failure.AllWhitespace(nameof(localName));
            }

            Prefix = string.IsNullOrEmpty(prefix) ? null : prefix;
            LocalName = localName;
        }

        public override string ToString() {
            if (Prefix == null) {
                return LocalName;
            }
            return $"{Prefix}:{LocalName}";
        }

    }
}
