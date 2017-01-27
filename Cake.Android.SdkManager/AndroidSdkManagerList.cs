using System.Collections.Generic;

namespace Cake.AndroidSdkManager
{
	/// <summary>
	/// Encapsulates results from Android SDK Manager's listing
	/// </summary>
	public class AndroidSdkManagerList
	{
		/// <summary>
		/// Gets or sets the available packages to install.
		/// </summary>
		/// <value>The available packages.</value>
		public List<AndroidSdkPackage> AvailablePackages { get; set; } = new List<AndroidSdkPackage>();
		/// <summary>
		/// Gets or sets the already installed packages.
		/// </summary>
		/// <value>The installed packages.</value>
		public List<InstalledAndroidSdkPackage> InstalledPackages { get; set; } = new List<InstalledAndroidSdkPackage>();
		/// <summary>
		/// Gets or sets the available package updates.
		/// </summary>
		/// <value>The available updates.</value>
		public List<AvailableAndroidSdkUpdate> AvailableUpdates { get; set; } = new List<AvailableAndroidSdkUpdate>();
	}
}
