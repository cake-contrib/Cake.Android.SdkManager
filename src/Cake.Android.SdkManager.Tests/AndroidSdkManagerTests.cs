using System;
using System.IO;
using Cake.Core.IO;
using Cake.AndroidSdkManager.Fakes;
using Xunit;
using System.Linq;

namespace Cake.AndroidSdkManager.Tests
{
    public class Tests : TestFixtureBase
    {
        const string ANDROID_SDK_ROOT = "../../../android-sdk/";

        [Fact]
        public void DownloadDefault()
        {
            // Arrange / Act
            Cake.AndroidSdkManagerDownload(null, null);

            // Assert
            Assert.True(Cake.FileSystem.Exist(new FilePath("./tools/androidsdk/tools/bin/sdkmanager"))
                || Cake.FileSystem.Exist(new FilePath("./tools/androidsdk/tools/bin/sdkmanager.bat")));
            Assert.True(ReadSdkVersion() >= new Version("26.1.1"));
        }
        
        [Fact]
        public void DownloadSpecificVersion()
        {
            // Arrange / Act
            var expected = new Version("26.1.1");
            Cake.AndroidSdkManagerDownload(specificVersion: expected);

            // Assert
            Assert.True(Cake.FileSystem.Exist(new FilePath("./tools/androidsdk/tools/bin/sdkmanager"))
                || Cake.FileSystem.Exist(new FilePath("./tools/androidsdk/tools/bin/sdkmanager.bat")));
            Assert.True(ReadSdkVersion() == expected);
        }

        [Fact]
        public void List()
        {
            // Arrange
            Cake.AndroidSdkManagerDownload();
            
            // Act
            var list = Cake.AndroidSdkManagerList(new AndroidSdkManagerToolSettings
            {
                SdkRoot = ANDROID_SDK_ROOT,
                SkipVersionCheck = false
            });

            // Assert
            Assert.NotNull(list);

            foreach (var a in list.AvailablePackages)
                Console.WriteLine($"{a.Description}\t{a.Version}\t{a.Path}");

            foreach (var a in list.InstalledPackages)
                Console.WriteLine($"{a.Description}\t{a.Version}\t{a.Path}");
        }

        [Fact]
        public void Install()
        {
            // Arrange
            Cake.AndroidSdkManagerDownload();
            var settings = new AndroidSdkManagerToolSettings
            {
                SkipVersionCheck = true,
                SdkRoot = "/Users/redth/Library/Developer/Xamarin/android-sdk-macosx/"
            };

            // Act
            Cake.AndroidSdkManagerInstall(new[] { "system-images;android-26;google_apis;x86" }, settings);

            // Assert
            var list = Cake.AndroidSdkManagerList(settings);

            Assert.NotNull(list.InstalledPackages.FirstOrDefault(ip => ip.Path == "extras;google;auto"));
        }

        private static Version ReadSdkVersion()
        {
            var lines = File.ReadAllLines("./tools/androidsdk/tools/source.properties");
            var revision = lines.First(line => line.StartsWith("Pkg.Revision"));
            return new Version(revision.Substring("Pkg.Revision=".Length));
        }
    }
}
