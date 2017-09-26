using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cake.AndroidSdkManager
{
	internal class AndroidSdkManagerTool : ToolEx<AndroidSdkManagerToolSettings>
	{
		public AndroidSdkManagerTool(ICakeContext cakeContext, IFileSystem fileSystem, ICakeEnvironment cakeEnvironment, IProcessRunner processRunner, IToolLocator toolLocator)
			: base(fileSystem, cakeEnvironment, processRunner, toolLocator)
		{
			context = cakeContext;
			environment = cakeEnvironment;
		}

		ICakeContext context;
		ICakeEnvironment environment;

		protected override string GetToolName()
		{
			return "sdkmanager";
		}

		protected override IEnumerable<string> GetToolExecutableNames()
		{
			return new List<string> {
				"sdkmanager",
				"sdkmanager.bat"
			};
		}

		protected override IEnumerable<FilePath> GetAlternativeToolPaths(AndroidSdkManagerToolSettings settings)
		{
			var results = new List<FilePath>();

			var ext = environment.Platform.IsUnix() ? "" : ".bat";
			var androidHome = settings.SdkRoot.MakeAbsolute(environment).FullPath;

			if (!System.IO.Directory.Exists (androidHome))
				androidHome = environment.GetEnvironmentVariable("ANDROID_HOME");

			if (!string.IsNullOrEmpty(androidHome) && System.IO.Directory.Exists(androidHome))
			{
				var exe = new DirectoryPath(androidHome).Combine("tools").Combine("bin").CombineWithFilePath("sdkmanager" + ext);
				results.Add(exe);
			}

			return results;
		}

		public AndroidSdkManagerList List(AndroidSdkManagerToolSettings settings)
		{
			if (settings == null)
				settings = new AndroidSdkManagerToolSettings();

			var version = GetBuildToolsVersion(settings);

			//adb devices -l
			var builder = new ProcessArgumentBuilder();

			builder.Append("--list");

			BuildStandardOptions(settings, builder);

			var p = RunProcess(settings, builder, new ProcessSettings
			{
				RedirectStandardOutput = true,
			});

			//var processField = p.GetType().GetField("_process", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance);

			//var process = (System.Diagnostics.Process)processField.GetValue(p);
			//process.StartInfo.RedirectStandardInput = true;
			//process.StandardInput.WriteLine("y");

			p.WaitForExit();

			if (version.StartsWith("26"))
				return BuildToolsParser.ParseSdkManagerList(p);
			else
				return LegacyBuildToolsParser.ParseSdkManagerList(p);
		}

		private string GetBuildToolsVersion(AndroidSdkManagerToolSettings settings)
		{
			var builder = new ProcessArgumentBuilder();

			builder.Append("--version");

			var p = RunProcess(settings, builder, new ProcessSettings
			{
				RedirectStandardOutput = true,
			});

			p.WaitForExit();

			foreach (var line in p.GetStandardOutput())
			{
				if (Regex.IsMatch(line, @"^\d", RegexOptions.Compiled))
				{
					return line.Trim();
				}

				if (line.ToLowerInvariant().Contains("unknown argument"))
				{
					break;
				}
			}

			return "";
		}

		public bool InstallOrUninstall(bool install, IEnumerable<string> packages, AndroidSdkManagerToolSettings settings)
		{
			if (settings == null)
				settings = new AndroidSdkManagerToolSettings();

			//adb devices -l
			var builder = new ProcessArgumentBuilder();

			if (!install)
				builder.Append("--uninstall");
			
			foreach (var pkg in packages)
				builder.AppendQuoted(pkg);

			BuildStandardOptions(settings, builder);

			var pex = RunProcessEx(settings, builder);

			pex.StandardInput.WriteLine("y");

			pex.Complete.Wait();

			foreach (var line in pex.StandardOutput)
			{
				if (line.StartsWith("Info:", StringComparison.InvariantCultureIgnoreCase))
					this.context.Log.Write(Core.Diagnostics.Verbosity.Diagnostic, Core.Diagnostics.LogLevel.Information, line);
			}

			return true;
		}

		public bool UpdateAll(AndroidSdkManagerToolSettings settings)
		{
			if (settings == null)
				settings = new AndroidSdkManagerToolSettings();

			//adb devices -l
			var builder = new ProcessArgumentBuilder();

			builder.Append("update");

			BuildStandardOptions(settings, builder);

			var pex = RunProcessEx(settings, builder);

			pex.StandardInput.WriteLine("y");

			pex.Complete.Wait();

			foreach (var line in pex.StandardOutput)
			{
				if (line.StartsWith("Info:", StringComparison.InvariantCultureIgnoreCase))
					this.context.Log.Write(Core.Diagnostics.Verbosity.Diagnostic, Core.Diagnostics.LogLevel.Information, line);
			}

			return true;
		}

		void BuildStandardOptions(AndroidSdkManagerToolSettings settings, ProcessArgumentBuilder builder)
		{
			builder.Append("--verbose");

			if (settings.Channel != AndroidSdkChannel.Stable)
				builder.Append("--channel=" + (int)settings.Channel);

			if (settings.SdkRoot != null)
				builder.Append("--sdk_root=\"{0}\"", settings.SdkRoot.MakeAbsolute(environment));

			if (settings.IncludeObsolete)
				builder.Append("--include_obsolete");

			if (settings.NoHttps)
				builder.Append("--no_https");

			if (settings.ProxyType != AndroidSdkManagerProxyType.None)
			{
				builder.Append("--proxy={0}", settings.ProxyType.ToString().ToLower());

				if (!string.IsNullOrEmpty(settings.ProxyHost))
					builder.Append("--proxy_host=\"{0}\"", settings.ProxyHost);

				if (settings.ProxyPort > 0)
					builder.Append("--proxy_port=\"{0}\"", settings.ProxyPort);
			}
		}
	}
}
