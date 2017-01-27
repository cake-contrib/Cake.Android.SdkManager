using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.AndroidSdkManager
{
	/// <summary>
	/// Android SDK Manager tool settings.
	/// </summary>
	public class AndroidSdkManagerToolSettings : ToolSettings
	{
		/// <summary>
		/// Gets or sets the Android SDK root path.
		/// </summary>
		/// <value>The sdk root.</value>
		public DirectoryPath SdkRoot { get; set; }
		/// <summary>
		/// Gets or sets the release channel.
		/// </summary>
		/// <value>The channel.</value>
		public AndroidSdkChannel Channel { get; set; } = AndroidSdkChannel.Stable;
		/// <summary>
		/// Gets or sets a value indicating whether or not to include obsoleted packages.
		/// </summary>
		/// <value><c>true</c> if include obsoleted packages; otherwise, <c>false</c>.</value>
		public bool IncludeObsolete { get; set; } = false;
		/// <summary>
		/// Gets or sets a value indicating whether HTTPS should be used.
		/// </summary>
		/// <value><c>true</c> if no HTTPS; otherwise, <c>false</c>.</value>
		public bool NoHttps { get; set; } = false;
		/// <summary>
		/// Gets or sets the type of the proxy to be used.
		/// </summary>
		/// <value>The type of the proxy.</value>
		public AndroidSdkManagerProxyType ProxyType { get; set; } = AndroidSdkManagerProxyType.None;
		/// <summary>
		/// Gets or sets the proxy host.
		/// </summary>
		/// <value>The proxy host.</value>
		public string ProxyHost { get; set; }
		/// <summary>
		/// Gets or sets the proxy port.
		/// </summary>
		/// <value>The proxy port.</value>
		public int ProxyPort { get; set; } = -1;
	}

	/// <summary>
	/// Android SDK Manager proxy type.
	/// </summary>
	public enum AndroidSdkManagerProxyType
	{
		/// <summary>
		/// Do not use a proxy.
		/// </summary>
		None,
		/// <summary>
		/// Use a HTTP proxy.
		/// </summary>
		Http,
		/// <summary>
		/// Use a SOCKS proxy.
		/// </summary>
		Socks
	}

	/// <summary>
	/// Android SDK Manager release channel.
	/// </summary>
	public enum AndroidSdkChannel
	{
		/// <summary>
		/// Stable packages.
		/// </summary>
		Stable = 0,
		/// <summary>
		/// Beta packages.
		/// </summary>
		Beta = 1,
		/// <summary>
		/// Developer packages.
		/// </summary>
		Dev = 2,
		/// <summary>
		/// Canary packages.
		/// </summary>
		Canary = 3,
	}
}