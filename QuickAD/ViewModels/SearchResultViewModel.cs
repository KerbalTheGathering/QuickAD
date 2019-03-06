using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Input;
using QuickAD.Helper_Classes;
using QuickAD.Models;
using QuickAD.Services;

namespace QuickAD.ViewModels
{
    class SearchResultViewModel : ObservableObject, IPageViewModel
    {
		#region Fields

		private AdService _adService;
		private ICommand _resetPasswordCommand;
		private bool _shouldPassAutoExpire;
		private ObservableCollection<UserSearchResult> _userSearchResults;
        private UserSearchResult _selectedUser;
        private string _resultMessageText;

        #endregion // Fields

        public SearchResultViewModel(AdService adService)
        {
	        _adService = adService;
		}

        #region Properties / Commands

        public string Name
        {
            get { return "Search Results"; }
        }

		public SecureString SecurePassword { private get; set; }

		public SecureString ConfirmSecurePassword { private get; set; }

		public bool ShouldPassAutoExpire
		{
	        get { return _shouldPassAutoExpire; }
	        set
	        {
		        if (_shouldPassAutoExpire != value)
		        {
			        _shouldPassAutoExpire = value;
					OnPropertyChanged("ShouldPassAutoExpire");
		        }
	        }
        }

        public ICommand ResetPwCommand
        {
            get
            {
                if (_resetPasswordCommand == null)
                {
                    _resetPasswordCommand = new RelayCommand(
                        p => ResetPassword((UserSearchResult)p),
                        p => p is UserSearchResult);
                }

                return _resetPasswordCommand;
            }
        }

        public ObservableCollection<UserSearchResult> SearchResultList
        {
            get { return _userSearchResults; }
            set
            {
                if (_userSearchResults != value)
                {
                    _userSearchResults = value;
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

        public UserSearchResult SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged("SelectedUser");
                }
            }
        }

		#endregion // Parameters

		#region Methods

		public void ResetPassword(UserSearchResult user)
        {
            if (_checkSecurePasswords(SecurePassword, ConfirmSecurePassword))
            {
				_adService.ResetUserPassword(SelectedUser.UserName, SecurePassword, _shouldPassAutoExpire, SetResultMessage);
            }
            else
            {
				SetResultMessage("Passwords do not match.");
            }
        }

        public void SetResultMessage(string message)
        {
	        ResultMessageText = message;
        }

        private bool _checkSecurePasswords(SecureString newPassword, SecureString confirmPassword)
		{
			IntPtr newPass = IntPtr.Zero;
			IntPtr confirmPass = IntPtr.Zero;
			try
			{
				newPass = Marshal.SecureStringToBSTR(newPassword);
				confirmPass = Marshal.SecureStringToBSTR(confirmPassword);
				int length1 = Marshal.ReadInt32(newPass, -4);
				int length2 = Marshal.ReadInt32(confirmPass, -4);
				if (length1 == length2)
				{
					for (int x = 0; x < length1; ++x)
					{
						byte b1 = Marshal.ReadByte(newPass, x);
						byte b2 = Marshal.ReadByte(confirmPass, x);
						if (b1 != b2) return false;
					}
				}
				else return false;
				return true;
			}
			finally
			{
				if (confirmPass != IntPtr.Zero) Marshal.ZeroFreeBSTR(confirmPass);
				if (newPass != IntPtr.Zero) Marshal.ZeroFreeBSTR(newPass);
			}
		}

		#endregion // Methods
	}
}
