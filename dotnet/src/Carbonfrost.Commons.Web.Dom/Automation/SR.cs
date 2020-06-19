
// This file was automatically generated.  DO NOT EDIT or else
// your changes could be lost!

#pragma warning disable 1570

using System;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace Carbonfrost.Commons.Web.Dom.Resources {

    /// <summary>
    /// Contains strongly-typed string resources.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("srgen", "1.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    internal static partial class SR {

        private static global::System.Resources.ResourceManager _resources;
        private static global::System.Globalization.CultureInfo _currentCulture;
        private static global::System.Func<string, string> _resourceFinder;

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(_resources, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Carbonfrost.Commons.Web.Dom.Automation.SR", typeof(SR).GetTypeInfo().Assembly);
                    _resources = temp;
                }
                return _resources;
            }
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return _currentCulture;
            }
            set {
                _currentCulture = value;
            }
        }

        private static global::System.Func<string, string> ResourceFinder {
            get {
                if (object.ReferenceEquals(_resourceFinder, null)) {
                    try {
                        global::System.Resources.ResourceManager rm = ResourceManager;
                        _resourceFinder = delegate (string s) {
                            return rm.GetString(s);
                        };
                    } catch (global::System.Exception ex) {
                        _resourceFinder = delegate (string s) {
                            return string.Format("localization error! {0}: {1} ({2})", s, ex.GetType(), ex.Message);
                        };
                    }
                }
                return _resourceFinder;
            }
        }


  /// <summary>An attribute with the specified name `${value}' already exists.</summary>
    internal static string AttributeWithGivenNameExists(
    object @value
    ) {
        return string.Format(Culture, ResourceFinder("AttributeWithGivenNameExists") , @value);
    }

  /// <summary>Cannot append a child node to this node.</summary>
    internal static string CannotAppendChildNode(
    
    ) {
        return string.Format(Culture, ResourceFinder("CannotAppendChildNode") );
    }

  /// <summary>Cannot append a node with the given type, `${nodeType}'.</summary>
    internal static string CannotAppendChildNodeWithType(
    object @nodeType
    ) {
        return string.Format(Culture, ResourceFinder("CannotAppendChildNodeWithType") , @nodeType);
    }

  /// <summary>Cannot append text with non-whitespace characters to the document outside of the document element.</summary>
    internal static string CannotAppendNonWSText(
    
    ) {
        return string.Format(Culture, ResourceFinder("CannotAppendNonWSText") );
    }

  /// <summary>Token cannot itself contain whitespace characters.</summary>
    internal static string CannotContainWhitespace(
    
    ) {
        return string.Format(Culture, ResourceFinder("CannotContainWhitespace") );
    }

  /// <summary>Cannot generate the default name for type `${type}'.  Specify the constructor with the name set explicitly or check that the provider factory supports generating the name.</summary>
    internal static string CannotGenerateName(
    object @type
    ) {
        return string.Format(Culture, ResourceFinder("CannotGenerateName") , @type);
    }

  /// <summary>Cannot append element to document that already has a root element.</summary>
    internal static string CannotHaveMultipleRoots(
    
    ) {
        return string.Format(Culture, ResourceFinder("CannotHaveMultipleRoots") );
    }

  /// <summary>Cannot replace the document node.</summary>
    internal static string CannotReplaceDocument(
    
    ) {
        return string.Format(Culture, ResourceFinder("CannotReplaceDocument") );
    }

  /// <summary>Cannot set name on the current node</summary>
    internal static string CannotSetName(
    
    ) {
        return string.Format(Culture, ResourceFinder("CannotSetName") );
    }

  /// <summary>Cannot unwrap node because it would create a document with multiple roots.</summary>
    internal static string CannotUnwrapWouldCreateMalformedDocument(
    
    ) {
        return string.Format(Culture, ResourceFinder("CannotUnwrapWouldCreateMalformedDocument") );
    }

  /// <summary>DOM objects can be arguments using this method.  Use the Append or ReplaceWith methods instead.</summary>
    internal static string CannotUseAddWithDomObjects(
    
    ) {
        return string.Format(Culture, ResourceFinder("CannotUseAddWithDomObjects") );
    }

  /// <summary>Attributes can only be replaced with other attributes.</summary>
    internal static string CanReplaceOnlyWithAttribute(
    
    ) {
        return string.Format(Culture, ResourceFinder("CanReplaceOnlyWithAttribute") );
    }

  /// <summary>Operation is valid due to the current state of the writer, `${state}`</summary>
    internal static string InvalidWriteState(
    object @state
    ) {
        return string.Format(Culture, ResourceFinder("InvalidWriteState") , @state);
    }

  /// <summary>No item in the collection can contain whitespace</summary>
    internal static string NoItemCanContainWhitespace(
    
    ) {
        return string.Format(Culture, ResourceFinder("NoItemCanContainWhitespace") );
    }

  /// <summary>Not a valid local name</summary>
    internal static string NotValidLocalName(

    ) {
        return string.Format(Culture, ResourceFinder("NotValidLocalName") );
    }

  /// <summary>Name `${what}' cannot be used in this context</summary>
    internal static string NotValidNameForThisContext(
    object @what
    ) {
        return string.Format(Culture, ResourceFinder("NotValidNameForThisContext") , @what);
    }

  /// <summary>Node must have a parent for this operation.</summary>
    internal static string ParentNodeRequired(
    
    ) {
        return string.Format(Culture, ResourceFinder("ParentNodeRequired") );
    }

  /// <summary>Document has no root element to set inner text.</summary>
    internal static string RequiresDocumentElementToSetInnerText(
    
    ) {
        return string.Format(Culture, ResourceFinder("RequiresDocumentElementToSetInnerText") );
    }

  /// <summary>Processing instruction target can be the empty string or contain whitespace characters.</summary>
    internal static string TargetCannotBeAllWhitepace(
    
    ) {
        return string.Format(Culture, ResourceFinder("TargetCannotBeAllWhitepace") );
    }

    }
}
