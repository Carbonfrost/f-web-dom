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

using System;
using System.Reflection;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    public partial class DomValue : IDomValue {

        internal static IDomValue Create() {
            return new Automatic();
        }

        internal class Automatic : DomValue {}

        public static IDomValue Create(Type valueType) {
            if (valueType == null) {
                throw new ArgumentNullException(nameof(valueType));
            }
            if (typeof(IDomValue).IsAssignableFrom(valueType)) {
                return Activation.CreateInstance<IDomValue>(valueType);
            }

            return (IDomValue) Activator.CreateInstance(
                typeof(DomValue<>).MakeGenericType(valueType)
            );
        }
    }
}

