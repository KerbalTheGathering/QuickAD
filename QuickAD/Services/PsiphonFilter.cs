using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace QuickAD.Services
{
	class PsiphonFilter
	{
		private static string wallpaperPath = @"D:\Pictures\psiphonFilter\PsiphonFilter.jpg";

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SystemParametersInfo(uint uiAction, uint uiParam,
			string pvParam, uint fWinIni);

		private const uint SPI_SETDESKWALLPAPER = 0x0014;
		private const uint SPIF_UPDATEINIFILE = 0x01;
		private const uint SPIF_SENDWININICHANGE = 0x02;

		public static void SearchRemoteDirectory(string compName)
		{
			var startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			if (!File.Exists(wallpaperPath)) return;
			var remoteC = "\\\\"+ compName + "\\c$\\users";
			if (Directory.Exists(remoteC))
			{
				Console.Out.WriteLine("Remote directory accessed.\n" + remoteC);
				var users = Directory.GetDirectories(remoteC);
				foreach (var user in users)
				{
					if (user == Path.Combine(remoteC, "gbloo"))
					{
						var appData = Path.Combine(user, "AppData\\Roaming");
						var appDataDirectories = Directory.GetDirectories(appData);
						foreach (var appDataDirectory in appDataDirectories)
						{
							if (!appDataDirectory.Contains("Psiphon")) continue;
							PsiphonDetectedAction(user, appDataDirectory);
							break;
						}
					}
				}
			}
		}

		private static void PsiphonDetectedAction(string user, string psiphonDirectory)
		{
			var userName = Path.GetFileName(user);
			RecordPsiphonUsage(userName, psiphonDirectory);
			ChangeWallpaper();
			DeletePsiphon(user);
		}

		private static void RecordPsiphonUsage(string userName, string psiphonDirectory)
		{
			Console.Out.WriteLine("Psiphon Found!");
			Console.Out.WriteLine("User: " + userName);
			Console.Out.WriteLine("Last Used: " + Directory.GetLastAccessTime(psiphonDirectory));
		}

		//Change this to copy the Wallpaper changing .exe to user's Startup Folder
		private static void ChangeWallpaper()
		{
			if (!SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath,
				SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE))
			{
				throw new Win32Exception();
			}
		}

		private static void DeletePsiphon(string user)
		{
			var documents = Path.Combine(user, "Documents");
			var desktop = Path.Combine(user, "Desktop");
			var downloads = Path.Combine(user, "Downloads");

			var docDirectories = Directory.GetDirectories(documents);
			for (int i = 0; i < docDirectories.Length; i++)
			{
				if (docDirectories[i].Contains("Psiphon"))
					Directory.Delete(docDirectories[i], true);
			}
			var deskDirectories = Directory.GetDirectories(desktop);
			for (int i = 0; i < deskDirectories.Length; i++)
			{
				if (deskDirectories[i].Contains("Psiphon"))
					Directory.Delete(deskDirectories[i], true);
			}
			var downDirectories = Directory.GetDirectories(downloads);
			for(int i = 0; i < downDirectories.Length; i++)
			{
				if (downDirectories[i].Contains("Psiphone"))
					Directory.Delete(downDirectories[i], true);
			}
		}
	}
}
