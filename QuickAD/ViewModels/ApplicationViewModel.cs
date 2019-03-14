using QuickAD.Helper_Classes;
using QuickAD.Models;
using QuickAD.Services;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace QuickAD.ViewModels
{
	public class ApplicationViewModel : ObservableObject
    {
        #region Fields

        private ICommand _changePageCommand;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;


        #endregion // Fields

        public ApplicationViewModel()
        {
			DescriptionPrefix.Initialize();
			LoadAppSettings();
			var adService = new AdService();
			PageViewModels.Add(new HomeViewModel(adService));
			PageViewModels.Add(new ComputerViewModel(adService));
			PageViewModels.Add(new PasswordViewModel(adService));
			PageViewModels.Add(new ConfigurationViewModel(adService));
			CurrentPageViewModel = PageViewModels[0];
        }

        #region Properties / Commands

        public ICommand ChangePageCommand
        {
            get
            {
	            return _changePageCommand ?? (_changePageCommand = new RelayCommand(
		                   p => ChangeViewModel((IPageViewModel) p),
		                   p => p is IPageViewModel));
            }
        }

        public List<IPageViewModel> PageViewModels => _pageViewModels ?? (_pageViewModels = new List<IPageViewModel>());

        public IPageViewModel CurrentPageViewModel
        {
            get => _currentPageViewModel;
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        #endregion // Properties / Commands

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        private void LoadAppSettings()
        {
	        var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
	        var connectionPath = configFile.AppSettings.Settings["LastUsedConnections"];
	        if (connectionPath == null)
	        {
		        connectionPath = configFile.AppSettings.Settings["DefaultConnections"];
				configFile.AppSettings.Settings.Add("LastUsedConnections", connectionPath.Value);
				configFile.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection("appSettings");
				AdConfiguration.PopulateConfigurations(Path.Combine(Directory.GetCurrentDirectory(), connectionPath.Value));
				//AdConfiguration.GetConfigFromFile(Path.Combine(Directory.GetCurrentDirectory(), connectionPath.Value));
	        }
	        else
	        {
		        //AdConfiguration.GetConfigFromFile(connectionPath.Value);
			}
		}

        #endregion // Methods
    }
}
