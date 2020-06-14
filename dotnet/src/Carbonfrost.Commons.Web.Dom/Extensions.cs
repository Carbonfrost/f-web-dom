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

namespace Carbonfrost.Commons.Web.Dom {

    public static partial class Extensions {

        public static DomElementCollection Descendants(this DomContainer source, string name) {
            return source.Descendants(DomName.Create(name));
        }

        public static DomElementCollection Descendants(this DomContainer source, DomName name) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }

            return new DefaultDomElementCollection(source, s => FilterByName(s.Descendants, name));
        }

        public static DomElementCollection Elements(this DomContainer source, string name) {
            return source.Elements(DomName.Create(name));
        }

        public static DomElementCollection Elements(this DomContainer source, DomName name) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            return new DefaultDomElementCollection(source, s => FilterByName(s.Elements, name));
        }

        public static DomElementCollection FollowingSiblings(this DomElement source, string name) {
            return source.FollowingSiblings(DomName.Create(name));
        }

        public static DomElementCollection FollowingSiblings(this DomElement source, DomName name) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            return new DefaultDomElementCollection(source, s => FilterByName(((DomElement) s).FollowingSiblings, name));
        }

        static IEnumerable<DomElement> FilterByName(IEnumerable<DomElement> elements,
                                                    DomName name) {
            return elements.Where(t => t.Name == name);
        }
    }
}
