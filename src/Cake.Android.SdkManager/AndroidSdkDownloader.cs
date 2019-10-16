using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using Cake.Core;
using Cake.Core.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Cake.AndroidSdkManager
{
	public static class AndroidSdkDownloader
	{
		const string REPOSITORY_URL_BASE = "https://dl.google.com/android/repository/";
		const string REPOSITORY_URL = REPOSITORY_URL_BASE + "repository2-1.xml";

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
			
			var data = http.GetStringAsync(REPOSITORY_URL).Result;

			var xdoc = XDocument.Parse(data);

			var packages = xdoc.XPathSelectElements("//remotePackage[@path='tools']")
				.Select(p =>
				{
					var revision = p.Element("revision");
					var version =
						new Version($"{revision.Element("major").Value}.{revision.Element("minor").Value}.{revision.Element("micro").Value}");
					var archives = p.Element("archives").Elements();
					var platforms = new Dictionary<string, string>
					{
						{ "linux", archives.FirstOrDefault(a => a.Element("host-os").Value == "linux").Element("complete").Element("url").Value },
						{ "windows", archives.FirstOrDefault(a => a.Element("host-os").Value == "windows").Element("complete").Element("url").Value },
						{ "macosx", archives.FirstOrDefault(a => a.Element("host-os").Value == "macosx").Element("complete").Element("url").Value },
					};
					return new { version, platforms };
				})
				.OrderByDescending(p => p.version)
				.ToArray();

			var platformStr = "windows";
			switch (context.Environment.Platform.Family) {
				case PlatformFamily.OSX:
					platformStr = "macosx";
					break;
				case PlatformFamily.Linux:
					platformStr = "linux";
					break;
			}

			var package = packages.FirstOrDefault(p => specificVersion == null || p.version == specificVersion);

			if (package == null)
			{
				throw new InvalidOperationException("Package cannot be found.");
			}

			if (!package.platforms.TryGetValue(platformStr, out var platformUrl))
			{
				throw new InvalidOperationException($"Specific platform is not supported by the package version {package.version}.");
			}

			var sdkUrl = $"{REPOSITORY_URL_BASE}{platformUrl}";

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
