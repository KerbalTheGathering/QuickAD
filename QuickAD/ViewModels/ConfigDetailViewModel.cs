using QuickAD.Helper_Classes;
using QuickAD.Models;
using QuickAD.Services;

namespace QuickAD.ViewModels
{
    class ConfigDetailViewModel: ObservableObject, IPageViewModel
    {
		#region Fields

		private readonly AdService _adService;
		private string _sitePrefix;
		private string _computerSearch;
		private string _staffUserSearch;
		private string _nonStaffUserSearch;
		private string _defaultConnection;

		#endregion // Fields

		public ConfigDetailViewModel(AdService adService)
		{
			_adService = adService;
			Refresh(AdConfiguration.FilePath);
		}

		#region Properties

		public string Name { get { return "Configuration Details"; } }

		public string SitePrefix
		{
			get { return _sitePrefix; }
			set
			{
				if (_sitePrefix != value)
				{
					_sitePrefix = value;
					OnPropertyChanged("SitePrefix");
				}
			}
		}

		public string ComputerConnection
		{
			get { return _computerSearch; }
			set
			{
				if (_computerSearch != value)
				{
					_computerSearch = value;
					OnPropertyChanged("ComputerConnection");
				}
			}
		}

		public string StaffUserConnection
		{
			get { return _staffUserSearch; }
			set
			{
				if (_staffUserSearch != value)
				{
					_staffUserSearch = value;
					OnPropertyChanged("StaffUserConnection");
				}
			}
		}

		public string NonStaffUserConnection
		{
			get { return _nonStaffUserSearch; }
			set
			{
				if (_nonStaffUserSearch != value)
				{
					_nonStaffUserSearch = value;
					OnPropertyChanged("NonStaffUserConnection");
				}
			}
		}

		public string DefaultConnection
		{
			get { return _defaultConnection; }
			set
			{
				if (_defaultConnection != value)
				{
					_defaultConnection = value;
					OnPropertyChanged("DefaultConnection");
				}
			}
		}

		#endregion // Properties

		#region Methods

		public void Refresh(string path)
		{
			if (path != AdConfiguration.FilePath)	AdConfiguration.GetConfigFromFile(path);
			ComputerConnection = AdConfiguration.ComputerSearch;
			StaffUserConnection = AdConfiguration.StaffUserSearch;
			NonStaffUserConnection = AdConfiguration.NonStaffUserSearch;
			DefaultConnection = AdConfiguration.DefaultConnection;
			SitePrefix = AdConfiguration.SitePrefix;
		}

		#endregion // Methods
	}
}
