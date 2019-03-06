using QuickAD.Helper_Classes;
using QuickAD.Services;
using System.Windows.Input;

namespace QuickAD.ViewModels
{
    class SingleEditViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private AdService _adService;
		private ICommand _searchCommand;
		private ComputerSearchViewModel _computerSearchViewModel;

        #endregion // Fields

        public SingleEditViewModel(AdService adService)
        {
	        _adService = adService;
			_computerSearchViewModel = new ComputerSearchViewModel(adService);
        }

        #region Properties

        public string Name
        {
            get
            {
                return "Search";
            }
        }

		public ICommand SearchCommand
		{
			get
			{
				if (_searchCommand == null)
				{
					_searchCommand = new RelayCommand(
						p => SearchComputers((string)p),
						p => p is string);
				}

				return _searchCommand;
			}
		}

		public ComputerSearchViewModel CompSearchViewModel
		{
			get { return _computerSearchViewModel; }
		}


		#endregion // Properties

		#region Methods

		public void SearchComputers(string search)
		{
			if (!string.IsNullOrEmpty(search))
			{
				CompSearchViewModel.Search(search);
			}
		}

		#endregion // Methods
	}
}
