using System;
using Cake.Core.IO;
using Cake.AndroidSdkManager.Fakes;
using Cake.AndroidSdkManager;
using NUnit.Framework;
using Cake.Core;
using Cake;
using System.Linq;

namespace Cake.AndroidSdkManager.Tests
{
	public class Tests : TestFixtureBase
	{
		const string ANDROID_SDK_ROOT = "../../../android-sdk/";

		[Test] 
		public void A1_Download_SDK ()
		{
			Cake.AndroidSdkManagerDownload(null, null);

			Assert.IsTrue(Cake.FileSystem.Exist(new FilePath ("./tools/androidsdk/tools/bin/sdkmanager"))
			             || Cake.FileSystem.Exist(new FilePath("./tools/androidsdk/tools/bin/sdkmanager.bat")));
		}
		[Test]
		public void List()
		{
			var list = this.Cake.AndroidSdkManagerList(new AndroidSdkManagerToolSettings {
				SdkRoot = ANDROID_SDK_ROOT, SkipVersionCheck = false
			});

			Assert.NotNull(list);

			foreach (var a in list.AvailablePackages)
				Console.WriteLine($"{a.Description}\t{a.Version}\t{a.Path}");

			foreach (var a in list.InstalledPackages)
				Console.WriteLine($"{a.Description}\t{a.Version}\t{a.Path}");
		}

		[Test]
		public void Install()
		{
			var settings = new AndroidSdkManagerToolSettings {
                SkipVersionCheck=  true,
                SdkRoot = "/Users/redth/Library/Developer/Xamarin/android-sdk-macosx/"
			};

            this.Cake.AndroidSdkManagerInstall(new [] { "system-images;android-26;google_apis;x86" }, settings);

			var list = Cake.AndroidSdkManagerList(settings);

			Assert.NotNull(list.InstalledPackages.FirstOrDefault(ip => ip.Path == "extras;google;auto"));
		}
	}
}
