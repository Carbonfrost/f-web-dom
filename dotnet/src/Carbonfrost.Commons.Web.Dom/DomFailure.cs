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
using Carbonfrost.Commons.Core;
using Carbonfrost.Commons.Web.Dom.Resources;

namespace Carbonfrost.Commons.Web.Dom {

    static class DomFailure {

        public static ArgumentException CannotContainWhitespace(string argumentName) {
            return Failure.Prepare(new ArgumentException(SR.CannotContainWhitespace(), argumentName));
        }

        public static InvalidOperationException CannotHaveMultipleRoots() {
            return Failure.Prepare(new InvalidOperationException(SR.CannotHaveMultipleRoots()));
        }

        public static InvalidOperationException CannotAppendNonWSText() {
            return Failure.Prepare(new InvalidOperationException(SR.CannotAppendNonWSText()));
        }

        public static InvalidOperationException CannotUnwrapWouldCreateMalformedDocument() {
            return Failure.Prepare(new InvalidOperationException(SR.CannotUnwrapWouldCreateMalformedDocument()));
        }

        public static ArgumentException NoItemCanContainWhitespace(string argumentName) {
            return Failure.Prepare(new ArgumentException(SR.NoItemCanContainWhitespace(), argumentName));
        }

        public static InvalidOperationException ParentNodeRequired() {
            return Failure.Prepare(new InvalidOperationException(SR.ParentNodeRequired()));
        }

        public static ArgumentException TargetCannotBeAllWhitepace(string argumentName) {
            return Failure.Prepare(new ArgumentException(SR.TargetCannotBeAllWhitepace(), argumentName));
        }

        public static InvalidOperationException CannotAppendChildNode() {
            return Failure.Prepare(new InvalidOperationException(SR.CannotAppendChildNode()));
        }

        public static ArgumentException AttributeWithGivenNameExists(object name, string argumentName) {
            return Failure.Prepare(new ArgumentException(SR.AttributeWithGivenNameExists(name), argumentName));
        }

        public static NotSupportedException CannotReplaceDocument() {
            return Failure.Prepare(new NotSupportedException(SR.CannotReplaceDocument()));
        }

        public static NotSupportedException CannotSetName() {
            return Failure.Prepare(new NotSupportedException(SR.CannotSetName()));
        }

        public static ArgumentException CannotGenerateName(Type type) {
            return Failure.Prepare(new ArgumentException(SR.CannotGenerateName(type)));
        }

        // TODO Consider FormatExceptions here - should have descriptions

        public static FormatException MatchesSelectorCannotBeEmpty() {
            return Failure.Prepare(new FormatException());
        }

        public static FormatException HasSelectorCannotBeEmpty() {
            return Failure.Prepare(new FormatException());
        }

        public static FormatException NotSelectorSelectionCannotBeEmpty() {
            return Failure.Prepare(new FormatException());
        }

        public static FormatException ContainsSelectorCannotBeEmpty() {
            return Failure.Prepare(new FormatException());
        }

        public static Exception InvalidWriteState(DomWriteState s) {
            return Failure.Prepare(new InvalidOperationException(SR.InvalidWriteState(s)));
        }

        public static FormatException UnknownCombinator(char combinator) {
            return Failure.Prepare(new FormatException());
        }

        public static FormatException CouldNotParseQuery(string query, string remainder) {
            return Failure.Prepare(new FormatException());
        }

        public static FormatException CannotParseAttributeQuery(string query, string remainder) {
            return Failure.Prepare(new FormatException());
        }

        public static FormatException QueueDidNotMatch() {
            return Failure.Prepare(new FormatException());
        }

        public static FormatException QueueNotLongEnoughToConsumeSequence() {
            return Failure.Prepare(new FormatException());
        }

        public static ArgumentException CannotUseAddWithDomObjects(string argumentName) {
            return Failure.Prepare(new ArgumentException(argumentName));
        }

        public static InvalidOperationException CannotAppendChildNodeWithType(DomNodeType nodeType) {
            return Failure.Prepare(new InvalidOperationException(SR.CannotAppendChildNodeWithType(nodeType)));
        }

        public static InvalidOperationException RequiresDocumentElementToSetInnerText() {
            return Failure.Prepare(new InvalidOperationException(SR.RequiresDocumentElementToSetInnerText()));
        }

        public static ArgumentException NotValidLocalName(string argName) {
            return Failure.Prepare(new ArgumentException(SR.NotValidLocalName(), argName));
        }

        public static ArgumentException NotValidNameForThisContext(string argName, IDomNameApiConventions name) {
            return Failure.Prepare(new ArgumentException(SR.NotValidNameForThisContext(name.Name), argName));
        }
    }
}
