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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public class DomStringTokenList : IList<string>, ISet<string>, IDomValue {

        private readonly IList<string> _items = new List<string>();
        private HashSet<string> _setCache;

        public static readonly DomStringTokenList Empty = ReadOnly(new DomStringTokenList());

        public string Value {
            get {
                return ToString();
            }
            set {
                _items.Clear();
                AddRange(ParseItems(value));
            }
        }

        public DomStringTokenList() {}

        public DomStringTokenList(params string[] items) {
            AddRange(items);
        }

        public DomStringTokenList(IEnumerable<string> items) {
            AddRange(items);
        }

        private DomStringTokenList(IList<string> items, bool dummy) {
            _items = items;
        }

        public static DomStringTokenList Parse(string text) {
            return Utility.Parse<DomStringTokenList>(text, _TryParse);
        }

        public static bool TryParse(string text, out DomStringTokenList result) {
            return _TryParse(text, out result) == null;
        }

        static Exception _TryParse(string text, out DomStringTokenList result) {
            result = new DomStringTokenList();

            if (text == null) {
                return null;
            }

            text = text.Trim();
            if (text.Length == 0) {
                return null;
            }

            result = new DomStringTokenList(ParseItems(text));
            return null;
        }

        static string[] ParseItems(string text) {
            if (text == null) {
                return Array.Empty<string>();
            }
            return Regex.Split(text, "\\s+");
        }

        public static implicit operator DomStringTokenList(string text) {
            return Parse(text);
        }

        public string this[int index] {
            get {
                return _items[index];
            }
            set {
                if (_items[index] != value) {
                    if (_setCache != null) {
                        _setCache.Remove(_items[index]);
                        _setCache.Add(value);
                    }
                    _items[index] = value;
                }
            }
        }

        private bool AddCore(string item) {
            // Allow null or empty to be compat with W3C
            if (string.IsNullOrEmpty(item)) {
                return false;
            }

            if (!Contains(item)) {
                _items.Add(item);

                if (Count >= 16) {
                    RequireSetCache();
                }

                if (_setCache != null) {
                    _setCache.Add(item);
                }

                return true;
            }
            return false;
        }

        public bool Add(string item) {
            CheckItem(item, "item");
            return AddCore(item);
        }

        public bool AddRange(string item1) {
            CheckItem(item1, "item1");
            return AddCore(item1);
        }

        public bool AddRange(string item1, string item2) {
            CheckItem(item1, "item1");
            CheckItem(item2, "item2");
            return AddCore(item1) | AddCore(item2);
        }

        public bool AddRange(string item1, string item2, string item3) {
            CheckItem(item1, "item1");
            CheckItem(item2, "item2");
            CheckItem(item3, "item3");
            return AddCore(item1) | AddCore(item2) | AddCore(item3);
        }

        public bool AddRange(params string[] items) {
            return AddRange((IEnumerable<string>) items);
        }

        public void Clear() {
            _setCache = null;
            _items.Clear();
        }

        public bool Contains(string item) {
            if (string.IsNullOrEmpty(item)) {
                return false;
            }
            if (_setCache == null) {
                return _items.Contains(item);
            }
            return _setCache.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex) {
            _items.CopyTo(array, arrayIndex);
        }

        private int GetIndexOfToken(string item) {
            if (string.IsNullOrEmpty(item)) {
                return -1;
            }

            CheckItem(item, "item");

            return _items.IndexOf(item);
        }

        public IEnumerator<string> GetEnumerator() {
            return _items.GetEnumerator();
        }

        public int IndexOf(string item) {
            return GetIndexOfToken(item);
        }

        public void Insert(int index, string item) {
            if (string.IsNullOrEmpty(item)) {
                return;
            }

            _items.Insert(index, item);
        }

        public bool Remove(string item) {
            if (_setCache != null && !_setCache.Remove(item)) {
                return false;
            }
            int index = this.GetIndexOfToken(item);
            if (index >= 0) {
                _items.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool RemoveRange(string item1, string item2) {
            return Remove(item1) || Remove(item2);
        }

        public bool RemoveRange(string item1, string item2, string item3) {
            return Remove(item1) || Remove(item2) || Remove(item3);
        }

        public bool RemoveRange(IEnumerable<string> items) {
            bool result = false;
            foreach (var item in items) {
                result |= Remove(item);
            }
            return result;
        }

        public bool RemoveRange(params string[] items) {
            return RemoveRange((IEnumerable<string>) items);
        }

        public void RemoveAt(int index) {
            _items.RemoveAt(index);
        }

        void ICollection<string>.Add(string item) {
            Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Toggle(bool value, string item) {
            if (value) {
                Add(item);
            } else {
                Remove(item);
            }
        }

        public void Toggle(bool value, string item1, string item2) {
            Toggle(value, item1);
            Toggle(value, item2);
        }

        public void Toggle(bool value, string item1, string item2, string item3) {
            Toggle(value, item1);
            Toggle(value, item2);
            Toggle(value, item3);
        }

        public void Toggle(bool value, params string[] items) {
            foreach (var i in items) {
                Toggle(value, i);
            }
        }

        public void Toggle(string item) {
            if (GetIndexOfToken(item) < 0) {
                Add(item);

            } else {
                Remove(item);
            }
        }

        public override string ToString() {
            return string.Join(" ", _items);
        }

        public DomStringTokenList Clone() {
            return new DomStringTokenList(_items);
        }

        public int Count {
            get { return _items.Count; } }


        public bool IsReadOnly {
            get {
                return _items.IsReadOnly;
            }
        }

        public static DomStringTokenList ReadOnly(DomStringTokenList other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return new DomStringTokenList(
                other._items.ToArray(),
                false
            );
        }

        public bool AddRange(IEnumerable<string> items) {
            if (items == null) {
                return false;
            }
            items = items.Where(t => !string.IsNullOrEmpty(t));

            if (items.Any(t => Regex.IsMatch(t, @"\s"))) {
                throw DomFailure.NoItemCanContainWhitespace("items");
            }
            if (IsReadOnly) {
                throw Failure.ReadOnlyCollection();
            }

            bool result = false;
            foreach (var e in items) {
                result |= Add(e);
            }
            return result;
        }

        void IDomValue.Initialize(DomAttribute attribute) {}

        public void UnionWith(IEnumerable<string> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            AddRange(other);
        }

        public void IntersectWith(IEnumerable<string> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            var cache = RequireSetCache();
            _setCache = null;

            RemoveRange(cache.Except(other));
        }

        public void ExceptWith(IEnumerable<string> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (var o in other) {
                Remove(o);
            }
        }

        public void SymmetricExceptWith(IEnumerable<string> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            var cache = RequireSetCache();
            _setCache = null;

            RemoveRange(cache.Intersect(other));
            AddRange(other.Except(cache));
        }

        public bool IsSubsetOf(IEnumerable<string> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return RequireSetCache().IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<string> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return RequireSetCache().IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<string> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return RequireSetCache().IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<string> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return RequireSetCache().IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<string> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return RequireSetCache().Overlaps(other);
        }

        public bool SetEquals(IEnumerable<string> other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            return RequireSetCache().SetEquals(other);
        }

        static void CheckItem(string item, string argName) {
            if (!string.IsNullOrEmpty(item) && Regex.IsMatch(item, @"\s")) {
                throw DomFailure.CannotContainWhitespace(argName);
            }
        }

        private HashSet<string> RequireSetCache() {
            if (_setCache == null) {
                _setCache = new HashSet<string>(_items);
            }
            return _setCache;
        }

        internal static bool StaticContains(string text, DomStringTokenList names) {
            if (string.IsNullOrEmpty(text)) {
                return false;
            }
            if (text.Length < names.ToString().Length) {
                return false;
            }
            return DomStringTokenList.Parse(text).IsSupersetOf(names);
        }

        void IDomValue.AppendValue(object value) {
            if (value is null) {
                return;
            }
            if (value is DomStringTokenList list) {
                AddRange(list);
            } else {
                Add(value.ToString());
            }
        }

        IDomValue IDomValue.Clone() {
            return Clone();
        }
    }

}
