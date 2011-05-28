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
    public partial class Shout : PhoneApplicationPage
    {
        public Shout()
        {
            InitializeComponent();
        }

        private void shoutBox_KeyUp(object sender, KeyEventArgs e)
        {
            shoutBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (e.Key == Key.Enter)
                shoutProgressBar.IsIndeterminate = true;
        }

        private void doShout_Click(object sender, RoutedEventArgs e)
        {
            shoutProgressBar.IsIndeterminate = true;
        }
    }
}