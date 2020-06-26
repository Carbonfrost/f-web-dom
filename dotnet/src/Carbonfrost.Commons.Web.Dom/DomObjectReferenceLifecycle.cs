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

    public abstract class DomObjectReferenceLifecycle : IDomObjectReferenceLifecycle {

        public static readonly IDomObjectReferenceLifecycle Null
            = new NullImpl();

        private WeakReference<DomObject> _attachedDomObject;

        protected DomObject AttachedDomObject {
            get {
                if (_attachedDomObject.TryGetTarget(out DomObject result)) {
                    return result;
                }
                return null;
            }
        }

        public virtual void Attaching(DomObject instance) {
            _attachedDomObject = new WeakReference<DomObject>(instance);
        }

        public virtual void Detaching() {
            _attachedDomObject = null;
        }

        public abstract object Clone();

        sealed class NullImpl : IDomObjectReferenceLifecycle {

            void IDomObjectReferenceLifecycle.Attaching(DomObject instance) {
            }

            void IDomObjectReferenceLifecycle.Detaching() {
            }

            object IDomObjectReferenceLifecycle.Clone() {
                return this;
            }
        }
    }
}
