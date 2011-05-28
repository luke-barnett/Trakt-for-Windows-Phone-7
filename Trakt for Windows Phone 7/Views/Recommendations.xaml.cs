using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Trakt_for_Windows_Phone_7.Views
{
    public partial class Recommendations : PhoneApplicationPage
    {
        public Recommendations()
        {
            InitializeComponent();
        }

        private int imageCount;
        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            imageCount++;
            if (MoviesListBox.ItemsSource != null && ShowsListBox.ItemsSource != null)
            {
                if (imageCount >= (MoviesListBox.Items.Count + ShowsListBox.Items.Count))
                    recommendationsProgressBar.IsIndeterminate = false;
            }
        }
    }
}