//
// Copyright 2013, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Carbonfrost.Commons.Web.Dom {

    static class Mixin {

        public static void AddRange<T>(this ICollection<T> self, IEnumerable<T> items) {
            foreach (var e in items) {
                self.Add(e);
            }
        }

        public static object InvokeWithUnwrap(this MethodInfo method, object instance, object[] args) {
            try {
                return method.Invoke(instance, args);
            } catch (TargetInvocationException ex) {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        public static TValue GetValueOrCache<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TKey, TValue> func) {
            TValue value;
            if (source.TryGetValue(key, out value))
                return value;

            return (source[key] = func(key));
        }

        public static TValue FirstNonNull<T, TValue>(this IEnumerable<T> items, Func<T, TValue> func)
            where TValue : class
        {
            foreach (var m in items) {
                TValue result = func(m);
                if (result != null)
                    return result;
            }

            return null;
        }

        public static IEnumerable<T> NonNull<T>(this IEnumerable<T> items) {
            foreach (var m in items) {
                if (m != null) {
                    yield return m;
                }
            }
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key) {
            return GetValueOrDefault(source, key, default(TValue));
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue) {
            TValue value;
            if (source.TryGetValue(key, out value)) {
                return value;
            }

            return defaultValue;
        }
    }
}
