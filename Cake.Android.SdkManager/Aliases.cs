using System;
using System.Linq;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.AndroidSdkManager
{
    /// <summary>
    /// Android SDK related aliases.
    /// </summary>
    [CakeAliasCategory("Android SDK Manager")]
    public static class AndroidSdkManagerAliases
    {
        /// <summary>
        /// Installs all available updates in the Android SDK.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="settings">The settings.</param>
        [CakeMethodAlias]
        public static void AndroidSdkManagerUpdateAll (this ICakeContext context, AndroidSdkManagerToolSettings settings = null)
        {
            var runner = new AndroidSdkManagerTool(context, context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
			runner.UpdateAll(settings ?? new AndroidSdkManagerToolSettings());
        }

        /// <summary>
        /// Installs the specified Android SDK packages.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="packages">The packages to install.</param>
        /// <param name="settings">The settings.</param>
        [CakeMethodAlias]
        public static void AndroidSdkManagerInstall (this ICakeContext context, IEnumerable<string> packages, AndroidSdkManagerToolSettings settings = null)
        {
            var runner = new AndroidSdkManagerTool(context, context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
			runner.InstallOrUninstall(true, packages, settings ?? new AndroidSdkManagerToolSettings());
        }

		/// <summary>
		/// Uninstalls the specified Android SDK packages.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="packages">The packages to install.</param>
		/// <param name="settings">The settings.</param>
		[CakeMethodAlias]
		public static void AndroidSdkManagerUninstall(this ICakeContext context, IEnumerable<string> packages, AndroidSdkManagerToolSettings settings = null)
		{
			var runner = new AndroidSdkManagerTool(context, context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
			runner.InstallOrUninstall(false, packages, settings ?? new AndroidSdkManagerToolSettings());
		}

		/// <summary>
		/// Gets a list of the Installed, Available and Updates for the Android SDK Manager.
		/// </summary>
		/// <returns>Installed, Available, and Available Updates for the Android SDK.</returns>
		/// <param name="context">The context.</param>
		/// <param name="settings">The settings.</param>
		[CakeMethodAlias]
		public static AndroidSdkManagerList AndroidSdkManagerList (this ICakeContext context, AndroidSdkManagerToolSettings settings = null)
        {
            var runner = new AndroidSdkManagerTool (context, context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);
            return runner.List (settings ?? new AndroidSdkManagerToolSettings ());
        }
    }
}
