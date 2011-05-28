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
        public bool moviesLoaded = false;
        public bool showsLoaded = false;

        private void showImage_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            showCount++;
            if (showCount > 2)
            {
                showsProgressBar.IsIndeterminate = false;
                showsLoaded = true;
            }
        }

        private void movieImage_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            movieCount++;
            if (movieCount > 2)
            {
                moviesProgressBar.IsIndeterminate = false;
                moviesLoaded = true;
            }
        }

        private void toAnotherPage(object sender, System.EventArgs e)
        {
            moviesProgressBar.IsIndeterminate = false;
            showsProgressBar.IsIndeterminate = false;
        }
    }
}