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
		public void List()
		{
			var list = this.Cake.AndroidSdkManagerList(new AndroidSdkManagerToolSettings {
				SdkRoot = ANDROID_SDK_ROOT
			});

			Assert.NotNull(list);
		}

		[Test]
		public void Install()
		{
			var settings = new AndroidSdkManagerToolSettings {
				SdkRoot = ANDROID_SDK_ROOT
			};

			this.Cake.AndroidSdkManagerInstall(new [] { "extras;google;auto" }, settings);

			var list = Cake.AndroidSdkManagerList(settings);

			Assert.NotNull(list.InstalledPackages.FirstOrDefault(ip => ip.Path == "extras;google;auto"));
		}
	}
}
