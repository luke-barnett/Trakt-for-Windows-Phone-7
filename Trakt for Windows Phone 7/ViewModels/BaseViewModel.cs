using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using Caliburn.Micro;
using System.Windows;
using TraktAPI;
using Microsoft.Phone.Reactive;
using TraktAPI.TraktModels;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class BaseViewModel : Screen
    {
        private IsolatedStorageSettings _userSettings;
        private bool _progressBarVisible;

        public BaseViewModel()
        {
            _userSettings = IsolatedStorageSettings.ApplicationSettings;
            InternetConnectionAvailable();
            ProgressBarVisible = true;
            LoadSettings();
            TryLogIn();
        }

        public string ApplicationName { get { return "Trakt 7"; } }

        public string FontFamily { get { return "/Trakt for Windows Phone 7;component/Fonts/Fonts.zip#Droid Sans"; } }

        public bool ProgressBarVisible { get { return _progressBarVisible; } set { _progressBarVisible = value; NotifyOfPropertyChange(() => ProgressBarVisible); NotifyOfPropertyChange(() => ProgressBarVisibility); } }

        public string ProgressBarVisibility { get { return ProgressBarVisible ? "Visible" : "Collapsed"; } }

        public void HandleError(Exception error)
        {
            Debug.WriteLine(string.Format("CAUGHT ERROR {0}", error.Message));
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

        public void LoadSettings()
        {
            if (_userSettings.Contains("TraktUsername"))
                TraktSettings.Username = _userSettings["TraktUsername"] as string;
            else
                TraktSettings.Username = String.Empty;

            if (_userSettings.Contains("TraktPassword"))
                TraktSettings.Username = _userSettings["TraktPassword"] as string;
            else
                TraktSettings.Username = String.Empty;
        }

        public void TryLogIn()
        {
            if(!String.IsNullOrEmpty(TraktSettings.Password) && !String.IsNullOrEmpty(TraktSettings.Username))
            {
                TraktAPI.TraktAPI.TestAccount(TraktSettings.Username, TraktSettings.Password).Subscribe(onNext: response => UpdateLogInSettings(true), onError: error => UpdateLogInSettings(false));
            }
            else
                UpdateLogInSettings(false);
        }


        private void UpdateLogInSettings(bool successfulLogIn)
        {
            if (successfulLogIn)
                TraktSettings.LoggedIn = true;
            else
            {
                TraktSettings.LoggedIn = false;
                TraktSettings.Password = String.Empty;
            }
            ProgressBarVisible = false;
        }
    }
}
