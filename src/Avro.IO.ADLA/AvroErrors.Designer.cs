﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Avro.IO.ADLA {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AvroErrors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AvroErrors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Avro.IO.ADLA.AvroErrors", typeof(AvroErrors).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Base stream does not contain all the magic bytes!.
        /// </summary>
        internal static string BaseStreamMagicBytesWrong {
            get {
                return ResourceManager.GetString("BaseStreamMagicBytesWrong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot seek the base stream!.
        /// </summary>
        internal static string BaseStreamUnseekable {
            get {
                return ResourceManager.GetString("BaseStreamUnseekable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no last byte to read!.
        /// </summary>
        internal static string LastByteNotAvailable {
            get {
                return ResourceManager.GetString("LastByteNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can only read the entire magic stream!.
        /// </summary>
        internal static string MagicStreamMustReadAll {
            get {
                return ResourceManager.GetString("MagicStreamMustReadAll", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can only read the magic stream from the beginning!.
        /// </summary>
        internal static string MagicStreamReadOnlyBegin {
            get {
                return ResourceManager.GetString("MagicStreamReadOnlyBegin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can only seek the magic stream with no offset!.
        /// </summary>
        internal static string MagicStreamSeekOffsetNotAllowed {
            get {
                return ResourceManager.GetString("MagicStreamSeekOffsetNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can only seek the magic stream from the beginning!.
        /// </summary>
        internal static string MagicStreamSeekOnlyBegin {
            get {
                return ResourceManager.GetString("MagicStreamSeekOnlyBegin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can only decrement the position by one!.
        /// </summary>
        internal static string PositionCanOnlyDecrementByOne {
            get {
                return ResourceManager.GetString("PositionCanOnlyDecrementByOne", resourceCulture);
            }
        }
    }
}
