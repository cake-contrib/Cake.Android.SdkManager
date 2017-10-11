using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.AndroidSdkManager
{
	internal abstract class ToolEx<TSettings> : Tool<TSettings> where TSettings : Cake.Core.Tooling.ToolSettings
	{
		private readonly ICakeEnvironment _environment;
		private readonly IFileSystem _fileSystem;
		private readonly IToolLocator _tools;

		public ToolEx(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools) : base(fileSystem, environment, processRunner, tools)
		{
			_fileSystem = fileSystem;
			_environment = environment;
			_tools = tools;
		}

		protected ToolExProcess RunProcessEx(TSettings settings, ProcessArgumentBuilder arguments)
		{
			// Should we customize the arguments?
			if (settings.ArgumentCustomization != null)
			{
				arguments = settings.ArgumentCustomization(arguments);
			}

			// Get the tool name.
			var toolName = GetToolName();

			// Get the tool path.
			var toolPath = GetToolPath(settings);
			if (toolPath == null || !_fileSystem.Exist(toolPath))
			{
				const string message = "{0}: Could not locate executable.";
				throw new CakeException(string.Format(CultureInfo.InvariantCulture, message, toolName));
			}

			// Get the working directory.
			var workingDirectory = GetWorkingDirectory(settings);
			if (workingDirectory == null)
			{
				const string message = "{0}: Could not resolve working directory.";
				throw new CakeException(string.Format(CultureInfo.InvariantCulture, message, toolName));
			}

			// Create the process start info.
			var info = new ProcessStartInfo(toolPath.MakeAbsolute(_environment).FullPath)
			{
				Arguments = arguments.Render(),
				WorkingDirectory = workingDirectory.FullPath,
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				RedirectStandardInput = true
			};

			//// Add environment variables
			//ProcessHelper.SetEnvironmentVariable(info, "CAKE", "True");
			//ProcessHelper.SetEnvironmentVariable(info, "CAKE_VERSION", _environment.Runtime.CakeVersion.ToString(3));
			//if (settings.EnvironmentVariables != null)
			//{
			//	foreach (var environmentVariable in settings.EnvironmentVariables)
			//	{
			//		ProcessHelper.SetEnvironmentVariable(info, environmentVariable.Key, environmentVariable.Value);
			//	}
			//}

			// Start and return the process.
			var process = Process.Start(info);

			if (process == null)
			{
				return null;
			}

			process.EnableRaisingEvents = true;

			var consoleOutputQueue = SubscribeStandardConsoleOutputQueue(process);
			var consoleErrorQueue = SubscribeStandardConsoleErrorQueue(process);

			var complete = Task.Run(() => {
				process.WaitForExit();
				return process.ExitCode;
			});

			return new ToolExProcess
			{
				Complete = complete,
				StandardOutput = consoleOutputQueue,
				StandardError = consoleErrorQueue,
				StandardInput = process.StandardInput
			};
		}


		protected class ToolExProcess
		{
			public Task<int> Complete { get; set; }
			public ConcurrentQueue<string> StandardOutput { get; set; }
			public ConcurrentQueue<string> StandardError { get; set; }
			public StreamWriter StandardInput { get; set; }
		}


		private new FilePath GetToolPath(TSettings settings)
		{
			return GetToolPathUsingToolService(settings);
		}

		private FilePath GetToolPathUsingToolService(TSettings settings)
		{
			var toolPath = settings.ToolPath;
			if (toolPath != null)
			{
				return toolPath.MakeAbsolute(_environment);
			}

			// Look for each possible executable name in various places.
			var toolExeNames = GetToolExecutableNames();
			foreach (var toolExeName in toolExeNames)
			{
				var result =  _tools.Resolve(toolExeName);
				if (result != null)
				{
					return result;
				}
			}

			// Look through all the alternative directories for the tool.
			var alternativePaths = GetAlternativeToolPaths(settings) ?? Enumerable.Empty<FilePath>();
			foreach (var alternativePath in alternativePaths)
			{
				if (_fileSystem.Exist(alternativePath))
				{
					return alternativePath.MakeAbsolute(_environment);
				}
			}

			return null;
		}

		private static ConcurrentQueue<string> SubscribeStandardConsoleErrorQueue(Process process)
		{
			var consoleErrorQueue = new ConcurrentQueue<string>();
			process.ErrorDataReceived += (s, e) =>
			{
				if (e.Data != null)
				{
					consoleErrorQueue.Enqueue(e.Data);
				}
			};
			process.BeginErrorReadLine();
			return consoleErrorQueue;
		}

		private static ConcurrentQueue<string> SubscribeStandardConsoleOutputQueue(Process process)
		{
			var consoleOutputQueue = new ConcurrentQueue<string>();
			process.OutputDataReceived += (s, e) =>
			{
				if (e.Data != null)
				{
					consoleOutputQueue.Enqueue(e.Data);
				}
			};
			process.BeginOutputReadLine();
			return consoleOutputQueue;
		}
	}
}
