﻿using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using Caliburn.Micro;
using System.Windows;
using Microsoft.Phone.Shell;
using TraktAPI;
using Microsoft.Phone.Reactive;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;
using Trakt_for_Windows_Phone_7.Framework;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class BaseViewModel : Screen
    {
        #region Private Parameters

        private readonly IsolatedStorageSettings _userSettings;
        private int _progressBarVisible;
        private bool _firedLoadingEvent;
        private ApplicationBar _applicationBar;
        
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
        public BaseViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container)
        {
            Container = container;
            NavigationService = navigationService;
            WindowManager = windowManager;
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

        #region Images

        /// <summary>
        /// The default poster
        /// </summary>
        public readonly ImageSource DefaultPoster = (ImageSource)new ImageSourceConverter().ConvertFromString(@"..\artwork\poster-small.jpg");

        /// <summary>
        /// The default screen
        /// </summary>
        public readonly ImageSource DefaultScreen = (ImageSource)new ImageSourceConverter().ConvertFromString(@"..\artwork\episode-screen.jpg");

        /// <summary>
        /// The love image
        /// </summary>
        public readonly ImageSource LoveImage = (ImageSource)new ImageSourceConverter().ConvertFromString(@"..\artwork\iconLove.png");

        /// <summary>
        /// The hate image
        /// </summary>
        public readonly ImageSource HateImage = (ImageSource)new ImageSourceConverter().ConvertFromString(@"..\artwork\iconHate.png");

        /// <summary>
        /// Non faded love image
        /// </summary>
        public readonly ImageSource LoveFullImage = (ImageSource)new ImageSourceConverter().ConvertFromString(@"..\artwork\love.png");

        /// <summary>
        /// Faded love image
        /// </summary>
        public readonly ImageSource LoveFadeImage = (ImageSource)new ImageSourceConverter().ConvertFromString(@"..\artwork\love_f.png");

        /// <summary>
        /// Non faded hate image
        /// </summary>
        public readonly ImageSource HateFullImage = (ImageSource)new ImageSourceConverter().ConvertFromString(@"..\artwork\hate.png");

        /// <summary>
        /// Faded hate image
        /// </summary>
        public readonly ImageSource HateFadeImage = (ImageSource)new ImageSourceConverter().ConvertFromString(@"..\artwork\hate_f.png");

        /// <summary>
        /// Watchlist button uri
        /// </summary>
        public readonly Uri WatchListButtonUri = new Uri(@"/artwork/watchlist.png", UriKind.Relative);

        /// <summary>
        /// Unwatchlist button uri
        /// </summary>
        public readonly Uri UnWatchListButtonUri = new Uri(@"/artwork/unwatchlist.png", UriKind.Relative);

        /// <summary>
        /// Seen button uri
        /// </summary>
        public readonly Uri SeenButtonUri = new Uri(@"/artwork/seen.png", UriKind.Relative);

        /// <summary>
        /// Unseen button uri
        /// </summary>
        public readonly Uri UnSeenButtonUri = new Uri(@"/artwork/unseen.png", UriKind.Relative);

        /// <summary>
        /// Shout button uri
        /// </summary>
        public readonly Uri ShoutButtonUri = new Uri(@"/artwork/shout.png", UriKind.Relative);

        #endregion

        /// <summary>
        /// The navigation service
        /// </summary>
        public readonly INavigationService NavigationService;

        /// <summary>
        /// The window manager
        /// </summary>
        public readonly IWindowManager WindowManager;

        /// <summary>
        /// The phone container
        /// </summary>
        public readonly PhoneContainer Container;

        /// <summary>
        /// Awesome string
        /// </summary>
        public string Awesome { get { return "Awesome!"; } }

        /// <summary>
        /// Lame string
        /// </summary>
        public string Lame { get { return "Lame sauce :("; } }

        /// <summary>
        /// The Application Bar
        /// </summary>
        public ApplicationBar ApplicationBar { get { return _applicationBar; } set { _applicationBar = value; NotifyOfPropertyChange(() => ApplicationBar); } }

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
                NavigationService.GoBack();
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
                TraktSettings.Password = _userSettings["TraktPassword"] as string;
            else
                TraktSettings.Password = String.Empty;
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
            if(!TraktSettings.LoggedIn && !String.IsNullOrEmpty(TraktSettings.Password) && !String.IsNullOrEmpty(TraktSettings.Username))
            {
                TraktAPI.TraktAPI.TestAccount(TraktSettings.Username, TraktSettings.Password).Subscribe(response => UpdateLogInSettings(true), error => UpdateLogInSettings(false));
            }
            else if(!TraktSettings.LoggedIn)
                UpdateLogInSettings(false);
            else
                UpdateLogInSettings(true);
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
            {
                TraktSettings.LoggedIn = true;
                (Container.GetInstance(typeof(MainPageViewModel), "MainPageViewModel") as MainPageViewModel).SetUpApplicationBar();
            }
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

    /// <summary>
    /// Event Arguments for when loading finishes
    /// </summary>
    public class FinishedLoadingEventArgs : EventArgs
    {
        public readonly bool StatusOfAccount;

        public FinishedLoadingEventArgs(bool statusOfAccount)
        {
            StatusOfAccount = statusOfAccount;
        }
    
    }
}
