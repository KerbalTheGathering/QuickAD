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
		public static void InitializeConfiguration()
		{
			PopulateConfigurations(Path.Combine(Directory.GetCurrentDirectory(), "settings.json"));

			var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
			var lastUsed = configFile.AppSettings.Settings["LastUsedConfig"];
			if (lastUsed.Value == string.Empty)
			{
				CurrentConfiguration = Configurations[0];
				lastUsed.Value = CurrentConfiguration.ConfigName;
				configFile.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection("appSettings");
			}
			else
			{
				foreach (var adConfiguration in Configurations)
				{
					if (lastUsed.Value == adConfiguration.ConfigName)
					{
						CurrentConfiguration = adConfiguration;
					}
				}
			}
		}

		/// <summary>
		/// Update the application's config file with the name of the current connection configuration.
		/// </summary>
		public static void UpdateLastUsedConfig()
		{
			var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
			var lastUsed = configFile.AppSettings.Settings["LastUsedConfig"];
			lastUsed.Value = CurrentConfiguration.ConfigName;
			configFile.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection("appSettings");
		}

		/// <summary>
		/// Populate Configuration List from file containing multiple configurations.
		/// </summary>
		/// <param name="path">Path to config file.</param>
		public static void PopulateConfigurations(string path)
		{
			if (path == null || !File.Exists(path)) return;

			var configs = new List<AdConfiguration>();
			using (var reader = new JsonTextReader(new StreamReader(path)))
			{
				while (reader.Read())
				{
					if(reader.TokenType == JsonToken.PropertyName 
					   && reader.Value.ToString() == "Configurations")
					{
						reader.Read();
						if (reader.TokenType != JsonToken.StartArray) continue;
						while(reader.TokenType != JsonToken.EndArray)
							configs.Add(ParseConfiguration(reader));

						break;
					}
				}
			}
			Configurations = new List<AdConfiguration>(configs.OrderBy(u => u.ConfigName));
		}

		/// <summary>
		/// Parse json object to AdConfiguration object
		/// </summary>
		/// <param name="reader"></param>
		/// <returns>Newly parsed AdConfiguration</returns>
		private static AdConfiguration ParseConfiguration(JsonTextReader reader)
		{
			var newConfig = new AdConfiguration();
			while (reader.TokenType != JsonToken.EndObject)
			{
				if (reader.TokenType != JsonToken.PropertyName)
				{
					reader.Read();
					continue;
				}
				switch (reader.Value)
				{
					case "Name":
						reader.Read();
						newConfig.ConfigName = reader.Value.ToString();
						break;
					case "ComputerSearch":
						reader.Read();
						newConfig.ComputerSearch = reader.Value.ToString();
						break;
					case "StaffUserSearch":
						reader.Read();
						newConfig.StaffUserSearch = reader.Value.ToString();
						break;
					case "NonStaffUserSearch":
						reader.Read();
						newConfig.NonStaffUserSearch = reader.Value.ToString();
						break;
					case "DefaultConnection":
						reader.Read();
						newConfig.DefaultConnection = reader.Value.ToString();
						break;
					case "SitePrefix":
						reader.Read();
						newConfig.SitePrefix = reader.Value.ToString();
						break;
				}
			}

			reader.Read();
			return newConfig;
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
