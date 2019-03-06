using System;
using QuickAD.Helper_Classes;
using QuickAD.Models;
using QuickAD.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace QuickAD.ViewModels
{
	class ComputerSearchViewModel: ObservableObject, IPageViewModel
	{
		#region Fields

		private readonly AdService _adService;
		private ICommand _saveChangesCommand;
		private ObservableCollection<Computer> _computerSearchResults;
		private Computer _selectedComputer;
		private string _selectedDescription = "";
		private string _resultMessageText;
		private bool _isSearching;
		private bool _isSaving;
		private readonly Computer _nullErrorComputer;

		#endregion // Fields

		public ComputerSearchViewModel(AdService adService)
		{
			_adService = adService;
			_isSearching = _isSaving = false;
			_nullErrorComputer = new Computer()
			{
				Name = "Null Error",
				DescPrefix = "None",
				Description = ""
			};
		}

		#region Properties

		public string Name
		{
			get
			{
				return "Computer Search";
			}
		}

		public ICommand SaveChangesCommand
		{
			get
			{
				if (_saveChangesCommand == null)
				{
					_saveChangesCommand = new RelayCommand(
						p => SaveChangesAsync((Computer)p),
						p => p is Computer);
				}

				return _saveChangesCommand;
			}
		}

		public ObservableCollection<Computer> SearchResultList
		{
			get { return _computerSearchResults; }
			set
			{
				if (_computerSearchResults != value)
				{
					_computerSearchResults = value;
					OnPropertyChanged("SearchResultList");
				}
			}
		}

		public string ResultMessageText
		{
			get { return _resultMessageText; }
			set
			{
				if (_resultMessageText != value)
				{
					_resultMessageText = value;
					OnPropertyChanged("ResultMessageText");
				}
			}
		}

		public Computer SelectedComputer
		{
			get { return _selectedComputer; }
			set
			{
				if (_selectedComputer != value)
				{
					_selectedComputer = value;
					if (_selectedComputer == null) _selectedComputer = _nullErrorComputer;

					_setDescriptionBox(_selectedComputer.Description);
					OnPropertyChanged("SelectedComputer");
				}
			}
		}

		public string SelectedDescription
		{
			get
			{
				if (_selectedDescription == null) return "";
				return _selectedDescription;
			}
			set
			{
				if(_selectedDescription != value)
				{
					_selectedDescription = value;
					OnPropertyChanged("SelectedDescription");
				}
			}
		}

		#endregion // Properties

		#region Methods

		private void _setDescriptionBox(string newDescription)
		{
			_selectedDescription = newDescription;
			OnPropertyChanged("SelectedDescription");
		}

		private void _setResultMessage(string message)
		{
			ResultMessageText = message;
		}

		public async void Search(string search)
		{
			if (_isSearching) return;
			if (string.IsNullOrEmpty(search) || search.Length < 3)
			{
				ResultMessageText = "Search does not meet minimum length requirement.";
				return;
			}

			await Task.Run(async () =>
			{
				_isSearching = true;
				var resultList = await _adService.GetComputersAsync(search, _setResultMessage);
				Application.Current.Dispatcher.Invoke(() =>
				{
					SearchResultList = resultList;
					if (SearchResultList != null && SearchResultList.Count > 0) SelectedComputer = SearchResultList[0];
					else SelectedComputer = _nullErrorComputer;
					_isSearching = false;
				});
			});
		}

		public async void SaveChangesAsync(Computer newDescription)
		{
			if (_isSaving) return;
			await Task.Run(async () =>
			{
				_isSaving = true;
				var tmpComp = await _adService.SetDescriptionAsync(_selectedComputer.DistinguishedName
					              , _selectedComputer.Name
					              , DescriptionPrefix.GetFullDescription(_selectedDescription
						              , _selectedComputer.DescPrefix)
					              , _setResultMessage) ?? newDescription;
				Application.Current.Dispatcher.Invoke(() =>
				{
					_computerSearchResults.Insert(_computerSearchResults.IndexOf(SelectedComputer), tmpComp);
					_computerSearchResults.Remove(SelectedComputer);

					SelectedComputer = _computerSearchResults[_computerSearchResults.IndexOf(tmpComp)];
					_isSaving = false;
				});
			});
		}

		#endregion // Methods
	}
}
