//
// - IDomStream.cs -
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
using System.IO;
using System.Linq;

using System.Xml;

namespace Carbonfrost.Commons.Web.Dom {

    interface IDomStream<TNode> {

        void CopyTo(DomNode parent);
        void CopyContentsTo(DomNode parent);

        void WriteTo(TextWriter writer);
        void WriteTo(XmlWriter writer);
        void WriteTo(DomWriter writer);

        void WriteContentsTo(TextWriter writer);
        void WriteContentsTo(XmlWriter writer);
        void WriteContentsTo(DomWriter writer);

        DomNodeReader CreateReader();
        DomNodeReader CreateReader(DomNodeReaderSettings settings);
    }
}
