﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace com.atgardner.OMFG.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("com.atgardner.OMFG.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap logo {
            get {
                object obj = ResourceManager.GetObject("logo", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;OruxTracker xmlns=&quot;http://oruxtracker.com/app/res/calibration&quot; versionCode=&quot;2.1&quot;&gt;
        ///			&lt;MapCalibration layers=&quot;false&quot; layerLevel=&quot;{1}&quot;&gt;
        ///				&lt;MapName&gt;&lt;![CDATA[{0} {1}]]&gt;&lt;/MapName&gt;
        ///				&lt;MapChunks xMax=&quot;{2}&quot; yMax=&quot;{3}&quot; datum=&quot;WGS84&quot; projection=&quot;Mercator&quot; img_height=&quot;512&quot; img_width=&quot;512&quot; file_name=&quot;{0} {1}&quot; /&gt;
        ///				&lt;MapDimensions height=&quot;{4}&quot; width=&quot;{5}&quot; /&gt;
        ///				&lt;MapBounds minLat=&quot;{6}&quot; maxLat=&quot;{7}&quot; minLon=&quot;{8}&quot; maxLon=&quot;{9}&quot; /&gt;
        ///				&lt;CalibrationPoints&gt;
        ///					&lt;CalibrationPoint corner=&quot;TL&quot; lon=&quot;{8:0.000000}&quot; lat [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string OruxLayerTemplate {
            get {
                return ResourceManager.GetString("OruxLayerTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;UTF-8&quot;?&gt;
        ///&lt;OruxTracker xmlns=&quot;http://oruxtracker.com/app/res/calibration&quot; versionCode=&quot;3.0&quot;&gt;
        ///	&lt;MapCalibration layers=&quot;true&quot; layerLevel=&quot;0&quot;&gt;
        ///		&lt;MapName&gt;&lt;![CDATA[{0}]]&gt;&lt;/MapName&gt;
        ///		{1}
        ///	&lt;/MapCalibration&gt;
        ///&lt;/OruxTracker&gt;.
        /// </summary>
        internal static string OruxMapTemplate {
            get {
                return ResourceManager.GetString("OruxMapTemplate", resourceCulture);
            }
        }
    }
}
