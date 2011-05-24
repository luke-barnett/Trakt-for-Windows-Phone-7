using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using Microsoft.Phone.Net.NetworkInformation;
using System.Windows;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class BaseViewModel : PropertyChangedBase
    {
        public BaseViewModel()
        {
            InternetConnectionAvailable();
        }

        public string ApplicationName { get { return "Trakt 7"; } }

        public void handleError(Exception error)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("CAUGHT ERROR {0}", error.Message));
        }

        public bool InternetConnectionAvailable()
        {
            var available = NetworkInterface.GetIsNetworkAvailable();

            if (!available)
            {
                MessageBox.Show("No internet connection is available.  Try again later.", "Internet Required", MessageBoxButton.OK);
                return false;
            }
            return true;
        }
    }
}
