﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StockDrops.Maui.Resources.Strings.Discord {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class DiscordResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal DiscordResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("StockDrops.Maui.Resources.Strings.Discord.DiscordResources", typeof(DiscordResources).Assembly);
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
        ///   Looks up a localized string similar to Oopss something went wrong!.
        /// </summary>
        internal static string DiscordWebhookDefaultErrorMessage {
            get {
                return ResourceManager.GetString("DiscordWebhookDefaultErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Oopss... Received error status {0} - {1}.
        /// </summary>
        internal static string DiscordWebhookMessageSentError {
            get {
                return ResourceManager.GetString("DiscordWebhookMessageSentError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sent Succesfully.
        /// </summary>
        internal static string DiscordWebhookMessageSentSuccesfully {
            get {
                return ResourceManager.GetString("DiscordWebhookMessageSentSuccesfully", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You need the {0} subscription to use this..
        /// </summary>
        internal static string DiscordWebhookSubscriptionErrorMessage {
            get {
                return ResourceManager.GetString("DiscordWebhookSubscriptionErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error saving the webhook..
        /// </summary>
        internal static string WebhookSavedError {
            get {
                return ResourceManager.GetString("WebhookSavedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Succesfully updated.\nReady to test..
        /// </summary>
        internal static string WebhookSavedSuccesfully {
            get {
                return ResourceManager.GetString("WebhookSavedSuccesfully", resourceCulture);
            }
        }
    }
}
