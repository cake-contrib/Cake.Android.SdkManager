using Cake.Core.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cake.AndroidSdkManager
{
	internal static class BuildToolsParser
	{
		internal static AndroidSdkManagerList ParseSdkManagerList(IProcess process)
		{
			var result = new AndroidSdkManagerList();

			int section = 0;
			bool isDependencies = false;
			var bufferedLines = new Stack<string>();

			foreach (var line in process.GetStandardOutput())
			{
				if (line.ToLowerInvariant().Contains("installed packages:"))
				{
					section = 1;
					continue;
				}
				else if (line.ToLowerInvariant().Contains("available packages:"))
				{
					section = 2;
					continue;
				}
				else if (line.ToLowerInvariant().Contains("available updates:"))
				{
					section = 3;
					continue;
				}

				if (section >= 1 && section <= 3)
				{
					if (line.ToLowerInvariant().Contains("dependencies"))
					{
						isDependencies = true;
						continue;
					}

					if (string.IsNullOrWhiteSpace(line) && bufferedLines.Count > 0)
					{
						ParseBufferedData(result, section, bufferedLines);
						isDependencies = false;
						continue;
					}

					if (Regex.IsMatch(line, "^([a-z])", RegexOptions.IgnoreCase | RegexOptions.Compiled))
					{
						if (bufferedLines.Count > 0 && section == 3)
							ParseBufferedData(result, section, bufferedLines);

						bufferedLines.Push(line);
						continue;
					}

					var parts = line.Split(':');

					// These lines are not actually good data, skip them
					if (parts == null || parts.Length <= 1
						|| parts[0].ToLowerInvariant().Contains("------")
						|| isDependencies)
						continue;
					else
						bufferedLines.Push(string.Join(":", parts.Skip(1).ToArray()));
				}
			}

			return result;
		}

		private static void ParseBufferedData(AndroidSdkManagerList result, int section, Stack<string> bufferStack)
		{
			if (section == 1)
			{
				result.InstalledPackages.Add(new InstalledAndroidSdkPackage
				{
					Location = bufferStack.Pop()?.Trim(),
					Version = bufferStack.Pop()?.Trim(),
					Description = bufferStack.Pop()?.Trim(),
					Path = bufferStack.Pop()?.Trim()
				});
			}
			else if (section == 2)
			{
				result.AvailablePackages.Add(new AndroidSdkPackage
				{
					Version = bufferStack.Pop()?.Trim(),
					Description = bufferStack.Pop()?.Trim(),
					Path = bufferStack.Pop()?.Trim()
				});
			}
			else if (section == 3)
			{
				result.AvailableUpdates.Add(new AvailableAndroidSdkUpdate
				{
					AvailableVersion = bufferStack.Pop()?.Trim(),
					InstalledVersion = bufferStack.Pop()?.Trim(),
					Path = bufferStack.Pop()?.Trim()
				});
			}
		}
	}
}
