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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomAttribute {

        static readonly Dictionary<Type, MethodInfo> CONVERSION_TYPES;

        static DomAttribute() {
            CONVERSION_TYPES = typeof(DomAttribute).GetMethods().Where(m => m.Name == "op_Explicit")
                .ToDictionary(k => k.ReturnType, k => k);
        }

        [CLSCompliant(false)]
        public static explicit operator DateTime?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return DateTime.Parse(attribute.Value, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind);
        }

        [CLSCompliant(false)]
        public static explicit operator bool(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            return XmlConvert.ToBoolean(attribute.Value.ToLowerInvariant());
        }

        [CLSCompliant(false)]
        public static explicit operator Guid(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            return XmlConvert.ToGuid(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator bool?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return XmlConvert.ToBoolean(attribute.Value.ToLowerInvariant());
        }

        [CLSCompliant(false)]
        public static explicit operator int(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            return XmlConvert.ToInt32(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator Guid?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return XmlConvert.ToGuid(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator int?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return XmlConvert.ToInt32(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator TimeSpan?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return XmlConvert.ToTimeSpan(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator DateTime(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            return DateTime.Parse(attribute.Value, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind);
        }

        [CLSCompliant(false)]
        public static explicit operator DateTimeOffset?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return XmlConvert.ToDateTimeOffset(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator long?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return XmlConvert.ToInt64(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator ulong?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return XmlConvert.ToUInt64(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator DateTimeOffset(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            return XmlConvert.ToDateTimeOffset(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator float?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return XmlConvert.ToSingle(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator decimal(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            return XmlConvert.ToDecimal(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator double?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return XmlConvert.ToDouble(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator double(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            return XmlConvert.ToDouble(attribute.Value);
        }


        [CLSCompliant(false)]
        public static explicit operator decimal?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return XmlConvert.ToDecimal(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator long(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            return XmlConvert.ToInt64(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator uint?(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return XmlConvert.ToUInt32(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator float(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            return XmlConvert.ToSingle(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator string(DomAttribute attribute) {
            if (attribute == null) {
                return null;
            }

            return attribute.Value;
        }

        [CLSCompliant(false)]
        public static explicit operator TimeSpan(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            return XmlConvert.ToTimeSpan(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator uint(DomAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            return XmlConvert.ToUInt32(attribute.Value);
        }

        [CLSCompliant(false)]
        public static explicit operator ulong(DomAttribute attribute) {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            return XmlConvert.ToUInt64(attribute.Value);
        }

    }
}

