using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QuickAD.Helper_Classes;
using QuickAD.Services;

namespace QuickAD.ViewModels
{
    class PasswordViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private ICommand _searchCommand;
        private readonly SearchResultViewModel _searchResultViewModel;
        private AdService _adService;

        #endregion // Fields

        public PasswordViewModel(AdService adService)
        {
	        _adService = adService;
			_searchResultViewModel = new SearchResultViewModel(_adService);
        }

        #region Properties / Commands

        public string Name
        {
            get { return "Users"; }
        }

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(
                        p => SearchUsersAsync((string)p),
                        p => p is string);
                }

                return _searchCommand;
            }
        }

        public SearchResultViewModel SearchResultViewModel
        {
	        get { return _searchResultViewModel; }
        }

        #endregion // Properties

        #region Methods

        private async void SearchUsersAsync(string userName)
        {
	        if (string.IsNullOrEmpty(userName) || userName.Length <= 2) return;

			SearchResultViewModel.SetResultMessage(string.Empty);
			var searchResult = await _adService.SearchUserAsync(userName, SearchResultViewModel.SetResultMessage);
			SearchResultViewModel.SearchResultList = searchResult;
		}

		#endregion // Methods
	}
}
