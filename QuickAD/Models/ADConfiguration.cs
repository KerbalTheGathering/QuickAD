using System.IO;
using Newtonsoft.Json;

namespace QuickAD.Models
{
	static class AdConfiguration
	{
		#region Properties

		public static string Name { get; set; }

		public static string FilePath { get; set; }

		public static string SitePrefix { get; set; }

		public static string ComputerSearch { get; set; }

		public static string StaffUserSearch { get; set; }

		public static string NonStaffUserSearch { get; set; }

		public static string DefaultConnection { get; set; }

		#endregion // Properties
		
		#region Methods

		public static void GetConfigFromFile(string path = "")
		{
			path = Path.Combine(Directory.GetCurrentDirectory(), path);
			if (!File.Exists(path)) return;

			FilePath = path;
			try
			{
				using (JsonTextReader reader = new JsonTextReader(new StreamReader(path)))
				{
					while (reader.Read())
					{
						if (reader.Value != null)
						{
							if (reader.TokenType.ToString() == "PropertyName")
							{
								switch (reader.Value)
								{
									case "ComputerSearch":
										reader.Read();
										ComputerSearch = reader.Value.ToString();
										break;
									case "StaffUserSearch":
										reader.Read();
										StaffUserSearch = reader.Value.ToString();
										break;
									case "NonStaffUserSearch":
										reader.Read();
										NonStaffUserSearch = reader.Value.ToString();
										break;
									case "DefaultConnection":
										reader.Read();
										DefaultConnection = reader.Value.ToString();
										break;
									case "SitePrefix":
										reader.Read();
										SitePrefix = reader.Value.ToString();
										break;
								}
							}
						}
					}
				}
			}
			catch (FileNotFoundException) { }
		}

		#endregion // Methods
	}
}
