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
    public partial class Search : PhoneApplicationPage
    {
        public Search()
        {
            InitializeComponent();
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            SearchBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (e.Key == Key.Enter)
                searchProgressBar.IsIndeterminate = true;
        }

        private int imageCount;
        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            imageCount++;
            if (MovieResultsListBox.ItemsSource != null && ShowResultsListBox.ItemsSource != null && EpisodeResultsListBox.ItemsSource != null)
            {
                if (imageCount >= (MovieResultsListBox.Items.Count + MovieResultsListBox.Items.Count + MovieResultsListBox.Items.Count))
                    searchProgressBar.IsIndeterminate = false;
            }
        }
    }
}