using System.Windows;

namespace QuickAD
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ApplicationView app = new ApplicationView();
            ViewModels.ApplicationViewModel context = new ViewModels.ApplicationViewModel();
            app.DataContext = context;
            app.Show();
        }
    }
}
