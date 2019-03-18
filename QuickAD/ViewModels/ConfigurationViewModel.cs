using System.Collections.ObjectModel;
using QuickAD.Helper_Classes;
using QuickAD.Models;
using QuickAD.Services;

namespace QuickAD.ViewModels
{
    class ConfigurationViewModel : ObservableObject, IPageViewModel
    {
		#region Fields

		private AdService _adService;
		private readonly ObservableCollection<AdConfiguration> _configs;
		private AdConfiguration _selectedConfig;
		private readonly ConfigDetailViewModel _configDetailViewModel;

		#endregion // Fields
		
		public ConfigurationViewModel(AdService adService)
		{
			_adService = adService;
			_configs = new ObservableCollection<AdConfiguration>(AdConfiguration.Configurations);
			_selectedConfig = AdConfiguration.CurrentConfiguration;
			_configDetailViewModel = new ConfigDetailViewModel();
		}

		#region Properties

		public string Name => "Settings";

		public ConfigDetailViewModel ConfigurationDetailViewModel => _configDetailViewModel;

		public ObservableCollection<AdConfiguration> Configs => _configs;

		public AdConfiguration SelectedConfig
		{
			get => _selectedConfig;
			set
			{
				if (_selectedConfig == value) return;
				_selectedConfig = value;
				_configDetailViewModel.Refresh(_selectedConfig);
				AdConfiguration.CurrentConfiguration = _selectedConfig;
				AdConfiguration.UpdateLastUsedConfig();
				OnPropertyChanged("SelectedConfig");
			}
		}

		#endregion // Properties

		#region Methods

		/// <summary>
		/// Save modified configuration to settings.json
		/// TODO: Implement configuration modifications, additions, and subtractions.
		/// </summary>
		private void SaveConfiguration()
        {

        }
        
        #endregion // Methods
    }
}
