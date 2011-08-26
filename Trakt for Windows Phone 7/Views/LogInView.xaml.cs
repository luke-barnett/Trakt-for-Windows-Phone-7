using System.Windows;
using System.Windows.Controls;

namespace Trakt_for_Windows_Phone_7.Views
{
    public partial class LogInView : UserControl
    {
        public LogInView()
        {
            InitializeComponent();
        }

        private void UsernameChanged(object sender, TextChangedEventArgs e)
        {
            Username.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password.GetBindingExpression(PasswordBox.PasswordProperty).UpdateSource();
        }

        private void EmailChanged(object sender, TextChangedEventArgs e)
        {
            Email.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
