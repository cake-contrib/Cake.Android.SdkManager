using Cake.Core.IO;

namespace Cake.AndroidSdkManager
{
	internal static class LegacyBuildToolsParser
	{
		internal static AndroidSdkManagerList ParseSdkManagerList(IProcess process)
		{
			var result = new AndroidSdkManagerList();
			int section = 0;

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
					var parts = line.Split('|');

					// These lines are not actually good data, skip them
					if (parts == null || parts.Length <= 1
						|| parts[0].ToLowerInvariant().Contains("path")
						|| parts[0].ToLowerInvariant().Contains("id")
						|| parts[0].ToLowerInvariant().Contains("------"))
						continue;

					// If we got here, we should have a good line of data
					if (section == 1)
					{
						result.InstalledPackages.Add(new InstalledAndroidSdkPackage
						{
							Path = parts[0]?.Trim(),
							Version = parts[1]?.Trim(),
							Description = parts[2]?.Trim(),
							Location = parts[3]?.Trim()
						});
					}
					else if (section == 2)
					{
						result.AvailablePackages.Add(new AndroidSdkPackage
						{
							Path = parts[0]?.Trim(),
							Version = parts[1]?.Trim(),
							Description = parts[2]?.Trim()
						});
					}
					else if (section == 3)
					{
						result.AvailableUpdates.Add(new AvailableAndroidSdkUpdate
						{
							Path = parts[0]?.Trim(),
							InstalledVersion = parts[1]?.Trim(),
							AvailableVersion = parts[2]?.Trim()
						});
					}
				}
			}

			return result;
		}
	}
}
