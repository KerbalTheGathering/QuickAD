using QuickAD.Helper_Classes;
using System;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;
using QuickAD.Services;

namespace QuickAD.ViewModels
{
    class MultiEditViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private AdService _adService;
        private ICommand _browseCommand;
        private string _directoryToImportFrom;
		private readonly ComputerImportViewModel _computerImportViewModel;

        #endregion // Fields

        public MultiEditViewModel(AdService adService)
        {
	        _adService = adService;
			_computerImportViewModel = new ComputerImportViewModel(_adService);
        }

        #region Properties

        public string Name
        {
            get { return "Import"; }
        }

        public ComputerImportViewModel CompImportViewModel
        {
	        get { return _computerImportViewModel; }
        }

        public ICommand SearchCommand
        {
	        get
	        {
		        if (_browseCommand == null)
		        {
			        _browseCommand = new RelayCommand(
				        p => _browseForFile(),
				        p => p is string);
		        }

		        return _browseCommand;
	        }
        }

        public string DirectoryToImportFrom
        {
	        get { return _directoryToImportFrom; }
	        set
	        {
		        if (_directoryToImportFrom != value)
		        {
			        _directoryToImportFrom = value;
					OnPropertyChanged("DirectoryToImportFrom");
		        }
	        }
        }

		#endregion // Properties

		#region Methods

		private void _browseForFile()
		{
			var fileDialog = new OpenFileDialog
			{
				DefaultExt = ".csv",
				Filter = "Excel Files (*.csv, *xlsx, *.xls, *.xlw)|*.csv; *xlsx; *xls; *xlw",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			};

			var result = fileDialog.ShowDialog();
			if (result != null && result.Value)
			{
				_computerImportViewModel.SetFile(DirectoryToImportFrom = fileDialog.FileName);
			}
		}

		#endregion // Methods
	}
}
