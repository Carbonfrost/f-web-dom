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

namespace Carbonfrost.Commons.Web.Dom {

    partial class DomName {

        internal class DefaultImpl : DomName {

            private readonly string _localName;
            private readonly DomNamespace _ns;

            public override string LocalName {
                get {
                    return _localName;
                }
            }

            public override DomNamespace Namespace {
                get {
                    return _ns;
                }
            }

            internal DefaultImpl(DomNamespace ns, string localName) {
                _ns = ns;
                _localName = localName;
            }
        }

        internal class SimpleName : DomName {

            private string _name;

            public override string LocalName {
                get {
                    return _name;
                }
            }

            public override DomNamespace Namespace {
                get {
                    return DomNamespace.Default;;
                }
            }

            public SimpleName(string name) {
                _name = name;
            }

            internal override XmlNameSemantics? TryXmlSemantics() {
                var xmlSemantics = XmlNameSemantics.FromName(_name);
                if (xmlSemantics.Prefix != null) {
                    return xmlSemantics;
                }
                return null;
            }
        }

        internal class QName : DomName {

            private string _prefix;
            private DomName _actualName;

            public override string Prefix {
                get {
                    return _prefix;
                }
            }

            public string LocalPart {
                get {
                    return _actualName.LocalName;
                }
            }

            public override string LocalName {
                get {
                    return LocalPart;
                }
            }

            public override DomNamespace Namespace {
                get {
                    return _actualName.Namespace;
                }
            }

            public QName(string prefix, DomName qName) {
                _prefix = prefix;
                _actualName = qName;
            }

            internal override XmlNameSemantics? TryXmlSemantics() {
                return new XmlNameSemantics(_prefix, LocalPart);
            }

            public override DomName WithPrefix(string prefix) {
                return new QName(prefix, _actualName);
            }

            public override DomName WithLocalName(string name) {
                return new QName(_prefix, name);
            }

            internal override string PrefixFormat() {
                return $"{Prefix}:{LocalName}";
            }
        }
    }
}
