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
using System.Linq;

namespace Carbonfrost.Commons.Web.Dom {

    class DomWriterStateMachine {
        private DomWriteState _state;

        public DomWriteState WriteState {
            get;
            private set;
        }

        public void StartElement() {
            ImplicitlyStartDocument();
            Transition(DomWriteState.Element);
        }

        public void StartAttribute() {
            ValidState(DomWriteState.Element, DomWriteState.Attribute);
            Transition(DomWriteState.Attribute);
        }

        public void EndAttribute() {
            ValidState(DomWriteState.Attribute);
            Transition(DomWriteState.Element);
        }

        public void Value() {
            ValidState(DomWriteState.Element, DomWriteState.Content, DomWriteState.Attribute);
            Transition(DomWriteState.Element, DomWriteState.Content);
        }

        public void EndDocument() {
            ImplicitlyStartDocument();
            Transition(DomWriteState.Closed);
        }

        public void DocumentType() {
            ImplicitlyStartDocument();
        }

        public void EntityReference() {
            CharData();
        }

        public void ProcessingInstruction() {
            CharData();
        }

        public void Notation() {
        }

        public void Comment() {
            CharData();
        }

        public void CDataSection() {
            CharData();
        }

        public void Text() {
            CharData();

        }

        public void StartDocument() {
            ValidState(DomWriteState.Start);
            ImplicitlyStartDocument();
        }

        public void EndElement() {
            ValidState(DomWriteState.Element, DomWriteState.Attribute, DomWriteState.Content);
            Transition(DomWriteState.Content);
        }

        private void ValidState(params DomWriteState[] opts) {
            if (!opts.Contains(_state)) {
                throw InvalidStateError();
            }
        }

        private void ImplicitlyStartDocument() {
            Transition(DomWriteState.Start, DomWriteState.Prolog);
        }

        private void CharData() {
            ImplicitlyStartDocument();
            ValidState(DomWriteState.Prolog, DomWriteState.Element, DomWriteState.Content);
            Transition(DomWriteState.Content);
        }

        private void Transition(DomWriteState from, DomWriteState to) {
            if (_state == from) {
                Transition(to);
            }
        }

        internal void Transition(DomWriteState to) {
            _state = to;
        }

        private void InvalidState(DomWriteState s) {
            if (_state == s) {
                throw InvalidStateError();
            }
        }

        private Exception InvalidStateError() {
            // Cache the state so that it is correct for the error message
            var s = _state;
            Transition(DomWriteState.Error);
            return DomFailure.InvalidWriteState(s);
        }
    }
}
