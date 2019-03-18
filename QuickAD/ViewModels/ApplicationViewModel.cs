using QuickAD.Helper_Classes;
using QuickAD.Models;
using QuickAD.Services;
using System.Collections.Generic;
using System.Linq;
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
	            if (_currentPageViewModel == value) return;
	            _currentPageViewModel = value;
                OnPropertyChanged("CurrentPageViewModel");
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

		/// <summary>
		/// Initializes app settings and connection configurations.
		/// </summary>
		private void LoadAppSettings()
		{
			DescriptionPrefix.Initialize();
			AdConfiguration.InitializeConfiguration();
		}

        #endregion // Methods
    }
}
