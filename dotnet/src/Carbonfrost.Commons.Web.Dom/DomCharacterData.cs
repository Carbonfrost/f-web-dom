//
// Copyright 2013, 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using System.Text.RegularExpressions;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomCharacterData : DomNode, IDomCharacterData<DomCharacterData> {

        public string Data {
            get {
                return (string) content;
            }
            set {
                content = value;
            }
        }

        public override string TextContent {
            get {
                return Data;
            }
            set {
                Data = value;
            }
        }

        public override DomNameContext NameContext {
            get {
                return ParentElement.NameContext;
            }
            set {
                throw new NotSupportedException();
            }
        }

        public bool IsWhitespace {
            get {
                return Utility.IsAllWhitespace(Data);
            }
        }

        protected internal DomCharacterData() {}

        public override string NodeValue {
            get {
                return Data;
            }
            set {
                Data = value;
            }
        }

        public DomCharacterData AppendData(string data) {
            Data += data;
            return this;
        }

        internal override void TryCompressWS() {
            if (Data == null) {
                return;
            }
            MatchEvaluator me = m => m.Value.IndexOf("\r\n") >= 0 ? "\r\n"
                : m.Value.IndexOf("\r") >= 0 ? "\r"
                : m.Value.IndexOf("\n") >= 0 ? "\n"
                : " ";
            Data = Regex.Replace(Data, @"\s+", me);
        }

        public DomCharacterData SplitText(Regex pattern) {
            if (pattern == null) {
                throw new ArgumentNullException(nameof(pattern));
            }
            return SplitTextCore(pattern.Split(Data));
        }

        public DomCharacterData SplitText(int index, int length) {
            return SplitTextCore(Data.Substring(0, index),
                Data.Substring(index, length),
                Data.Substring(index + length));
        }

        public DomCharacterData SplitText(int index) {
            After(OwnerDocument.CreateText(Data.Substring(index)));
            Data = Data.Substring(0, index);
            return this;
        }

        private DomCharacterData SplitTextCore(params string[] items) {
            var document = OwnerDocument;
            var myItems = items.Where(t => t.Length > 0);
            After(myItems.Skip(1).Select(n => document.CreateText(n)));
            Data = myItems.FirstOrDefault() ?? string.Empty;
            return this;
        }

        public DomCharacterData SetData(string value) {
            Data = value;
            return this;
        }
    }
}
