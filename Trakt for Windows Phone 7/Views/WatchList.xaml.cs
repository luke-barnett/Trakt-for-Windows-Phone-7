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
    public partial class WatchList : PhoneApplicationPage
    {
        public WatchList()
        {
            InitializeComponent();
        }

        private int imageCount;
        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            watchListProgressBar.IsIndeterminate = true;
            imageCount++;
            int requirement = 0;
            if (MovieListBox.ItemsSource != null)
            {
                requirement += MovieListBox.Items.Count;
            }
            if (ShowListBox.ItemsSource != null)
            {
                requirement += ShowListBox.Items.Count;
            }
            if (EpisodeListBox.ItemsSource != null)
            {
                requirement += EpisodeListBox.Items.Count;
            }

            System.Diagnostics.Debug.WriteLine(imageCount);
            System.Diagnostics.Debug.WriteLine(requirement);
            if (imageCount >= requirement)
                watchListProgressBar.IsIndeterminate = false;
        }
    }
}