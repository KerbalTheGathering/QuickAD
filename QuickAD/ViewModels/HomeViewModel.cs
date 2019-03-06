using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using QuickAD.Helper_Classes;
using QuickAD.Services;
using System.Windows.Input;

namespace QuickAD.ViewModels
{
    class HomeViewModel : ObservableObject, IPageViewModel
    {
		#region Fields

		private readonly AdService _adService;
		private string _authenticationName;
		private string _defaultConnectionStatus;
		private string _staffConnectionStatus;
		private string _nonStaffConnectionStatus;
		private string _computerConnectionStatus;
		private Brush _defaultColor;
		private Brush _staffColor;
		private Brush _nonStaffColor;
		private Brush _computerColor;
		private ICommand _refreshCommand;

		#endregion // Fields

		public HomeViewModel(AdService adService)
		{
			_adService = adService;
			RefreshStatusText();
		}

		#region Properties

		public string Name
        {
	        get
	        {
		        return "Home";
	        }
        }

		public string AuthenticationName
		{
			get { return _authenticationName; }
			set
			{
				if (_authenticationName != value)
				{
					_authenticationName = value;
					OnPropertyChanged("AuthenticationName");
				}
			}
		}

		public string DefaultConnectionStatus
		{
			get { return _defaultConnectionStatus; }
			set
			{
				if (_defaultConnectionStatus != value)
				{
					_defaultConnectionStatus = value;
					OnPropertyChanged("DefaultConnectionStatus");
				}
			}
		}

		public string StaffConnectionStatus
		{
			get { return _staffConnectionStatus; }
			set
			{
				if (_staffConnectionStatus != value)
				{
					_staffConnectionStatus = value;
					OnPropertyChanged("StaffConnectionStatus");
				}
			}
		}

		public string NonStaffConnectionStatus
		{
			get { return _nonStaffConnectionStatus; }
			set
			{
				if (_nonStaffConnectionStatus != value)
				{
					_nonStaffConnectionStatus = value;
					OnPropertyChanged("NonStaffConnectionStatus");
				} 
			}
		}

		public string ComputerConnectionStatus
		{
			get { return _computerConnectionStatus; }
			set
			{
				if (_computerConnectionStatus != value)
				{
					_computerConnectionStatus = value;
					OnPropertyChanged("ComputerConnectionStatus");
				}
			}
		}

		public Brush DefaultColor
		{
			get { return _defaultColor; }
			set
			{
				if (_defaultColor != value)
				{
					_defaultColor = value;
					OnPropertyChanged("DefaultColor");
				}
			}
		}

		public Brush StaffColor
		{
			get { return _staffColor; }
			set
			{
				if (_staffColor != value)
				{
					_staffColor = value;
					OnPropertyChanged("StaffColor");
				}
			}
		}

		public Brush NonStaffColor
		{
			get { return _nonStaffColor; }
			set
			{
				if (_nonStaffColor != value)
				{
					_nonStaffColor = value;
					OnPropertyChanged("NonStaffColor");
				}
			}
		}

		public Brush ComputerColor
		{
			get { return _computerColor; }
			set
			{
				if (_computerColor != value)
				{
					_computerColor = value;
					OnPropertyChanged("ComputerColor");
				}
			}
		}

		public ICommand RefreshCommand
		{
			get
			{
				if (_refreshCommand == null)
				{
					_refreshCommand = new RelayCommand(
						p => Refresh(),
						p => p is null);
				}

				return _refreshCommand;
			}
		}

		#endregion // Properties

		#region Methods

		private void RefreshStatusText()
		{
			AuthenticationName = /*Environment.UserName;*/ System.Security.Principal.WindowsIdentity.GetCurrent().Name;
			SetAllStatusToPending();

			Task.Run(async () =>
			{
				var status = await _adService.ConnectionStatus(AdService.ConnectionType.Default);
				Application.Current.Dispatcher.Invoke(() =>
				{
					DefaultConnectionStatus = status ? "Success" : "Failed";
					DefaultColor = GetStatusColor(status);
				});
			});
			Task.Run(async () =>
			{
				var status = await _adService.ConnectionStatus(AdService.ConnectionType.Staff);
				Application.Current.Dispatcher.Invoke(() =>
				{
					StaffConnectionStatus = status ? "Success" : "Failed";
					StaffColor = GetStatusColor(status);
				});
			});
			Task.Run(async () =>
			{
				var status = await _adService.ConnectionStatus(AdService.ConnectionType.NonStaff);
				Application.Current.Dispatcher.Invoke(() =>
				{
					NonStaffConnectionStatus = status ? "Success" : "Failed";
					NonStaffColor = GetStatusColor(status);
				});
			});
			Task.Run(async () =>
			{
				var status = await _adService.ConnectionStatus(AdService.ConnectionType.Computers);
				Application.Current.Dispatcher.Invoke(() =>
				{
					ComputerConnectionStatus = status ? "Success" : "Failed";
					ComputerColor = GetStatusColor(status);
				});
			});

		}

		private Brush GetStatusColor(bool status)
		{
			return status ? Brushes.Green : Brushes.Red;
		}

		private void SetAllStatusToPending()
		{
			DefaultConnectionStatus 
				= StaffConnectionStatus
					= NonStaffConnectionStatus
						= ComputerConnectionStatus
							= "Pending";
			DefaultColor
				= StaffColor
					= NonStaffColor
						= ComputerColor
							= Brushes.CornflowerBlue;
		}

		private void Refresh()
		{
			RefreshStatusText();
		}

		#endregion // Methods
	}
}
