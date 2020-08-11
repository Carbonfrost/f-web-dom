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

namespace Carbonfrost.Commons.Web.Dom {
    interface IDomNodeQuery<T> where T : IDomNodeQuery<T> {
        string Attribute(string name);
        string Attribute(DomName name);
        TValue Attribute<TValue>(string name);
        TValue Attribute<TValue>(DomName name);
        T Attribute(string name, object value);
        T Attribute(DomName name, object value);
        T RemoveAttribute(string name);
        T RemoveAttribute(DomName name);

        bool HasAttribute(string name);
        bool HasAttribute(DomName name);
        bool HasClass(string name);
        DomElementQuery QuerySelectorAll(string selector);
        DomElement QuerySelector(string selector);
        DomElementQuery QuerySelectorAll(DomSelector selector);
        DomElement QuerySelector(DomSelector selector);
        DomObjectQuery Select(string selector);
        DomObjectQuery Select(DomSelector selector);
        T Closest(string selector);
        T Closest(DomSelector selector);
    }
}
