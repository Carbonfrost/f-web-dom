//
// Copyright 2020 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Reflection;

namespace Carbonfrost.Commons.Web.Dom {

    public class DomNodeVisitDispatcher : IDomNodeVisitDispatcher {

        private static readonly IDictionary<Type, DispatchCacheImpl> _allDispatchCaches = new Dictionary<Type, DispatchCacheImpl>();
        private readonly IDomNodeVisitor _visitor;

        private DispatchCacheImpl DispatchCache {
            get {
                return _allDispatchCaches.GetValueOrCache(
                    _visitor.GetType(),
                    t => new DispatchCacheImpl(t)
                );
            }
        }

        public DomNodeVisitDispatcher(IDomNodeVisitor visitor) {
            if (visitor == null) {
                throw new ArgumentNullException(nameof(visitor));
            }

            _visitor = visitor;
        }

        public static IDomNodeVisitDispatcher Create(IDomNodeVisitor visitor) {
            if (visitor is IDomNodeVisitDispatcher d) {
                return d;
            }
            var result = new DomNodeVisitDispatcher(visitor);
            if (result.DispatchCache.IsEmpty) {
                return new FastDispatcher(visitor);
            }
            return result;
        }

        public virtual void Dispatch(DomObject obj) {
            if (obj == null) {
                return;
            }
            SlowDispatch(obj);
        }

        private static IDictionary<Type, MethodInfo> NewDispatchCache(IDomNodeVisitor v) {
            var dispatch = new Dictionary<Type, MethodInfo>();
            foreach (var face in v.GetType().GetInterfaces()) {
                if (!typeof(IDomNodeVisitor).IsAssignableFrom(face)) {
                    continue;
                }
                if (typeof(IDomNodeVisitor) == face) {
                    continue;
                }
                var mapping = v.GetType().GetInterfaceMap(face);
                for (int i = 0; i < mapping.InterfaceMethods.Length; i++) {
                    if (mapping.InterfaceMethods[i].Name == "Visit") {
                        var elementType = mapping.InterfaceMethods[i].GetParameters()[0].ParameterType;
                        dispatch[elementType] = mapping.TargetMethods[i];
                    }
                }
            }
            return dispatch;
        }

        private static bool CanFastDispatch(Type type) {
            return type.Assembly == DomProviderFactory.THIS_ASSEMBLY;
        }

        private void SlowDispatch(DomObject obj) {
            var type = obj.GetType();

            while (!CanFastDispatch(type)) {
                // FIXME Makes more sense to invert this
                if (DispatchCache.TryGetValue(type, out MethodInfo method)) {
                    method.InvokeWithUnwrap(_visitor, new[] { obj });
                    return;
                }
                type = type.BaseType;
            }

            FastDispatch(_visitor, obj);
        }

        private static void FastDispatch(IDomNodeVisitor v, DomObject obj) {
            switch (obj) {
                case DomElement _:
                    v.Visit((DomElement) obj);
                    return;

                case DomAttribute _:
                    v.Visit((DomAttribute) obj);
                    return;

                case DomText _:
                    v.Visit((DomText) obj);
                    return;

                case DomCDataSection _:
                    v.Visit((DomCDataSection) obj);
                    return;

                case DomEntityReference _:
                    v.Visit((DomEntityReference) obj);
                    return;

                case DomEntity _:
                    v.Visit((DomEntity) obj);
                    return;

                case DomProcessingInstruction _:
                    v.Visit((DomProcessingInstruction) obj);
                    return;

                case DomComment _:
                    v.Visit((DomComment) obj);
                    return;

                case DomDocument _:
                    v.Visit((DomDocument) obj);
                    return;

                case DomDocumentType _:
                    v.Visit((DomDocumentType) obj);
                    return;

                case DomDocumentFragment _:
                    v.Visit((DomDocumentFragment) obj);
                    return;

                case DomNotation _:
                    v.Visit((DomNotation) obj);
                    return;
            }
        }

        class DispatchCacheImpl {
            private readonly Dictionary<Type, MethodInfo> _dispatch;

            public DispatchCacheImpl(Type type) {
                _dispatch = new Dictionary<Type, MethodInfo>();
                foreach (var face in type.GetInterfaces()) {
                    if (!typeof(IDomNodeVisitor).IsAssignableFrom(face)) {
                        continue;
                    }
                    if (typeof(IDomNodeVisitor) == face) {
                        continue;
                    }
                    var mapping = type.GetInterfaceMap(face);
                    for (int i = 0; i < mapping.InterfaceMethods.Length; i++) {
                        if (mapping.InterfaceMethods[i].Name == "Visit") {
                            var elementType = mapping.InterfaceMethods[i].GetParameters()[0].ParameterType;
                            _dispatch[elementType] = mapping.TargetMethods[i];
                        }
                    }
                }
            }

            public bool IsEmpty {
                get {
                    return _dispatch.Count == 0;
                }
            }

            public bool TryGetValue(Type type, out MethodInfo method) {
                return _dispatch.TryGetValue(type, out method);
            }
        }

        class FastDispatcher : IDomNodeVisitDispatcher {
            private readonly IDomNodeVisitor _visitor;

            public FastDispatcher(IDomNodeVisitor visitor) {
                _visitor = visitor;
            }

            public void Dispatch(DomObject obj) {
                FastDispatch(_visitor, obj);
            }
        }

    }
}
