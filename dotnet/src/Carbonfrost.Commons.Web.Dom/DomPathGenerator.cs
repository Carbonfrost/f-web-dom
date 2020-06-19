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

using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    public class DomPathGenerator {

        public static readonly DomPathGenerator Minimal = new MinimalImpl();
        public static readonly DomPathGenerator Default = new DefaultImpl();
        public static readonly DomPathGenerator Unambiguous = Default;

        public virtual bool KeepElementIndex(DomElement element) {
            return true;
        }

        public virtual bool KeepElementId(DomElement element) {
            return false;
        }

        class DefaultImpl : DomPathGenerator {}

        class MinimalImpl : DomPathGenerator {

            public override bool KeepElementIndex(DomElement element) {
                if (element.Parent == null) {
                    return false;
                }

                var sibs = element.Parent.Children;
                if (sibs.Count == 1 || sibs.Where(e => e.Name == element.Name).Count() == 1) {
                    return false;
                }

                return true;
            }

            public override bool KeepElementId(DomElement element) {
                return true;
            }
        }
    }
}
