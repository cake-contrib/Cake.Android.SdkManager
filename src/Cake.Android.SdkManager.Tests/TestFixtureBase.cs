using Cake.Core;
using Cake.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
        }
    }
}
