using System.Windows.Controls;
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
        }
    }
}