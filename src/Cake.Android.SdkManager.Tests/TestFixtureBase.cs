using Cake.Core;
using System;
using System.IO;

namespace Cake.AndroidSdkManager.Fakes
{
    public abstract class TestFixtureBase : IDisposable
    {
        FakeCakeContext context;

        public ICakeContext Cake { get { return context.CakeContext; } }

        public TestFixtureBase()
        {
            Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(typeof(TestFixtureBase).Assembly.Location);
            context = new FakeCakeContext();

            //var dp = new DirectoryPath("./testdata");
            //var d = context.CakeContext.FileSystem.GetDirectory(dp);

            //if (d.Exists)
            //  d.Delete(true);

            //d.Create();
        }

        public void Dispose()
        {
            context.DumpLogs();
            Directory.Delete("./tools", recursive: true);
        }
    }
}
