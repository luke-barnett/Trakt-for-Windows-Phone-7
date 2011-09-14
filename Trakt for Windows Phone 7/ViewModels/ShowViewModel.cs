using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Reactive;
using Caliburn.Micro;
using Microsoft.Phone.Shell;
using TraktAPI;
using TraktAPI.TraktModels;
using Trakt_for_Windows_Phone_7.Framework;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class ShowViewModel : BaseViewModel
    {
        #region Private Parameters

        private string _tvdbid;
        private bool _showMainPivot;
        private ImageSource _showPoster;
        private TraktShow _show;
        private List<TraktShout> _shouts;
        private List<UIElement> _seasons;
        private readonly ShoutViewModel _shoutViewModel;

        #endregion

        public ShowViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container, ShoutViewModel shoutViewModel) : base(navigationService, windowManager, container)
        {
            _shoutViewModel = shoutViewModel;
            _shoutViewModel.TVDBID = TVDBID;
            _shoutViewModel.LibraryType = TraktLibraryTypes.shows;
            _shouts = new List<TraktShout>();
            _seasons = new List<UIElement>();
            ShowPoster = DefaultPoster;
            ShowMainPivot = false;
        }

        #region Public Parameters

        public string TVDBID { get { return _tvdbid; } set { _tvdbid = value; NotifyOfPropertyChange(() => TVDBID); GetShowDetails(); } }

        public bool ShowMainPivot { get { return _showMainPivot; } set { _showMainPivot = value; NotifyOfPropertyChange(() => MainPivotVisibility); } }

        public Visibility MainPivotVisibility { get { return (ShowMainPivot) ? Visibility.Visible : Visibility.Collapsed; } }

        public ImageSource ShowPoster { get { return _showPoster; } set { _showPoster = value; NotifyOfPropertyChange(() => ShowPoster); } }

        public TraktShow Show { get { return _show; } set { _show = value; NotifyOfPropertyChange(() => Show); UpdateDetails(); } }

        /// <summary>
        /// The list of shouts for the show
        /// </summary>
        public List<TraktShout> Shouts { get { return _shouts; } set { _shouts = value; NotifyOfPropertyChange(() => Shouts); NotifyOfPropertyChange(() => ShowShouts); } }

        /// <summary>
        /// The visibility of the shouts panel
        /// </summary>
        public Visibility ShowShouts { get { return (Shouts.Count > 0) ? Visibility.Visible : Visibility.Collapsed; } }

        public List<UIElement> Seasons { get { return _seasons; } set { _seasons = value; NotifyOfPropertyChange(() => Seasons); NotifyOfPropertyChange(() => ShowSeasons); } }

        public Visibility ShowSeasons { get { return (Seasons.Count > 0) ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility RateBoxVisibility { get { return (ShowMainPivot && TraktSettings.LoggedIn) ? Visibility.Visible : Visibility.Collapsed; } }

        #region Details

        public string Title { get { return (Show == null) ? string.Empty : Show.Title; } }

        public string Network { get { return (Show == null) ? string.Empty : Show.Network; } }

        public string Certification { get { return (Show == null) ? string.Empty : Show.Certification; } }

        public string RunTime { get { return (Show == null) ? string.Empty : Show.RunTime + " mins"; } }

        public string Overview { get { return (Show == null) ? string.Empty : Show.Overview; } }

        public ImageSource RatingImage { get { return (Show != null) ? (Show.Ratings.Percentage > 50) ? LoveImage : HateImage : null; } }

        public string RatingPercentage { get { return (Show != null) ? Show.Ratings.Percentage + "%" : String.Empty; } }

        public string RatingCount { get { return (Show != null) ? Show.Ratings.Votes + " votes" : String.Empty; } }

        public ImageSource LoveRateBoxImage { get { return (String.IsNullOrEmpty(Show.Rating) || Show.Rating == TraktRateTypes.love.ToString() || Show.Rating.CompareTo("False") == 0) ? LoveFullImage : LoveFadeImage; } }

        /// <summary>
        /// The hate image for the rate box
        /// </summary>
        public ImageSource HateRateBoxImage { get { return (String.IsNullOrEmpty(Show.Rating) || Show.Rating == TraktRateTypes.hate.ToString() || Show.Rating.CompareTo("False") == 0) ? HateFullImage : HateFadeImage; } }

        /// <summary>
        /// The users rating text for the show
        /// </summary>
        public string UserRating { get { return Show.Rating == TraktRateTypes.love.ToString() ? Awesome : (Show.Rating == TraktRateTypes.hate.ToString() ? Lame : String.Empty); } }

        /// <summary>
        /// The opacity of the love image in the rate box
        /// </summary>
        public double LoveRateBoxOpacity { get { return (String.IsNullOrEmpty(Show.Rating) || Show.Rating == TraktRateTypes.love.ToString() || Show.Rating.CompareTo("False") == 0) ? 1d : 0.5d; } }

        /// <summary>
        /// The opacity of the hate image in the rate box
        /// </summary>
        public double HateRateBoxOpacity { get { return (String.IsNullOrEmpty(Show.Rating) || Show.Rating == TraktRateTypes.hate.ToString() || Show.Rating.CompareTo("False") == 0) ? 1d : 0.5d; } }

        /// <summary>
        /// Whether or not to show the watchlist button
        /// </summary>
        public bool ShowWatchListButton { get { return (TraktSettings.LoggedIn && !Show.InWatchList); } set { Show.InWatchList = value; UpdateApplicationBar(); } }

        /// <summary>
        /// Whether or not to show the un watchlist button
        /// </summary>
        public bool ShowUnWatchListButton { get { return !ShowWatchListButton; } }

        #endregion

        #endregion

        #region Private Methods

        private void GetShowDetails()
        {
            Debug.WriteLine("Getting show details");
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetShow(TVDBID).Subscribe(HandleShow, HandleError);

            GetShowSeasons();

            GetShowShouts();
        }

        private void HandleShow(TraktShow show)
        {
            Debug.WriteLine("Getting the poster");
            var poster = new BitmapImage(new Uri(show.Images.Poster)) {CreateOptions = BitmapCreateOptions.None};

            ProgressBarVisible = true;
            poster.ImageOpened += (sender, args) => { ShowPoster = poster; ProgressBarVisible = false; Debug.WriteLine("Got poster successfully"); };
            poster.ImageFailed += (sender, args) => { ProgressBarVisible = false; Debug.WriteLine("Failed to get poster"); };

            Show = show;

            ProgressBarVisible = false;
            ShowMainPivot = true;
        }

        /// <summary>
        /// Gets the shows shouts
        /// </summary>
        private void GetShowShouts()
        {
            Debug.WriteLine("Getting show shouts");
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetShowShouts(TVDBID).Subscribe(HandleShouts, HandleError);
        }

        /// <summary>
        /// Handles the updating of shouts
        /// </summary>
        /// <param name="shouts"></param>
        private void HandleShouts(TraktShout[] shouts)
        {
            Debug.WriteLine("Updating Shouts");
            Shouts.Clear();
            Shouts.AddRange(shouts);
            Shouts = new List<TraktShout>(Shouts);
            ProgressBarVisible = false;
        }

        private void GetShowSeasons()
        {
            Debug.WriteLine("Getting show seasons");
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetSeasonInfo(TVDBID).Subscribe(HandleSeasons, HandleError);
        }

        private void HandleSeasons(TraktSeasonInfo[] seasons)
        {
            Debug.WriteLine("Updating Seasons");
            Seasons.Clear();
            Seasons.AddRange((from season in seasons select GenerateSeasonUIElement(season)));
            Seasons = new List<UIElement>(Seasons);
            ProgressBarVisible = false;
        }

        private UIElement GenerateSeasonUIElement(TraktSeasonInfo season)
        {
            var textBlock = new TextBlock {Text = season.AsString, FontSize = 30};
            textBlock.MouseLeftButtonDown += (sender, args) =>
                                                 {
                                                     Debug.WriteLine("User selected season {0}", season.Season.ToString());
                                                     NavigationService.Navigate(new Uri("/Views/SeasonView.xaml?TVDBID=" + TVDBID + "&Season=" + season.Season, UriKind.Relative));
                                                 };
            return textBlock;
        }

        private void UpdateDetails()
        {
            Debug.WriteLine("Updating show details");
            NotifyOfPropertyChange(() => Title);
            NotifyOfPropertyChange(() => Network);
            NotifyOfPropertyChange(() => RunTime);
            NotifyOfPropertyChange(() => Overview);
            NotifyOfPropertyChange(() => Certification);
            NotifyOfPropertyChange(() => RatingImage);
            NotifyOfPropertyChange(() => RatingPercentage);
            NotifyOfPropertyChange(() => RatingCount);

            UpdateApplicationBar();
        }

        /// <summary>
        /// Updates the rate box
        /// </summary>
        private void UpdateRateBox()
        {
            Debug.WriteLine("Updating the rate box");
            NotifyOfPropertyChange(() => LoveRateBoxImage);
            NotifyOfPropertyChange(() => LoveRateBoxOpacity);
            NotifyOfPropertyChange(() => HateRateBoxImage);
            NotifyOfPropertyChange(() => HateRateBoxOpacity);
            NotifyOfPropertyChange(() => UserRating);
        }

        /// <summary>
        /// Updates the ratings
        /// </summary>
        /// <param name="ratings">The new ratings to use</param>
        private void UpdateRatings(TraktRateResponse ratings)
        {
            Debug.WriteLine("Updating the ratings");
            Show.Ratings = ratings.Ratings;
            NotifyOfPropertyChange(() => RatingCount);
            NotifyOfPropertyChange(() => RatingImage);
            NotifyOfPropertyChange(() => RatingPercentage);
        }

        /// <summary>
        /// Updates the application bar
        /// </summary>
        private void UpdateApplicationBar()
        {
            Debug.WriteLine("Building Application Bar");
            var appBar = new ApplicationBar { IsVisible = true, Opacity = 1 };

            if (TraktSettings.LoggedIn)
            {
                if (ShowWatchListButton)
                {
                    Debug.WriteLine("Adding Watchlist button");
                    var watchListButton = new ApplicationBarIconButton(WatchListButtonUri) { Text = "Watchlist", IsEnabled = ShowWatchListButton };
                    watchListButton.Click += (sender, args) => AddToWatchList();

                    appBar.Buttons.Add(watchListButton);
                }

                if (ShowUnWatchListButton)
                {
                    Debug.WriteLine("Adding Unwatchlist button");
                    var unWatchListButton = new ApplicationBarIconButton(UnWatchListButtonUri) { Text = "UnWatchlist", IsEnabled = ShowUnWatchListButton };
                    unWatchListButton.Click += (sender, args) => RemoveFromWatchList();

                    appBar.Buttons.Add(unWatchListButton);
                }

                Debug.WriteLine("Adding shout button");
                var shoutButton = new ApplicationBarIconButton(ShoutButtonUri) { Text = "Shout", IsEnabled = TraktSettings.LoggedIn };
                shoutButton.Click += (sender, args) => CreateShout();

                appBar.Buttons.Add(shoutButton);
            }

            Debug.WriteLine("Adding season refresh button");
            var seasonRefreshButton = new ApplicationBarMenuItem("Refresh Seasons") { IsEnabled = true };
            seasonRefreshButton.Click += (sender, args) => GetShowSeasons();

            appBar.MenuItems.Add(seasonRefreshButton);

            Debug.WriteLine("Adding shout refresh button");
            var shoutRefeshButton = new ApplicationBarMenuItem("Refresh Shouts") { IsEnabled = true };
            shoutRefeshButton.Click += (sender, args) => GetShowShouts();

            appBar.MenuItems.Add(shoutRefeshButton);

            ApplicationBar = appBar;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the clicking on the love image from the rate box
        /// </summary>
        public void Love()
        {
            if (Show.Rating == TraktRateTypes.love.ToString())
            {
                Debug.WriteLine("Unrating show");
                TraktAPI.TraktAPI.RateShow(Show.TVDBID, Show.IMDBID, Show.Title, Show.Year, TraktRateTypes.unrate.ToString()).Subscribe(UpdateRatings, HandleError);
                Show.Rating = String.Empty;
            }
            else
            {
                Debug.WriteLine("Rating the show as loved");
                TraktAPI.TraktAPI.RateShow(Show.TVDBID, Show.IMDBID, Show.Title, Show.Year, TraktRateTypes.love.ToString()).Subscribe(UpdateRatings, HandleError);
                Show.Rating = TraktRateTypes.love.ToString();
            }
            UpdateRateBox();
        }

        /// <summary>
        /// Handles the clicking on the hate image from the rate box
        /// </summary>
        public void Hate()
        {
            if (Show.Rating == TraktRateTypes.hate.ToString())
            {
                Debug.WriteLine("Unrating show");
                TraktAPI.TraktAPI.RateShow(Show.TVDBID, Show.IMDBID, Show.Title, Show.Year, TraktRateTypes.unrate.ToString()).Subscribe(UpdateRatings, HandleError);
                Show.Rating = String.Empty;
            }
            else
            {
                Debug.WriteLine("Rating the show as unloved");
                TraktAPI.TraktAPI.RateShow(Show.TVDBID, Show.IMDBID, Show.Title, Show.Year, TraktRateTypes.hate.ToString()).Subscribe(UpdateRatings, HandleError);
                Show.Rating = TraktRateTypes.hate.ToString();
            }
            UpdateRateBox();
        }

        /// <summary>
        /// Adds the show to the users watchlist
        /// </summary>
        public void AddToWatchList()
        {
            Debug.WriteLine("Adding to watchlist");
            TraktAPI.TraktAPI.WatchListShow(Show.TVDBID);
            ShowWatchListButton = false;
        }

        /// <summary>
        /// Removes the show from the users watchlist
        /// </summary>
        public void RemoveFromWatchList()
        {
            Debug.WriteLine("Removing from watchlist");
            TraktAPI.TraktAPI.UnwatchListShow(Show.TVDBID);
            ShowWatchListButton = true;
        }

        /// <summary>
        /// Prompts the user for a shout
        /// </summary>
        public void CreateShout()
        {
            _shoutViewModel.TVDBID = TVDBID;
            WindowManager.ShowDialog(_shoutViewModel);
        }
        #endregion
    }
}
