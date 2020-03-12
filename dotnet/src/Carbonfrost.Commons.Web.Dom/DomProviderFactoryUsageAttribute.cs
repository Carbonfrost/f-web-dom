//
// Copyright 2014, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class DomProviderFactoryUsageAttribute : ProviderAttribute {

        public string Extensions { get; set; }

        public DomProviderFactoryUsageAttribute() : base(typeof(DomProviderFactory)) {
        }

        protected override int MatchCriteriaCore(object criteria) {
            if (criteria == null) {
                return 0;
            }

            int result = 0;
            var pp = PropertyProvider.FromValue(criteria);
            result += EnumerateExtensions().Contains(pp.GetString("Extension")) ? 1 : 0;
            return result;
        }

        public IEnumerable<string> EnumerateExtensions() {
            var chars = new [] { '\t', '\r', '\n', ' ', ';', ',' };
            return (Extensions ?? string.Empty).Split(chars, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
