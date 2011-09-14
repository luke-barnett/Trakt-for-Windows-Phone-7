using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace Trakt_for_Windows_Phone_7.Views
{
    public partial class SearchView : PhoneApplicationPage
    {
        public SearchView()
        {
            InitializeComponent();
        }

        private void SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            SearchBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (e.Key == Key.Enter)
                Focus();
        }
    }
}