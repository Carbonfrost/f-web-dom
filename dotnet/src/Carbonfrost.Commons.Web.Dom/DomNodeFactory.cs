//
// Copyright 2013, 2016, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Reflection;
using Carbonfrost.Commons.Core.Runtime;

namespace Carbonfrost.Commons.Web.Dom {

    [Providers]
    public class DomNodeFactory : IDomNodeFactory, IDomNodeFactoryApiConventions {

        private readonly IDictionary<Type, NodeConstructorInfo> _cache = new Dictionary<Type, NodeConstructorInfo>();

        [DomNodeFactoryUsage]
        public static readonly IDomNodeFactory Null = new NullDomNodeFactory();

        [DomNodeFactoryUsage]
        public static readonly IDomNodeFactory Default = new DomNodeFactory();

        public IDomNodeTypeProvider NodeTypeProvider {
            get;
            private set;
        }

        public static IDomNodeFactory FromName(string name) {
            return App.GetProvider<IDomNodeFactory>(name);
        }

        [DomNodeFactoryUsage]
        public static IDomNodeFactory Compose(params IDomNodeFactory[] items) {
            return Compose((IEnumerable<IDomNodeFactory>) items);
        }

        public static IDomNodeFactory Compose(IEnumerable<IDomNodeFactory> items) {
            return Utility.OptimalComposite(
                items, m => new CompositeDomNodeFactory(m), Null
            );
        }

        public DomNodeFactory() : this(null) {}

        public DomNodeFactory(IDomNodeTypeProvider nodeTypeProvider) {
            NodeTypeProvider = nodeTypeProvider ?? DomNodeTypeProvider.Default;
        }

        public Type GetAttributeNodeType(string name) {
            return NodeTypeProvider.GetAttributeNodeType(name);
        }

        public Type GetElementNodeType(string name) {
            return NodeTypeProvider.GetElementNodeType(name);
        }

        public Type GetProcessingInstructionNodeType(string target) {
            return NodeTypeProvider.GetProcessingInstructionNodeType(target);
        }

        public virtual DomComment CreateComment() {
            return new DomComment();
        }

        public virtual DomCDataSection CreateCDataSection() {
            return new DomCDataSection();
        }

        public virtual DomText CreateText() {
            return new DomText();
        }

        public virtual DomDocumentFragment CreateDocumentFragment() {
            return new DomDocumentFragment();
        }

        public virtual DomProcessingInstruction CreateProcessingInstruction(string target) {
            return CreateNode(
                GetProcessingInstructionNodeType(target),
                target,
                nameOrTarget => new DomProcessingInstruction(nameOrTarget)
            );
        }

        public virtual DomAttribute CreateAttribute(string name) {
            return CreateNode(
                GetAttributeNodeType(name),
                name,
                nameOrTarget => new DomAttribute(nameOrTarget)
            );
        }

        public virtual DomElement CreateElement(string name) {
            return CreateNode(
                GetElementNodeType(name),
                name,
                nameOrTarget => new DomElement(nameOrTarget)
            );
        }

        public virtual DomEntityReference CreateEntityReference(string name) {
            return new DomEntityReference(name);
        }

        public virtual DomDocumentType CreateDocumentType(string name) {
            return new DomDocumentType(name);
        }

        public virtual DomEntity CreateEntity(string name) {
            return new DomEntity();
        }

        public virtual DomNotation CreateNotation(string name) {
            return new DomNotation();
        }

        // Conventions
        public DomComment CreateComment(string data) {
            var result = CreateComment();
            result.Data = data;
            return result;
        }

        public DomCDataSection CreateCDataSection(string data) {
            var result = CreateCDataSection();
            result.Data = data;
            return result;
        }

        public DomText CreateText(string data) {
            var result = CreateText();
            result.Data = data;
            return result;
        }

        public DomProcessingInstruction CreateProcessingInstruction(string target, string data) {
            var result = CreateProcessingInstruction(target);
            result.Data = data;
            return result;
        }

        public DomAttribute CreateAttribute(string name, string value) {
            var attr = CreateAttribute(name);
            attr.Value = value;
            return attr;
        }

        public DomAttribute CreateAttribute(string name, IDomValue value) {
            var result = CreateAttribute(name);

            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            result.DomValue = value;
            return result;
        }

        public DomDocumentType CreateDocumentType(string name, string publicId, string systemId) {
            var result = CreateDocumentType(name);

            // TODO Look up based on name
            result.PublicId = publicId;
            result.SystemId = systemId;
            return result;
        }

        private T CreateNode<T>(Type type, string nameOrTarget, Func<string, T> ctor) {
            if (type == typeof(T) || type == null) {
                return ctor(nameOrTarget);
            }
            var info = _cache.GetValueOrCache(type, t => new NodeConstructorInfo(t));
            return (T) info.Invoke(nameOrTarget);
        }

        struct NodeConstructorInfo {
            public readonly ConstructorInfo Ctor;
            public readonly ConstructorInfo CtorString;

            public NodeConstructorInfo(Type type) {
                CtorString = type.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new [] { typeof(string) },
                    null
                );
                Ctor = type.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null
                );
            }

            internal object Invoke(string nameOrTarget) {
                // TODO Improve error handling here - it should be a domain exception if neither
                // .ctor(string) OR .ctor() exist
                if (CtorString != null) {
                    return CtorString.Invoke(new [] { nameOrTarget });
                }

                return Ctor.Invoke(Array.Empty<object>());
            }
        }
    }
}
