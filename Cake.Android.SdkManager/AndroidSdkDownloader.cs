using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.AndroidSdkManager
{
	public static class AndroidSdkDownloader
	{
		const string REPOSITORY_URL_BASE = "https://dl.google.com/android/repository/";
		const string REPOSITORY_URL = REPOSITORY_URL_BASE + "repository2-1.xml";
		const string REPOSITORY_SDK_PATTERN = REPOSITORY_URL_BASE + "tools_r{0}.{1}.{2}-{3}.zip";


		/// <summary>
		/// Downloads the Android SDK
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="destinationDirectory">Destination directory, or ./tools/androidsdk if none is specified.</param>
		/// <param name="specificVersion">Specific version, or latest if none is specified.</param>
		public static void DownloadSdk (ICakeContext context, DirectoryPath destinationDirectory = null, Version specificVersion = null)
		{
			var http = new HttpClient();
			http.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
			http.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
			http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
			http.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");


			if (specificVersion == null) {
				try
				{
					var data = http.GetStringAsync(REPOSITORY_URL).Result;

					var xdoc = new System.Xml.XmlDocument();
					xdoc.LoadXml(data);

					var revNode = xdoc.SelectSingleNode("//remotePackage[@path='tools']/revision");

					var strVer = revNode.SelectSingleNode("major")?.InnerText + "." + revNode.SelectSingleNode("minor").InnerText + "." + revNode.SelectSingleNode("micro").InnerText;

					specificVersion = Version.Parse(strVer);
				} catch {
					specificVersion = new Version(25, 2, 5);
				}
			}


			var platformStr = "windows";
			switch (context.Environment.Platform.Family) {
				case PlatformFamily.OSX:
					platformStr = "macosx";
					break;
				case PlatformFamily.Linux:
					platformStr = "linux";
					break;
			}

			var sdkUrl = string.Format(REPOSITORY_SDK_PATTERN, specificVersion.Major, specificVersion.Minor, specificVersion.Build, platformStr);

			var toolsDir = new DirectoryPath("./tools");
			if (!context.FileSystem.Exist(toolsDir))
				Directory.CreateDirectory(toolsDir.MakeAbsolute(context.Environment).FullPath);

			var sdkDir = new DirectoryPath("./tools/androidsdk");
			if (!context.FileSystem.Exist(sdkDir))
				Directory.CreateDirectory(sdkDir.MakeAbsolute(context.Environment).FullPath);

			var sdkZipFile = new FilePath("./tools/androidsdk.zip");

			using (var httpStream = http.GetStreamAsync(sdkUrl).Result)
			using (var fileStream = File.Create(sdkZipFile.MakeAbsolute(context.Environment).FullPath))
			{
				httpStream.CopyTo(fileStream);
			}

			ZipFile.ExtractToDirectory(sdkZipFile.MakeAbsolute(context.Environment).FullPath,
															 sdkDir.MakeAbsolute(context.Environment).FullPath);
		}
	}
}
