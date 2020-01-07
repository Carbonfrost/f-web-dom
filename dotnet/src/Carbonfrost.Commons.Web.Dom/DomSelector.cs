//
// Copyright 2016 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Core.Runtime.Expressions;
using Carbonfrost.Commons.Web.Dom.Query;

namespace Carbonfrost.Commons.Web.Dom {

    public abstract class DomSelector {

        public abstract DomObjectQuery Select(DomNode node);
        public abstract bool Matches(DomNode node);

        public static DomSelector Parse(string text) {
            DomSelector result;
            var ex = _TryParse(text, out result);
            if (ex != null) {
                throw ex;
            }

            return result;
        }

        public static bool TryParse(string text, out DomSelector result) {
            return _TryParse(text, out result) == null;
        }

        private static Exception _TryParse(string text, out DomSelector result) {
            result = null;

            try {
                result = new CssSelector(text);
                return null;
            } catch (Exception ex) {
                return Failure.NotParsable("text", typeof(DomSelector), ex);
            }
        }
    }

}
