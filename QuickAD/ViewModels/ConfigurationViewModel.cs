using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Microsoft.Win32;
using QuickAD.Helper_Classes;
using QuickAD.Models;
using QuickAD.Services;

namespace QuickAD.ViewModels
{
    class ConfigurationViewModel : ObservableObject, IPageViewModel
    {
		#region Fields

		private AdService _adService;
		private ICommand _browseCommand;
		private List<string> _configNames;
		private string _selectedConfig;
		private ConfigDetailViewModel _configDetailViewModel;

		#endregion // Fields
		
		public ConfigurationViewModel(AdService adService)
		{
			_adService = adService;
			_configDetailViewModel = new ConfigDetailViewModel();
		}

		#region Properties

		public ICommand BrowseCommand
		{
			get
			{
				if (_browseCommand == null)
				{
					_browseCommand = new RelayCommand(
							p => BrowseForConfigFile(),
									null);
				}

				return _browseCommand;
			}
		}
		
		public string Name
		{
			get { return "Settings"; }
		}

		public List<string> ConfigNames
		{
			get => _configNames;
			set
			{
				if (_configNames == value) return;
				_configNames = value;
				OnPropertyChanged("ConfigNames");
			}
		}

		public string SelectedConfig
		{
			get => _selectedConfig;
			set
			{
				if (_selectedConfig == value) return;
				_selectedConfig = value;
				OnPropertyChanged("SelectedConfig");
			}
		}

		public string ConfigFile
		{
			get => ConfigurationDetailViewModel.FilePath;
			set
			{
				if (ConfigurationDetailViewModel.FilePath == value) return;
				ConfigurationDetailViewModel.FilePath = value;
				OnPropertyChanged("ConfigFile");
			}
		}

		public ConfigDetailViewModel ConfigurationDetailViewModel
		{
			get { return _configDetailViewModel; }
		}

		#endregion // Properties

		#region Methods

		private void BrowseForConfigFile()
		{
			var fileDialog = new OpenFileDialog
			{
				DefaultExt = ".json",
				Filter = "Json Files (*.json)|*.json",
				InitialDirectory = Directory.GetCurrentDirectory()
			};

			var result = fileDialog.ShowDialog();
			if (result != null && result.Value)
			{
				var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
				var connectionPath = configFile.AppSettings.Settings["LastUsedConnections"];
				if (connectionPath == null)
				{
					File.Copy(fileDialog.FileName, Path.Combine(Directory.GetCurrentDirectory(), fileDialog.SafeFileName));
					configFile.AppSettings.Settings.Add("LastUsedConnections", fileDialog.SafeFileName);
					configFile.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection("appSettings");
					connectionPath = configFile.AppSettings.Settings["LastUsedConnections"];
					//ConfigurationDetailViewModel.Refresh(connectionPath.Value);
				}
				else
				{
					if (fileDialog.FileName != Path.Combine(Directory.GetCurrentDirectory(), fileDialog.SafeFileName))
					{
						File.Copy(fileDialog.FileName,
							Path.Combine(Directory.GetCurrentDirectory(), fileDialog.SafeFileName), true);
					}
					connectionPath.Value = fileDialog.SafeFileName;
					configFile.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection("appSettings");
					//ConfigurationDetailViewModel.Refresh(connectionPath.Value);
				}
			}
		}

		private void SaveConfiguration()
        {

        }
        
        #endregion // Methods

    }
}
