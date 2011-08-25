using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using Caliburn.Micro;
using System.Windows;
using TraktAPI;
using Microsoft.Phone.Reactive;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class BaseViewModel : Screen
    {
        #region Private Parameters

        private readonly IsolatedStorageSettings _userSettings;
        private int _progressBarVisible;
        private bool _firedLoadingEvent;
        
        #endregion

        #region Events

        /// <summary>
        /// Event handler for finished loading event
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="eventArgs">The event arguments</param>
        public delegate void FinishedLoadingHandler(object sender, FinishedLoadingEventArgs eventArgs);

        /// <summary>
        /// Event fired only once initial loading has been completed
        /// </summary>
        public event FinishedLoadingHandler FinishedLoading = delegate {};

        #endregion

        /// <summary>
        /// The base view model that handles common tasks
        /// </summary>
        public BaseViewModel()
        {
            _userSettings = IsolatedStorageSettings.ApplicationSettings;
            InternetConnectionAvailable();
            ProgressBarVisible = true;
            LoadSettings();
            TryLogIn();
        }

        #region Public Parameters

        /// <summary>
        /// Returns the application's name
        /// </summary>
        public string ApplicationName { get { return "Trakt 7"; } }

        /// <summary>
        /// Returns the Font Family to use
        /// </summary>
        public string FontFamily { get { return "/Trakt for Windows Phone 7;component/Fonts/Fonts.zip#Droid Sans"; } }

        /// <summary>
        /// Whether or not to show the progress bar
        /// </summary>
        public bool ProgressBarVisible { get { return (_progressBarVisible > 0); } set { IncrementProgressBarVisibility(value); NotifyOfPropertyChange(() => ProgressBarVisible); NotifyOfPropertyChange(() => ProgressBarVisibility); } }

        /// <summary>
        /// The visibility state for the progress bar
        /// </summary>
        public Visibility ProgressBarVisibility { get { return ProgressBarVisible ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// The default poster
        /// </summary>
        public readonly ImageSource DefaultPoster = (ImageSource)new ImageSourceConverter().ConvertFromString(@"..\artwork\poster-small.jpg");

        #endregion

        #region Public Methods

        /// <summary>
        /// Records an error to Debug
        /// </summary>
        /// <param name="error">The error to record</param>
        public void HandleError(Exception error)
        {
            Debug.WriteLine(string.Format("CAUGHT ERROR {0}", error.Message));
        }

        /// <summary>
        /// Checks if there is an internet connection available
        /// </summary>
        /// <returns>The status of the internet connection</returns>
        public bool InternetConnectionAvailable()
        {
            var available = NetworkInterface.GetIsNetworkAvailable();
            Debug.WriteLine("Internet connection {0} available",(available)?"is":"is not");
            if (!available)
            {
                MessageBox.Show("No internet connection is available.  Try again later.", "Internet Required", MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Loads in the users settings from isolated storage
        /// </summary>
        public void LoadSettings()
        {
            Debug.WriteLine("Loading Settings");
            if (_userSettings.Contains("TraktUsername"))
                TraktSettings.Username = _userSettings["TraktUsername"] as string;
            else
                TraktSettings.Username = String.Empty;

            if (_userSettings.Contains("TraktPassword"))
                TraktSettings.Username = _userSettings["TraktPassword"] as string;
            else
                TraktSettings.Username = String.Empty;
            Debug.WriteLine("Loaded Values {0}", TraktSettings.ToString);
        }

        /// <summary>
        /// Saves the users settings from isolated storage
        /// </summary>
        public void SaveSettings()
        {
            _userSettings["TraktUsername"] = TraktSettings.Username;
            _userSettings["TraktPassword"] = TraktSettings.Password;
            _userSettings.Save();
            Debug.WriteLine("Saved Settings");
        }

        /// <summary>
        /// Attempts to log on to Trakt
        /// </summary>
        public void TryLogIn()
        {
            Debug.WriteLine("Try to log in");
            if(!String.IsNullOrEmpty(TraktSettings.Password) && !String.IsNullOrEmpty(TraktSettings.Username))
            {
                TraktAPI.TraktAPI.TestAccount(TraktSettings.Username, TraktSettings.Password).Subscribe(response => UpdateLogInSettings(true), error => UpdateLogInSettings(false));
            }
            else
                UpdateLogInSettings(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the log in status
        /// </summary>
        /// <param name="successfulLogIn">The status to update to</param>
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
            Debug.WriteLine("{0} to log in",(successfulLogIn)?"Managed":"Failed");
            OnFinishedLoading(new FinishedLoadingEventArgs(successfulLogIn));
        }

        /// <summary>
        /// Incremenets the progress bar visibility count
        /// </summary>
        /// <param name="increase">To increase or decrease</param>
        private void IncrementProgressBarVisibility(bool increase)
        {
            _progressBarVisible = (increase) ? _progressBarVisible + 1 : _progressBarVisible - 1;
            if (_progressBarVisible < 0)
                _progressBarVisible = 0;
        }

        /// <summary>
        /// Event trigger for when loading has finished
        /// </summary>
        protected void OnFinishedLoading(FinishedLoadingEventArgs args)
        {
            if (FinishedLoading == null || _firedLoadingEvent) return;
            Debug.WriteLine("Finished Loading, firing event");
            _firedLoadingEvent = true;
            FinishedLoading(this, args);
        }

        #endregion
    }

    public class FinishedLoadingEventArgs : EventArgs
    {
        public readonly bool StatusOfAccount;

        public FinishedLoadingEventArgs(bool statusOfAccount)
        {
            StatusOfAccount = statusOfAccount;
        }
    
    }
}
