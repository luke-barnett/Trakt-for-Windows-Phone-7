using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TraktAPI;
using Microsoft.Phone.Reactive;
using TraktAPI.TraktModels;
using Trakt_for_Windows_Phone_7.Models;
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
        public event FinishedLoadingHandler FinishedLoading = delegate { };

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
        /// Refresh button Uri
        /// </summary>
        public readonly Uri RefreshButtonUri = new Uri(@"/artwork/refresh.png", UriKind.Relative);

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
        public ApplicationBar ApplicationBar { get { return _applicationBar; } set { _applicationBar = value; foreach (var button in ApplicationWideApplicationBarMenuItems()) _applicationBar.MenuItems.Insert(0, button); NotifyOfPropertyChange(() => ApplicationBar); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Records an error to Debug
        /// </summary>
        /// <param name="error">The error to record</param>
        public void HandleError(Exception error)
        {
            Debug.WriteLine(string.Format("CAUGHT ERROR {0}", error.Message));
            MessageBox.Show("Oh dear! There seems to be an error :(");
            ProgressBarVisible = false;
        }

        /// <summary>
        /// Checks if there is an internet connection available
        /// </summary>
        /// <returns>The status of the internet connection</returns>
        public bool InternetConnectionAvailable()
        {
            var available = NetworkInterface.GetIsNetworkAvailable();
            Debug.WriteLine("Internet connection {0} available", (available) ? "is" : "is not");
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
            if (!TraktSettings.LoggedIn && !String.IsNullOrEmpty(TraktSettings.Password) && !String.IsNullOrEmpty(TraktSettings.Username))
            {
                TraktAPI.TraktAPI.TestAccount(TraktSettings.Username, TraktSettings.Password).Subscribe(response => UpdateLogInSettings(true), error => UpdateLogInSettings(false));
            }
            else if (!TraktSettings.LoggedIn)
                UpdateLogInSettings(false);
            else
                UpdateLogInSettings(true);
        }

        /// <summary>
        /// Creates a UI Element for a Show
        /// </summary>
        /// <param name="show">The show to create it for</param>
        /// <returns>The created UIElement</returns>
        public UIElement GenerateGeneralShowElement(TraktShow show)
        {
            var showGrid = new Grid { Margin = new Thickness(5, 10, 5, 10) };
            showGrid.ColumnDefinitions.Add(new ColumnDefinition());
            showGrid.ColumnDefinitions.Add(new ColumnDefinition());

            var showPoster = new Image { Source = Statics.PosterImageStore[show.Images.Poster], MaxWidth = 200 };

            Statics.PosterImageStore.PropertyChanged += (sender, args) =>
                                                            {
                                                                if (args.PropertyName != show.Images.Poster)
                                                                    return;
                                                                Debug.WriteLine("Updating {0} from image store", show.Images.Poster);
                                                                showPoster.Source = Statics.PosterImageStore[show.Images.Poster];
                                                            };

            Grid.SetColumn(showPoster, 0);
            showGrid.Children.Add(showPoster);

            var showDetails = new Grid { Margin = new Thickness(5, 0, 0, 0) };
            showDetails.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
            showDetails.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
            showDetails.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });

            var showTitle = new TextBlock { Text = show.Title, FontSize = 34, TextWrapping = TextWrapping.Wrap };
            Grid.SetRow(showTitle, 0);
            showDetails.Children.Add(showTitle);

            var showCeritification = new TextBlock { Text = show.Certification };
            Grid.SetRow(showCeritification, 1);
            showDetails.Children.Add(showCeritification);

            var showRunTime = new TextBlock { Text = show.RunTime + " mins" };
            Grid.SetRow(showRunTime, 2);
            showDetails.Children.Add(showRunTime);

            Grid.SetColumn(showDetails, 1);

            showGrid.Children.Add(showDetails);

            var gestureListener = GestureService.GetGestureListener(showGrid);
            gestureListener.DoubleTap += (o, e) =>
            {
                Debug.WriteLine("Navigating to show {0}", show.TitleAndYear);
                NavigationService.Navigate(new Uri("/Views/ShowView.xaml?TVDBID=" + show.TVDBID, UriKind.Relative));
            };

            return showGrid;
        }

        /// <summary>
        /// Creates a UI Element for a movie
        /// </summary>
        /// <param name="movie">The movie to create it for</param>
        /// <returns>The created UIElement</returns>
        public UIElement GenerateGeneralMovieElement(TraktMovie movie)
        {
            var movieGrid = new Grid { Margin = new Thickness(5, 10, 5, 10) };
            movieGrid.ColumnDefinitions.Add(new ColumnDefinition());
            movieGrid.ColumnDefinitions.Add(new ColumnDefinition());

            var moviePoster = new Image { Source = Statics.PosterImageStore[movie.Images.Poster], MaxWidth = 200 };

            Statics.PosterImageStore.PropertyChanged += (sender, args) =>
                                                            {
                                                                if (args.PropertyName != movie.Images.Poster)
                                                                    return;
                                                                Debug.WriteLine("Updating {0} from image store", movie.Images.Poster);
                                                                moviePoster.Source = Statics.PosterImageStore[movie.Images.Poster];
                                                            };

            Grid.SetColumn(moviePoster, 0);
            movieGrid.Children.Add(moviePoster);

            var movieDetails = new Grid { Margin = new Thickness(5, 0, 0, 0) };
            movieDetails.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
            movieDetails.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
            movieDetails.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });

            var movieTitle = new TextBlock { Text = movie.TitleAndYear, FontSize = 34, TextWrapping = TextWrapping.Wrap };
            Grid.SetRow(movieTitle, 0);
            movieDetails.Children.Add(movieTitle);

            var movieCertification = new TextBlock { Text = movie.Certification };
            Grid.SetRow(movieCertification, 1);
            movieDetails.Children.Add(movieCertification);

            var movieRunTime = new TextBlock { Text = movie.RunTime + " mins" };
            Grid.SetRow(movieRunTime, 2);
            movieDetails.Children.Add(movieRunTime);

            Grid.SetColumn(movieDetails, 1);

            movieGrid.Children.Add(movieDetails);

            var gestureListener = GestureService.GetGestureListener(movieGrid);
            gestureListener.DoubleTap += (o, e) =>
            {
                Debug.WriteLine("Navigating to movie {0}", movie.TitleAndYear);
                NavigationService.Navigate(new Uri("/Views/MovieView.xaml?IMDBID=" + movie.IMDBID, UriKind.Relative));
            };

            return movieGrid;
        }

        /// <summary>
        /// Creates a UI Element for a episode
        /// </summary>
        /// <param name="episode">The episode to create it for</param>
        /// <param name="tvdbid">The episodes show TVDBID</param>
        /// <returns>The created UIElement</returns>
        public UIElement GenerateGeneralEpisodeElement(TraktEpisode episode, string tvdbid)
        {
            var episodeGrid = new Grid { Margin = new Thickness(5, 10, 5, 10) };
            episodeGrid.ColumnDefinitions.Add(new ColumnDefinition());
            episodeGrid.ColumnDefinitions.Add(new ColumnDefinition());

            var episodeImage = new Image { Source = Statics.ScreenImageStore[episode.Images.Screen], MaxWidth = 200 };

            Statics.ScreenImageStore.PropertyChanged += (sender, args) =>
                                                            {
                                                                if (args.PropertyName != episode.Images.Screen)
                                                                    return;

                                                                Debug.WriteLine("Updating {0} from image store", episode.Images.Screen);
                                                                episodeImage.Source = Statics.ScreenImageStore[episode.Images.Screen];
                                                            };

            Grid.SetColumn(episodeImage, 0);
            episodeGrid.Children.Add(episodeImage);

            var episodeDetails = new Grid { Margin = new Thickness(5, 0, 0, 0) };
            episodeDetails.RowDefinitions.Add(new RowDefinition());
            episodeDetails.RowDefinitions.Add(new RowDefinition());

            var episodeTitle = new TextBlock { Text = episode.Title, FontSize = 28, TextWrapping = TextWrapping.Wrap };
            Grid.SetRow(episodeTitle, 0);

            episodeDetails.Children.Add(episodeTitle);

            var episodeValue = new TextBlock { Text = episode.CombinedSeasonAndEpisodeText, FontSize = 18 };
            Grid.SetRow(episodeValue, 1);

            episodeDetails.Children.Add(episodeValue);

            Grid.SetColumn(episodeDetails, 1);

            episodeGrid.Children.Add(episodeDetails);

            var gestureListener = GestureService.GetGestureListener(episodeGrid);
            gestureListener.DoubleTap += (o, e) =>
            {
                Debug.WriteLine("Navigating to episode {0}", episode.Episode);
                NavigationService.Navigate(new Uri("/Views/EpisodeView.xaml?TVDBID=" + tvdbid + "&Season=" + episode.Season + "&Episode=" + episode.Episode, UriKind.Relative));
            };

            return episodeGrid;
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
            Debug.WriteLine("{0} to log in", (successfulLogIn) ? "Managed" : "Failed");
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
        private void OnFinishedLoading(FinishedLoadingEventArgs args)
        {
            if (FinishedLoading == null || _firedLoadingEvent) return;
            Debug.WriteLine("Finished Loading, firing event");
            _firedLoadingEvent = true;
            FinishedLoading(this, args);
        }

        private IEnumerable<ApplicationBarMenuItem> ApplicationWideApplicationBarMenuItems()
        {
            var buttons = new List<ApplicationBarMenuItem>();

            if (TraktSettings.LoggedIn)
            {
                var recommendations = new ApplicationBarMenuItem { IsEnabled = true, Text = "Recommendations" };
                recommendations.Click += (o, e) =>
                                             {
                                                 Debug.WriteLine("Navigating to recommendations view");
                                                 NavigationService.Navigate(new Uri("/Views/RecommendationsView.xaml", UriKind.Relative));
                                             };
                buttons.Add(recommendations);

                var watchlist = new ApplicationBarMenuItem { IsEnabled = true, Text = "WatchList" };
                watchlist.Click += (o, e) =>
                                        {
                                            Debug.WriteLine("Navigating to watchlist view");
                                            NavigationService.Navigate(new Uri("/Views/WatchListView.xaml", UriKind.Relative));
                                        };
                buttons.Add(watchlist);
            }

            var search = new ApplicationBarMenuItem { IsEnabled = true, Text = "Search" };
            search.Click += (o, e) =>
                                {
                                    Debug.WriteLine("Navigating to search view");
                                    NavigationService.Navigate(new Uri("/Views/SearchView.xaml", UriKind.Relative));
                                };
            buttons.Add(search);

            return buttons;
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
