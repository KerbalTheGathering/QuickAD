using System.IO;
using Newtonsoft.Json;

namespace QuickAD.Models
{
	static class AdConfiguration
	{
		#region Fields

		private static string _configFilePath;
		private static string _sitePrefix;
		private static string _defaultConnection;
		private static string _computerSearch;
		private static string _staffUserSearch;
		private static string _nonStaffUserSearch;

		#endregion // Fields

		public static void EmptyAdConfiguration()
		{
			_sitePrefix = "";
			_defaultConnection = "";
			_computerSearch = "";
			_staffUserSearch = "";
			_nonStaffUserSearch = "";
		}

		public static void SetAdConfiguration(string sitePrefix, string defaultConnection, string computerSearch, string staffUserSearch, string nonStaffUserSearch)
		{
			_sitePrefix = sitePrefix;
			_defaultConnection = defaultConnection;
			_computerSearch = computerSearch;
			_staffUserSearch = staffUserSearch;
			_nonStaffUserSearch = nonStaffUserSearch;
		}

		#region Properties

		public static string FilePath
		{
			get { return _configFilePath; }
			set { _configFilePath = value; }
		}

		public static string SitePrefix
		{
			get { return _sitePrefix; }
			set { _sitePrefix = value; }
		}

		public static string ComputerSearch
		{
			get { return _computerSearch; }
			set { _computerSearch = value; }
		}

		public static string StaffUserSearch
		{
			get { return _staffUserSearch; }
			set { _staffUserSearch = value; }
		}

		public static string NonStaffUserSearch
		{
			get { return _nonStaffUserSearch; }
			set { _nonStaffUserSearch = value; }
		}

		public static string DefaultConnection
		{
			get { return _defaultConnection; }
			set { _defaultConnection = value; }
		}

		#endregion // Properties
		
		#region Methods

		public static void GetConfigFromFile(string path = "")
		{
			EmptyAdConfiguration();
			path = Path.Combine(Directory.GetCurrentDirectory(), path);
			if (path == "" || !File.Exists(path)) return;

			_configFilePath = path;
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
										_computerSearch = reader.Value.ToString();
										break;
									case "StaffUserSearch":
										reader.Read();
										_staffUserSearch = reader.Value.ToString();
										break;
									case "NonStaffUserSearch":
										reader.Read();
										_nonStaffUserSearch = reader.Value.ToString();
										break;
									case "DefaultConnection":
										reader.Read();
										_defaultConnection = reader.Value.ToString();
										break;
									case "SitePrefix":
										reader.Read();
										_sitePrefix = reader.Value.ToString();
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
