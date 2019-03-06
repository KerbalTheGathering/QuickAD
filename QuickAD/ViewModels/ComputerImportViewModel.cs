using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QuickAD.Helper_Classes;
using QuickAD.Models;
using QuickAD.Services;

namespace QuickAD.ViewModels
{
	class ComputerImportViewModel: ObservableObject, IPageViewModel
	{
		#region Fields

		private readonly AdService _adService;
		private ICommand _importCommand;
		private string _fileLocation;
		private bool _isSetting;
		private bool _isImporting;
		private float _jobProgress;
		private string _resultMessageText;
		private ObservableCollection<BatchImport> _batchImports;
		private readonly ObservableCollection<BatchImport> _emptyBatchImports;

		#endregion // Fields

		public ComputerImportViewModel(AdService adService)
		{
			_adService = adService;
			_isSetting = false;
			_emptyBatchImports = new ObservableCollection<BatchImport>();
		}

		#region Properties

		public string Name
		{
			get { return "Computer Import"; }
		}

		public ObservableCollection<BatchImport> BatchImportList
		{
			get { return _batchImports; }
			set
			{
				if (_batchImports != value)
				{
					_batchImports = value;
					OnPropertyChanged("BatchImportList");
				}
			}
		}

		public ICommand ImportCommand
		{
			get
			{
				if (_importCommand == null)
				{
					_importCommand  = new RelayCommand(
						p => ImportAsync(),
						p => p is null);
				}

				return _importCommand;
			}
		}

		public float JobProgress
		{
			get { return _jobProgress; }
			set
			{
				if (_jobProgress != value)
				{
					_jobProgress = value;
					OnPropertyChanged("JobProgress");
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

		#endregion // Properties

		#region Methods

		public async void SetFile(string file)
		{
			if (_isSetting || string.IsNullOrEmpty(file)) return;
			BatchImportList = _emptyBatchImports;
			JobProgress = 0;
			await Task.Run(() =>
			{
				_isSetting = true;
				_fileLocation = file;
				var importList = Path.GetExtension(_fileLocation) == ".csv"
					? BatchImport.CreateBatchFromCsv(_fileLocation)
					: BatchImport.CreateBatchFromExcel(_fileLocation);

				Application.Current.Dispatcher.Invoke(() =>
				{
					try
					{
						//if (_batchImports == null)
						BatchImportList = new ObservableCollection<BatchImport>();
						foreach (var item in importList)
						{
							BatchImportList.Add(item);
						}

						_isSetting = false;
					}
					catch (NotSupportedException) { }

					_isSetting = false;
				});
			});

		}

		public async void ImportAsync()
		{
			if (_isImporting) return;

			JobProgress = 0;
			var completed = 0f;
			_isImporting = true;

			foreach (var import in _batchImports)
			{
				var desc = import.BatchDescription;
				await Task.Run(async () =>
				{
					foreach (var c in import.ComputerNames)
					{
						var computer = await _adService.GetComputersAsync(c, UpdateResultMessageText);
						if(computer.Count > 0)
							await _adService.SetDescriptionAsync(computer[0], desc, UpdateResultMessageText);
						Application.Current.Dispatcher.Invoke(() =>
						{
							completed++;
							JobProgress = 100 * (completed / GetNumJobs());
						});
					}
				});
			}

			_isImporting = false;
		}

		private void UpdateResultMessageText(string text)
		{
			ResultMessageText = text;
		}

		private float GetNumJobs()
		{
			var jobs = 0f;
			foreach (var import in _batchImports)
				jobs += import.BatchCount;

			return jobs;
		}

		#endregion // Methods
	}
}
