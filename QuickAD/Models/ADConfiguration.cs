using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace QuickAD.Models
{
	class AdConfiguration
	{
		#region Properties

		public string ConfigName { get; set; }

		public string FilePath { get; set; }

		public string SitePrefix { get; set; }

		public string ComputerSearch { get; set; }

		public string StaffUserSearch { get; set; }

		public string NonStaffUserSearch { get; set; }

		public string DefaultConnection { get; set; }

		public static AdConfiguration CurrentConfiguration { get; set; }

		public static List<AdConfiguration> Configurations { get; private set; }

		#endregion // Properties

		#region Methods

		/// <summary>
		/// Use AppSettings in App.config to set the last used configuration as active.
		/// </summary>
		public static void SetCurrentConfigurationFromLastUsed()
		{
			var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
			var lastUsed = configFile.AppSettings.Settings["LastUsedConnections"];
			if (lastUsed == null)
			{
				lastUsed = configFile.AppSettings.Settings["DefaultConnections"];
			}

			foreach (var adConfiguration in Configurations)
			{
				if (lastUsed.Value == adConfiguration.SitePrefix)
				{
					CurrentConfiguration = adConfiguration;
				}
			}

			if (CurrentConfiguration == null) CurrentConfiguration = Configurations[0];
		}

		/// <summary>
		/// Populate the Configuration List from passed file paths.
		/// </summary>
		/// <param name="paths">File paths to create new AdConfiguration objects from.</param>
		public static void PopulateConfigurations(List<string> paths)
		{
			if (paths == null) return;
			var configs = new List<AdConfiguration>();
			foreach (var path in paths)
			{
				var tmp = GetConfigFromFile(path);
				if (tmp == new AdConfiguration()) continue;
				configs.Add(tmp);
			}

			Configurations = new List<AdConfiguration>(configs.OrderBy(u => u.ConfigName));
		}

		/// <summary>
		/// Create a new AdConfiguration from the file at the specified path.
		/// </summary>
		/// <param name="path">Path to config file.</param>
		/// <returns>New AdConfiguration, empty if error reading file.</returns>
		private static AdConfiguration GetConfigFromFile(string path = "")
		{
			path = Path.Combine(Directory.GetCurrentDirectory(), path);
			if (!File.Exists(path)) return new AdConfiguration();

			var config = new AdConfiguration
			{
				FilePath = path
			};
			try
			{
				using (var reader = new JsonTextReader(new StreamReader(path)))
				{
					while (reader.Read())
					{
						if (reader.Value != null)
						{
							if (reader.TokenType.ToString() == "PropertyName")
							{
								switch (reader.Value)
								{
									case "Name":
										reader.Read();
										config.ConfigName = reader.Value.ToString();
										break;
									case "ComputerSearch":
										reader.Read();
										config.ComputerSearch = reader.Value.ToString();
										break;
									case "StaffUserSearch":
										reader.Read();
										config.StaffUserSearch = reader.Value.ToString();
										break;
									case "NonStaffUserSearch":
										reader.Read();
										config.NonStaffUserSearch = reader.Value.ToString();
										break;
									case "DefaultConnection":
										reader.Read();
										config.DefaultConnection = reader.Value.ToString();
										break;
									case "SitePrefix":
										reader.Read();
										config.SitePrefix = reader.Value.ToString();
										break;
								}
							}
						}
					}
				}
			}
			catch (FileNotFoundException) { return new AdConfiguration(); }

			return config;
		}

		#endregion // Methods
	}
}
