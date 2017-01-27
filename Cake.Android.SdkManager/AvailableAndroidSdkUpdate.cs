
namespace Cake.AndroidSdkManager
{
	/// <summary>
	/// Available Android Sdk Update package information.
	/// </summary>
	public class AvailableAndroidSdkUpdate
	{
		/// <summary>
		/// Gets or sets the Android SDK Manager path.
		/// </summary>
		/// <value>The path.</value>
		public string Path { get; set; }
		/// <summary>
		/// Gets or sets the installed version of the package.
		/// </summary>
		/// <value>The installed version.</value>
		public string InstalledVersion { get; set; }
		/// <summary>
		/// Gets or sets the available version of the package.
		/// </summary>
		/// <value>The available version.</value>
		public string AvailableVersion { get; set; }
	}
}
