//
// - Extensions.cs -
//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Core;

namespace Carbonfrost.Commons.Web.Dom {

    public static class Extensions {

        public static IEnumerable<DomElement> Descendants(this DomContainer source, string name) {
            if (source == null)
                throw new ArgumentNullException("source");
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw Failure.EmptyString("name");

            return FilterByName(source.Descendants, name);
        }

        public static IEnumerable<DomElement> Descendants(this DomContainer source, string name, string xmlns) {
            if (source == null)
                throw new ArgumentNullException("source");
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw Failure.EmptyString("name");

            return FilterByName(source.Descendants, name, xmlns);
        }

        public static IEnumerable<DomElement> Elements(this DomContainer source, string name) {
            if (source == null)
                throw new ArgumentNullException("source");
            return FilterByName(source.Elements, name, null);
        }

        public static IEnumerable<DomElement> Elements(this DomContainer source, string name, string xmlns) {
            if (source == null)
                throw new ArgumentNullException("source");
            return FilterByName(source.Elements, name, xmlns);
        }

        public static IEnumerable<DomElement> FollowingSiblings(this DomElement source, string name) {
            if (source == null) {
                throw new ArgumentNullException("source");
            }
            return FilterByName(source.FollowingSiblings, name, null);
        }

        public static IEnumerable<DomElement> FollowingSiblings(this DomElement source, string name, string xmlns) {
            if (source == null) {
                throw new ArgumentNullException("source");
            }
            return FilterByName(source.FollowingSiblings, name, xmlns);
        }

        static IEnumerable<DomElement> FilterByName(IEnumerable<DomElement> elements,
                                                    string name,
                                                    string xmlns = null) {
            return elements.Where(t => t.Name == name && t.NamespaceUri == xmlns);
        }
    }
}
