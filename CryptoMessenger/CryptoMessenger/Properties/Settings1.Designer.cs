﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CryptoMessenger.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF252525")]
        public global::System.Windows.Media.SolidColorBrush UIPrimaryBrush {
            get {
                return ((global::System.Windows.Media.SolidColorBrush)(this["UIPrimaryBrush"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF656565")]
        public global::System.Windows.Media.SolidColorBrush UISecondaryBrush {
            get {
                return ((global::System.Windows.Media.SolidColorBrush)(this["UISecondaryBrush"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF00A388")]
        public global::System.Windows.Media.SolidColorBrush SpecialBrush {
            get {
                return ((global::System.Windows.Media.SolidColorBrush)(this["SpecialBrush"]));
            }
            set {
                this["SpecialBrush"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF757575")]
        public global::System.Windows.Media.SolidColorBrush UISecondaryLightBrush {
            get {
                return ((global::System.Windows.Media.SolidColorBrush)(this["UISecondaryLightBrush"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FFFF0000")]
        public global::System.Windows.Media.SolidColorBrush AlertBrush {
            get {
                return ((global::System.Windows.Media.SolidColorBrush)(this["AlertBrush"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FFFFFFFF")]
        public global::System.Windows.Media.SolidColorBrush TextPrimaryBrush {
            get {
                return ((global::System.Windows.Media.SolidColorBrush)(this["TextPrimaryBrush"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FFDCDCDC")]
        public global::System.Windows.Media.SolidColorBrush TextSecondaryBrush {
            get {
                return ((global::System.Windows.Media.SolidColorBrush)(this["TextSecondaryBrush"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF00A388")]
        public global::System.Windows.Media.Color SpecialColor {
            get {
                return ((global::System.Windows.Media.Color)(this["SpecialColor"]));
            }
            set {
                this["SpecialColor"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FFFF0000")]
        public global::System.Windows.Media.Color AlertColor {
            get {
                return ((global::System.Windows.Media.Color)(this["AlertColor"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF303030")]
        public global::System.Windows.Media.SolidColorBrush UIPrimaryLightBrush {
            get {
                return ((global::System.Windows.Media.SolidColorBrush)(this["UIPrimaryLightBrush"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public int ActionDelayMsec {
            get {
                return ((int)(this["ActionDelayMsec"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2500")]
        public int HideNotificationDelayMsec {
            get {
                return ((int)(this["HideNotificationDelayMsec"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5000")]
        public int WaitServerConnectionDelayMsec {
            get {
                return ((int)(this["WaitServerConnectionDelayMsec"]));
            }
        }
    }
}
