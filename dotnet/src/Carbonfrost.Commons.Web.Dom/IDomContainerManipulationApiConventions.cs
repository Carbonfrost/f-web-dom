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
using System.Collections.Generic;

namespace Carbonfrost.Commons.Web.Dom {

    interface IDomContainerManipulationApiConventions<TResult> {
        TResult Add(object content);
        TResult AddAnnotation(object annotation);
        TResult AddAnnotations(IEnumerable<object> annotations);
        TResult AddClass(string className);
        TResult AddRange(object content1);
        TResult AddRange(object content1, object content2);
        TResult AddRange(object content1, object content2, object content3);
        TResult AddRange(params object [] content);
        TResult Append(DomNode child);
        TResult Attribute(string name, object value);
        TResult Clone();
        TResult CompressWhitespace();
        TResult Empty();
        TResult Remove();
        TResult RemoveAnnotation(object value);
        TResult RemoveAnnotations(Type type);
        TResult RemoveAnnotations<T>() where T : class;
        TResult RemoveAttribute(string name);
        TResult RemoveAttributes();
        TResult RemoveClass(string className);
        TResult RemoveChildNodes();
        TResult RemoveSelf();
        TResult SetName(string name);
        TResult Wrap(DomNode newParent);
        TResult Wrap(string element);
    }
}
