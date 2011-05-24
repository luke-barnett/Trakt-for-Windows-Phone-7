namespace Trakt_for_Windows_Phone_7.Views
{
    using Microsoft.Phone.Controls;
    using System.Windows.Input;
    using System.Windows.Controls;

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void searchBox_KeyUp(object sender, KeyEventArgs e)
        {
            searchBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}