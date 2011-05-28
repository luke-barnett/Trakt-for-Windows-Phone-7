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

        private int showCount;
        private int movieCount;

        private void showImage_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            showCount++;
            if (showCount > 2)
                showsProgressBar.IsIndeterminate = false;
        }

        private void movieImage_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            movieCount++;
            if (movieCount > 2)
                moviesProgressBar.IsIndeterminate = false;
        }
    }
}