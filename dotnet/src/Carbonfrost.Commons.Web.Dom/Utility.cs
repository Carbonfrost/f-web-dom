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
using Carbonfrost.Commons.Core;

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
            if (str != null) {
                return str;
            }

            switch (value) {
                case double _:
                    return XmlConvert.ToString((double) value);
                case float _:
                    return XmlConvert.ToString((float) value);
                case decimal _:
                    return XmlConvert.ToString((decimal) value);
                case bool _:
                    return XmlConvert.ToString((bool) value);
                case DateTime _:
                    return XmlConvert.ToString((DateTime) value, XmlDateTimeSerializationMode.Utc);
                case DateTimeOffset _:
                    return XmlConvert.ToString((DateTimeOffset) value);
                case TimeSpan _:
                    return XmlConvert.ToString((TimeSpan) value);
                case DomObject _:
                    throw DomFailure.CannotUseAddWithDomObjects("value");
                default:
                    return Convert.ToString(value);
            }
        }

        internal static void CopyToArray<T>(IReadOnlyCollection<T> items, T[] array, int arrayIndex) {
            if (array == null) {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0 || arrayIndex >= array.Length) {
                throw Failure.IndexOutOfRange(nameof(arrayIndex), arrayIndex);
            }
            if (arrayIndex + items.Count > array.Length) {
                throw Failure.NotEnoughSpaceInArray(nameof(array), array);
            }
            foreach (var e in items) {
                array[arrayIndex] = e;
                arrayIndex++;
            }
        }

        internal static string LocalPath(Uri uri) {
            if (uri.IsAbsoluteUri) {
                return uri.LocalPath;
            }
            return uri.ToString();
        }

        public static T OptimalComposite<T>(IEnumerable<T> items, Func<T[], T> compositeFactory, T nullInstance)
            where T : class
        {
            if (items == null) {
                return nullInstance;
            }

            items = items.Where(t => t != null && !object.ReferenceEquals(t, nullInstance));
            if (!items.Any()) {
                return nullInstance;
            }
            if (items.Skip(1).Any()) { // 2 or more
                return compositeFactory(items.ToArray());
            }

            return items.First();
        }
    }
}
