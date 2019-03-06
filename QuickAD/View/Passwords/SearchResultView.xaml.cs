using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace QuickAD.View.Passwords
{
    /// <summary>
    /// Interaction logic for SearchResultView.xaml
    /// </summary>
    public partial class SearchResultView : UserControl
    {
        public SearchResultView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                var secure = new SecureString();
                foreach (char c in ((PasswordBox) sender).Password)
                {
                    secure.AppendChar(c);
                }
                ((dynamic)this.DataContext).SecurePassword = secure;

            }
        }

        private void PasswordBox_ConfirmPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                var secure = new SecureString();
                foreach (char c in ((PasswordBox)sender).Password)
                {
                    secure.AppendChar(c);
                }
                ((dynamic)this.DataContext).ConfirmSecurePassword = secure;

            }
        }
    }
}
