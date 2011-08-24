using System;
using Caliburn.Micro;
using System.Windows;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class BaseViewModel : PropertyChangedBase
    {
        public BaseViewModel()
        {
            InternetConnectionAvailable();
        }

        public string ApplicationName { get { return "Trakt 7"; } }

        public string FontFamily { get { return "/Trakt for Windows Phone 7;component/Fonts/Fonts.zip#Droid Sans"; } }

        public void HandleError(Exception error)
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
