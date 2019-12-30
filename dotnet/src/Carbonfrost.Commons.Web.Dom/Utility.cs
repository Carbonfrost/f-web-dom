//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Carbonfrost.Commons.Web.Dom {

    static class Utility {

        internal delegate Exception TryParser<T>(string text, out T result);

        public static T Parse<T>(string text, TryParser<T> _TryParse) {
            T result;
            Exception ex = _TryParse(text, out result);
            if (ex == null)
                return result;
            else
                throw ex;
        }

        public static IEnumerable<T> Cons<T>(T item, IEnumerable<T> other) {
            return Enumerable.Concat(new T[] { item }, other);
        }

        public static IEnumerable<Type> GetAncestorTypes(Type type) {
            while (type != null) {
                yield return type;
                type = type.GetTypeInfo().BaseType;
            }
        }

        public static string ConvertToString(object value) {
            var str = value as string;
            if (str != null)
                return str;

            if (value is double)
                return XmlConvert.ToString((double) value);

            if (value is float)
                return XmlConvert.ToString((float) value);

            if (value is decimal)
                return XmlConvert.ToString((decimal) value);

            if (value is bool)
                return XmlConvert.ToString((bool) value);

            if (value is DateTime)
                return XmlConvert.ToString((DateTime) value, XmlDateTimeSerializationMode.Utc);

            if (value is DateTimeOffset)
                return XmlConvert.ToString((DateTimeOffset) value);

            if (value is TimeSpan)
                return XmlConvert.ToString((TimeSpan) value);

            if (value is DomObject)
                throw DomFailure.CannotUseAddWithDomObjects("value");

            return Convert.ToString(value);
        }
    }
}
