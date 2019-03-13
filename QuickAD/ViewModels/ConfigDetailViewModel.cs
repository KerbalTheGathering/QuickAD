using QuickAD.Helper_Classes;
using QuickAD.Models;
using QuickAD.Services;

namespace QuickAD.ViewModels
{
    class ConfigDetailViewModel: ObservableObject, IPageViewModel
    {
		#region Fields

		private AdConfiguration _currentConfig;

		#endregion // Fields

		public ConfigDetailViewModel()
		{
			Refresh(AdConfiguration.CurrentConfiguration);
		}

		#region Properties

		public string Name => "Configuration Details";

		public string ConfigName
		{
			get => _currentConfig.ConfigName;
			set
			{
				if (_currentConfig.ConfigName == value) return;
				_currentConfig.ConfigName = value;
				OnPropertyChanged("ConfigName");
			}
		}

		public string SitePrefix
		{
			get => _currentConfig.SitePrefix;
			set
			{
				if (_currentConfig.SitePrefix == value) return;
				_currentConfig.SitePrefix = value;
				OnPropertyChanged("SitePrefix");
			}
		}

		public string ComputerConnection
		{
			get => _currentConfig.ComputerSearch;
			set
			{
				if (_currentConfig.ComputerSearch == value) return;
				_currentConfig.ComputerSearch = value;
				OnPropertyChanged("ComputerConnection");
			}
		}

		public string StaffUserConnection
		{
			get => _currentConfig.StaffUserSearch;
			set
			{
				if (_currentConfig.StaffUserSearch == value) return;
				_currentConfig.StaffUserSearch = value;
				OnPropertyChanged("StaffUserConnection");
			}
		}

		public string NonStaffUserConnection
		{
			get => _currentConfig.NonStaffUserSearch;
			set
			{
				if (_currentConfig.NonStaffUserSearch == value) return;
				{
					_currentConfig.NonStaffUserSearch = value;
					OnPropertyChanged("NonStaffUserConnection");
				}
			}
		}

		public string DefaultConnection
		{
			get => _currentConfig.DefaultConnection;
			set
			{
				if (_currentConfig.DefaultConnection == value) return;
				_currentConfig.DefaultConnection = value;
				OnPropertyChanged("DefaultConnection");
			}
		}

		#endregion // Properties

		#region Methods

		public void Refresh(AdConfiguration config)
		{
			if (config == new AdConfiguration())	return;
			_currentConfig = config;
			OnPropertyChanged("ConfigName");
			OnPropertyChanged("SitePrefix");
			OnPropertyChanged("ComputerConnection");
			OnPropertyChanged("StaffUserConnection");
			OnPropertyChanged("NonStaffUserConnection");
			OnPropertyChanged("DefaultConnection");
			//ComputerConnection = AdConfiguration.ComputerSearch;
			//StaffUserConnection = AdConfiguration.StaffUserSearch;
			//NonStaffUserConnection = AdConfiguration.NonStaffUserSearch;
			//DefaultConnection = AdConfiguration.DefaultConnection;
			//SitePrefix = AdConfiguration.SitePrefix;
		}

		#endregion // Methods
	}
}
