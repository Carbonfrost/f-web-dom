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
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    internal class DomPathParser {

        private readonly Token[] _tokens;
        private int _index = -1;
        private DomPath _result;

        public DomPathParser(string text) {
            _tokens = Tokens(text).ToArray();
        }

        private Token Current {
            get {
                return _tokens[_index];
            }
        }

        private Token LA {
            get {
                return _tokens[_index + 1];
            }
        }

        internal bool Parse(out DomPath result) {
            _result = DomPath.Root;

            while (MoveNext() && ParseAxis()) {
            }

            result = _result;
            if (Current.Type == TokenType.EOF) {
                return true;
            }

            return Current.Type == TokenType.EOF;
        }

        private bool ParseAxis() {
            switch (Current.Type) {
                case TokenType.Slash:
                    return MoveNext() && ParseChild();

                case TokenType.AtSign:
                    return MoveNext() && ParseAttribute();

                case TokenType.DoubleSlash:
                    return MoveNext() && ParseDescendant();
            }
            return false;
        }

        // attr = '@' name
        private bool ParseAttribute() {
            if (Require(TokenType.String)) {
                _result = _result.Attribute(Current.Value);
                return true;
            }
            return false;
        }

        private bool ParseChild() {
            if (Current.Type != TokenType.String) {
                return false;
            }
            var name = Current.Value;
            int index = -1;
            if (name == "*") {
                return false;
            }

            if (LA.Type == TokenType.LBracket) {
                var _ = MoveNext()
                    && MoveNext()
                    && ParseIndex(out index)
                    && Require(TokenType.RBracket);
            }

            _result = _result.Element(name, index);
            return true;
        }

        private bool ParseIndex(out int index) {
            return Int32.TryParse(Current.Value, out index) && MoveNext();
        }

        private bool ParseDescendant() {
            if (Current.Type != TokenType.Asterisk) {
                return false;
            }

            DomPath path = _result;
            return MoveNext() && ParseHasAttribute();
        }

        private bool ParseName(out DomName name) {
            name = DomName.Create(Current.Value);
            if (Current.Type == TokenType.String) {
                return MoveNext();
            }
            return false;
        }

        private bool ParseHasAttribute() {
            DomName name = null;
            var success = Require(TokenType.LBracket)
                && MoveNext()
                && Require(TokenType.AtSign)
                && MoveNext()
                && ParseName(out name);

            if (success && Current.Type == TokenType.EqualSign) {
                MoveNext();
                _result = _result.DescendantHasAttributeValue(name, Current.Value);
                return MoveNext();
            }

            _result = _result.DescendantHasAttribute(name);
            return success;
        }

        private bool Require(TokenType type) {
            return Current.Type == type;
        }

        private bool MoveNext() {
            _index++;
            return _index < _tokens.Length;
        }

        internal static IEnumerable<Token> Tokens(string text) {
            var e = new CharEnumerator(text);
            while (e.MoveNext()) {
                if (e.C == '"' || e.C == '\'') {
                    yield return ScanQuotedString(e);

                } else if (char.IsWhiteSpace(e.C)) {
                    continue;

                } else {
                    switch (e.C) {
                        case '/' when e.LA == '/':
                            e.MoveNext();
                            yield return Token.DoubleSlash; break;
                        case '/':
                            yield return Token.Slash; break;
                        case '@':
                            yield return Token.AtSign; break;
                        case '[':
                            yield return Token.LBracket; break;
                        case ']':
                            yield return Token.RBracket; break;
                        case '*':
                            yield return Token.Asterisk; break;
                        case '=':
                            yield return Token.EqualSign; break;
                        default:
                            yield return ScanIdent(e); break;
                    }
                }
            }
            yield return Token.EOF;
        }

        private static Token ScanQuotedString(CharEnumerator e) {
            char quote = e.C;
            e.Mark(1);

            // TODO Escape sequences
            while (e.MoveNext() && e.C != quote) {
            }
            return new Token(e.Sub());
        }

        private static Token ScanIdent(CharEnumerator e) {
            e.Mark(0);
            while (e.MoveNext() && !char.IsWhiteSpace(e.C) && !"*/=@[]'\"".Contains(e.C)) {
            }
            var str = e.Sub();
            e.MovePrevious();
            return new Token(str);
        }

        class CharEnumerator {
            private int _index;
            private int _mark;
            private readonly string _string;

            public CharEnumerator(string s) {
                _index = -1;
                _string = s;
                _mark = -1;
            }

            public char C {
                get {
                    return _string[_index];
                }
            }

            public char LA {
                get {
                    if (_index + 1 < _string.Length) {
                        return _string[1 + _index];
                    }
                    return '\0';
                }
            }

            public void Mark(int offset) {
                _mark = _index + offset;
            }

            public string Sub() {
                return _string.Substring(_mark, _index - _mark);
            }

            public bool MoveNext() {
                _index++;
                return _index < _string.Length;
            }

            public void MovePrevious() {
                _index--;
            }
        }

        internal struct Token {
            public readonly string Value;
            internal readonly TokenType Type;
            public static readonly Token Slash = new Token(TokenType.Slash);
            public static readonly Token DoubleSlash = new Token(TokenType.DoubleSlash);
            public static readonly Token AtSign = new Token(TokenType.AtSign);
            public static readonly Token LBracket = new Token(TokenType.LBracket);
            public static readonly Token RBracket = new Token(TokenType.RBracket);
            public static readonly Token Asterisk = new Token(TokenType.Asterisk);
            public static readonly Token EqualSign = new Token(TokenType.EqualSign);
            public static readonly Token Error = new Token(TokenType.Error);
            public static readonly Token EOF = new Token(TokenType.EOF);

            private Token(TokenType type) {
                Type = type;
                Value = ValueOf(type);
            }

            internal Token(string v) {
                Value = v;
                Type = TokenType.String;
            }

            private static string ValueOf(TokenType type) {
                switch (type) {
                    case TokenType.Error:
                        return "<error>";
                    case TokenType.LBracket:
                        return "[";
                    case TokenType.RBracket:
                        return "]";
                    case TokenType.AtSign:
                        return "@";
                    case TokenType.DoubleSlash:
                        return "//";
                    case TokenType.Slash:
                        return "/";
                    case TokenType.Asterisk:
                        return "*";
                    case TokenType.EqualSign:
                        return "=";
                    case TokenType.EOF:
                        return "<eof>";
                }
                return "?";
            }

            internal static Token From(string value) {
                switch (value) {
                    case "":
                    case null:
                        return Error;
                    case "/":
                        return Slash;
                    case "//":
                        return DoubleSlash;
                    case "@":
                        return AtSign;
                    case "[":
                        return LBracket;
                    case "]":
                        return RBracket;
                    case "*":
                        return Asterisk;
                    case "=":
                        return EqualSign;
                }
                return new Token(value);
            }

            public override string ToString() {
                return Value;
            }
        }

        internal enum TokenType {
            Error,
            LBracket,
            RBracket,
            AtSign,
            DoubleSlash,
            Slash,
            String,
            EOF,
            Asterisk,
            EqualSign
        }
    }
}
