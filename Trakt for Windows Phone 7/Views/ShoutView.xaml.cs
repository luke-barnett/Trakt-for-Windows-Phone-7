using System.Windows.Controls;

namespace Trakt_for_Windows_Phone_7.Views
{
    public partial class ShoutView : UserControl
    {
        public ShoutView()
        {
            InitializeComponent();
        }

        private void ShoutTextChanged(object sender, TextChangedEventArgs e)
        {
            ShoutText.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
