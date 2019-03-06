using System.Collections.Generic;
using System.Linq;
using QuickAD.Helper_Classes;
using System.Windows.Input;
using QuickAD.Services;

namespace QuickAD.ViewModels
{
    class ComputerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private ICommand _changeTabCommand;
        private List<IPageViewModel> _tabsViewModels;
        private IPageViewModel _currentTabViewModel;
		
        #endregion // Fields

        public ComputerViewModel(AdService adService)
        {
            TabsViewModels.Add(new SingleEditViewModel(adService));
            TabsViewModels.Add(new MultiEditViewModel(adService));
            CurrentTabViewModel = TabsViewModels[0];
        }

        #region Properties

        public string Name
        {
            get { return "Computers"; }
        }

        public ICommand ChangeTabCommand
        {
            get
            {
                if (_changeTabCommand == null)
                {
                    _changeTabCommand = new RelayCommand(
                        p => ChangeTabViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);
                }

                return _changeTabCommand;
            }
        }

        public List<IPageViewModel> TabsViewModels
        {
            get
            {
                if (_tabsViewModels == null)
                    _tabsViewModels = new List<IPageViewModel>();
                return _tabsViewModels;
            }
        }

        public IPageViewModel CurrentTabViewModel
        {
            get { return _currentTabViewModel; }
            set
            {
                if (_currentTabViewModel != value)
                {
                    _currentTabViewModel = value;
                    OnPropertyChanged("CurrentTabViewModel");
                }
            }
        }

        #endregion // Properties

        #region Methods

        private void ChangeTabViewModel(IPageViewModel viewModel)
        {
            if (!TabsViewModels.Contains(viewModel))
                TabsViewModels.Add(viewModel);

            CurrentTabViewModel = TabsViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        #endregion // Methods
    }
}
