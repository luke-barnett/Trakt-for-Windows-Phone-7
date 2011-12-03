using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Microsoft.Phone.Shell;
using TraktAPI;
using TraktAPI.TraktModels;
using Trakt_for_Windows_Phone_7.Framework;
using Microsoft.Phone.Reactive;
using Trakt_for_Windows_Phone_7.Models;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class EpisodeViewModel : BaseViewModel
    {
        private string _tvdbid;
        private string _season;
        private string _episode;
        private bool _showMainPivot;
        private TraktEpisodeSummary _episodeSummary;
        private ImageSource _episodeImage;
        private List<TraktShout> _shouts;
        private readonly ShoutViewModel _shoutViewModel;

        public EpisodeViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container, ShoutViewModel shoutViewModel) : base(navigationService, windowManager, container)
        {
            _shoutViewModel = shoutViewModel;
            _shoutViewModel.LibraryType = TraktLibraryTypes.episodes;
            _shouts =  new List<TraktShout>();
            ShowMainPivot = false;
        }

        public string TVDBID { get { return _tvdbid; } set { _tvdbid = value; GetEpisode(); } }
        public string Season { get { return _season; } set { _season = value; GetEpisode(); } }
        public string Episode { get { return _episode; } set { _episode = value; GetEpisode(); } }

        public TraktEpisodeSummary EpisodeSummary { get { return _episodeSummary; } set { _episodeSummary = value; UpdateDetails(); } }

        public bool ShowMainPivot { get { return _showMainPivot; } set { _showMainPivot = value; NotifyOfPropertyChange(() => MainPivotVisibility); } }

        public Visibility MainPivotVisibility { get { return (ShowMainPivot) ? Visibility.Visible : Visibility.Collapsed; } }

        public ImageSource EpisodeImage { get { return _episodeImage; } set { _episodeImage = value; NotifyOfPropertyChange(() => EpisodeImage); } }

        /// <summary>
        /// The list of shouts for the movie
        /// </summary>
        public List<TraktShout> Shouts { get { return _shouts; } set { _shouts = value; NotifyOfPropertyChange(() => Shouts); NotifyOfPropertyChange(() => ShowShouts); } }

        /// <summary>
        /// The visibility of the shouts panel
        /// </summary>
        public Visibility ShowShouts { get { return (Shouts.Count > 0) ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// Whether or not to show the watchlist button
        /// </summary>
        public bool ShowWatchListButton { get { return (TraktSettings.LoginStatus.IsLoggedIn && !EpisodeSummary.Episode.Watched && !EpisodeSummary.Episode.InWatchList); } set { EpisodeSummary.Episode.InWatchList = value; UpdateApplicationBar(); } }

        /// <summary>
        /// Whether or not to show the un watchlist button
        /// </summary>
        public bool ShowUnWatchListButton { get { return !ShowWatchListButton && !EpisodeSummary.Episode.Watched; } }

        #region Details

        public string Title { get { return (EpisodeSummary == null || EpisodeSummary.Episode == null) ? String.Empty : EpisodeSummary.Episode.Title; } }

        public string EpisodeDetails { get { return (EpisodeSummary == null || EpisodeSummary.Episode == null) ? String.Empty : EpisodeSummary.Episode.CombinedSeasonAndEpisodeText; } }

        public string Overview { get { return (EpisodeSummary == null || EpisodeSummary.Episode == null) ? String.Empty : EpisodeSummary.Episode.Overview; } }

        public string Certification { get { return (EpisodeSummary == null || EpisodeSummary.Show == null) ? String.Empty : EpisodeSummary.Show.Certification; } }

        public string RunTime { get { return (EpisodeSummary == null || EpisodeSummary.Show == null) ? String.Empty : EpisodeSummary.Show.RunTime + " mins"; } }

        public ImageSource RatingImage { get { return (EpisodeSummary == null || EpisodeSummary.Episode == null) ? null : (EpisodeSummary.Episode.Ratings.Percentage > 50) ? LoveImage : HateImage; } }

        public string RatingPercentage { get { return (EpisodeSummary == null || EpisodeSummary.Episode == null) ? String.Empty : EpisodeSummary.Episode.Ratings.Percentage + "%"; } }

        public string RatingCount { get { return (EpisodeSummary == null || EpisodeSummary.Episode == null) ? String.Empty : EpisodeSummary.Episode.Ratings.Votes + " votes"; } }

        public Visibility RateBoxVisibility { get { return (ShowMainPivot && TraktSettings.LoginStatus.IsLoggedIn) ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// The love image for the rate box
        /// </summary>
        public ImageSource LoveRateBoxImage { get { return (String.IsNullOrEmpty(EpisodeSummary.Episode.Rating) || EpisodeSummary.Episode.Rating == TraktRateTypes.love.ToString() || EpisodeSummary.Episode.Rating.CompareTo("False") == 0) ? LoveFullImage : LoveFadeImage; } }

        /// <summary>
        /// The hate image for the rate box
        /// </summary>
        public ImageSource HateRateBoxImage { get { return (String.IsNullOrEmpty(EpisodeSummary.Episode.Rating) || EpisodeSummary.Episode.Rating == TraktRateTypes.hate.ToString() || EpisodeSummary.Episode.Rating.CompareTo("False") == 0) ? HateFullImage : HateFadeImage; } }

        /// <summary>
        /// The users rating text for the episode
        /// </summary>
        public string UserRating { get { return EpisodeSummary.Episode.Rating == TraktRateTypes.love.ToString() ? Awesome : (EpisodeSummary.Episode.Rating == TraktRateTypes.hate.ToString() ? Lame : String.Empty); } }

        /// <summary>
        /// The opacity of the love image in the rate box
        /// </summary>
        public double LoveRateBoxOpacity { get { return (String.IsNullOrEmpty(EpisodeSummary.Episode.Rating) || EpisodeSummary.Episode.Rating == TraktRateTypes.love.ToString() || EpisodeSummary.Episode.Rating.CompareTo("False") == 0) ? 1d : 0.5d; } }

        /// <summary>
        /// The opacity of the hate image in the rate box
        /// </summary>
        public double HateRateBoxOpacity { get { return (String.IsNullOrEmpty(EpisodeSummary.Episode.Rating) || EpisodeSummary.Episode.Rating == TraktRateTypes.hate.ToString() || EpisodeSummary.Episode.Rating.CompareTo("False") == 0) ? 1d : 0.5d; } }


        #endregion

        private void GetEpisode()
        {
            if (string.IsNullOrEmpty(TVDBID) || string.IsNullOrEmpty(Season) || string.IsNullOrEmpty(Episode))
                return;
            ProgressBarVisible = true;
            Debug.WriteLine("Getting Episode TVDBID {0} Season {1} Episode {2}", TVDBID, Season, Episode);
            TraktAPI.TraktAPI.GetEpisodeSummary(TVDBID, Season, Episode).Subscribe(HandleEpisode, HandleError);

            GetEpisodeShouts();
        }

        private void HandleEpisode(TraktEpisodeSummary episode)
        {
            Debug.WriteLine("Getting the episode image");

            EpisodeImage = Statics.ScreenImageStore[episode.Episode.Images.Screen];

            Statics.ScreenImageStore.PropertyChanged += (sender, args) =>
                                                            {
                                                                if (args.PropertyName != episode.Episode.Images.Screen)
                                                                    return;
                                                                Debug.WriteLine("Updating {0} from image store", episode.Episode.Images.Screen);
                                                                EpisodeImage = Statics.ScreenImageStore[episode.Episode.Images.Screen];
                                                            };

            EpisodeSummary = episode;

            ProgressBarVisible = false;
            ShowMainPivot = true;
        }

        private void GetEpisodeShouts()
        {
            Debug.WriteLine("Getting episode shouts");
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetEpisodeShouts(TVDBID, Season, Episode).Subscribe(HandleShouts, HandleError);
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

        private void UpdateDetails()
        {
            Debug.WriteLine("Updating movie details");
            NotifyOfPropertyChange(() => Title);
            NotifyOfPropertyChange(() => RunTime);
            NotifyOfPropertyChange(() => Overview);
            NotifyOfPropertyChange(() => Certification);
            NotifyOfPropertyChange(() => RatingImage);
            NotifyOfPropertyChange(() => RatingPercentage);
            NotifyOfPropertyChange(() => RatingCount);

            UpdateApplicationBar();
        }

        private void UpdateApplicationBar()
        {
            Debug.WriteLine("Building Application Bar");
            var appBar = new ApplicationBar { IsVisible = true, Opacity = 1 };

            if (TraktSettings.LoginStatus.IsLoggedIn)
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

                if (EpisodeSummary.Episode.Watched)
                {
                    Debug.WriteLine("Adding unwatch button");
                    var unwatchButton = new ApplicationBarIconButton(UnSeenButtonUri) { Text = "Un-Watch", IsEnabled = EpisodeSummary.Episode.Watched };
                    unwatchButton.Click += (sender, args) => MarkAsUnWatched();

                    appBar.Buttons.Add(unwatchButton);
                }
                else
                {
                    Debug.WriteLine("Adding watch button");
                    var watchButton = new ApplicationBarIconButton(SeenButtonUri) { Text = "Watch", IsEnabled = !EpisodeSummary.Episode.Watched };
                    watchButton.Click += (sender, args) => MarkAsWatched();

                    appBar.Buttons.Add(watchButton);
                }

                Debug.WriteLine("Adding shout button");
                var shoutButton = new ApplicationBarIconButton(ShoutButtonUri) { Text = "Shout", IsEnabled = TraktSettings.LoginStatus.IsLoggedIn };
                shoutButton.Click += (sender, args) => CreateShout();

                appBar.Buttons.Add(shoutButton);
            }

            Debug.WriteLine("Adding shout refresh button");
            var shoutRefeshButton = new ApplicationBarMenuItem("Refresh Shouts") { IsEnabled = true };
            shoutRefeshButton.Click += (sender, args) => GetEpisodeShouts();

            appBar.MenuItems.Add(shoutRefeshButton);

            ApplicationBar = appBar;
        }

        private void UpdateRateBox()
        {
            Debug.WriteLine("Updating the rate box");
            NotifyOfPropertyChange(() => LoveRateBoxImage);
            NotifyOfPropertyChange(() => LoveRateBoxOpacity);
            NotifyOfPropertyChange(() => HateRateBoxImage);
            NotifyOfPropertyChange(() => HateRateBoxOpacity);
            NotifyOfPropertyChange(() => UserRating);
        }

        private void UpdateRatings(TraktRateResponse ratings)
        {
            Debug.WriteLine("Updating the ratings");
            EpisodeSummary.Episode.Ratings = ratings.Ratings;
            NotifyOfPropertyChange(() => RatingCount);
            NotifyOfPropertyChange(() => RatingImage);
            NotifyOfPropertyChange(() => RatingPercentage);
        }

        #region Public Methods

        /// <summary>
        /// Handles the clicking on the love image from the rate box
        /// </summary>
        public void Love()
        {
            if (EpisodeSummary.Episode.Rating == TraktRateTypes.love.ToString())
            {
                Debug.WriteLine("Unrating episode");
                TraktAPI.TraktAPI.RateEpisode(EpisodeSummary.Show.TVDBID, EpisodeSummary.Show.IMDBID, EpisodeSummary.Show.Title, EpisodeSummary.Show.Year, EpisodeSummary.Episode.Season.ToString(), EpisodeSummary.Episode.Episode.ToString(), TraktRateTypes.unrate.ToString()).Subscribe(UpdateRatings, HandleError);
                EpisodeSummary.Episode.Rating = String.Empty;
            }
            else
            {
                Debug.WriteLine("Rating the episode as loved");
                TraktAPI.TraktAPI.RateEpisode(EpisodeSummary.Show.TVDBID, EpisodeSummary.Show.IMDBID, EpisodeSummary.Show.Title, EpisodeSummary.Show.Year, EpisodeSummary.Episode.Season.ToString(), EpisodeSummary.Episode.Episode.ToString(), TraktRateTypes.love.ToString()).Subscribe(UpdateRatings, HandleError);
                EpisodeSummary.Episode.Rating = TraktRateTypes.love.ToString();
            }
            UpdateRateBox();
        }

        /// <summary>
        /// Handles the clicking on the hate image from the rate box
        /// </summary>
        public void Hate()
        {
            if (EpisodeSummary.Episode.Rating == TraktRateTypes.hate.ToString())
            {
                Debug.WriteLine("Unrating episode");
                TraktAPI.TraktAPI.RateEpisode(EpisodeSummary.Show.TVDBID, EpisodeSummary.Show.IMDBID, EpisodeSummary.Show.Title, EpisodeSummary.Show.Year, EpisodeSummary.Episode.Season.ToString(), EpisodeSummary.Episode.Episode.ToString(), TraktRateTypes.unrate.ToString()).Subscribe(UpdateRatings, HandleError);
                EpisodeSummary.Episode.Rating = String.Empty;
            }
            else
            {
                Debug.WriteLine("Rating the episode as unloved");
                TraktAPI.TraktAPI.RateEpisode(EpisodeSummary.Show.TVDBID, EpisodeSummary.Show.IMDBID, EpisodeSummary.Show.Title, EpisodeSummary.Show.Year, EpisodeSummary.Episode.Season.ToString(), EpisodeSummary.Episode.Episode.ToString(), TraktRateTypes.hate.ToString()).Subscribe(UpdateRatings, HandleError);
                EpisodeSummary.Episode.Rating = TraktRateTypes.hate.ToString();
            }
            UpdateRateBox();
        }

        /// <summary>
        /// Adds the episode to the users watchlist
        /// </summary>
        public void AddToWatchList()
        {
            Debug.WriteLine("Adding to watchlist");
            TraktAPI.TraktAPI.WatchListEpisode(EpisodeSummary.Show.TVDBID, EpisodeSummary.Show.IMDBID, EpisodeSummary.Show.Title, EpisodeSummary.Show.Year, EpisodeSummary.Episode.Season.ToString(), EpisodeSummary.Episode.Episode.ToString());
            ShowWatchListButton = true;
        }

        /// <summary>
        /// Removes the episode from the users watchlist
        /// </summary>
        public void RemoveFromWatchList()
        {
            Debug.WriteLine("Removing from watchlist");
            TraktAPI.TraktAPI.UnwatchListEpisode(EpisodeSummary.Show.TVDBID, EpisodeSummary.Show.IMDBID, EpisodeSummary.Show.Title, EpisodeSummary.Show.Year, EpisodeSummary.Episode.Season.ToString(), EpisodeSummary.Episode.Episode.ToString());
            ShowWatchListButton = false;
        }

        /// <summary>
        /// Marks the episode as unwatched
        /// </summary>
        public void MarkAsUnWatched()
        {
            Debug.WriteLine("Marking as unwatched");
            TraktAPI.TraktAPI.WatchEpisode(EpisodeSummary.Show.TVDBID, EpisodeSummary.Show.IMDBID, EpisodeSummary.Show.Title, EpisodeSummary.Show.Year, EpisodeSummary.Episode.Season.ToString(), EpisodeSummary.Episode.Episode.ToString());
            EpisodeSummary.Episode.Watched = false;
            UpdateApplicationBar();
        }

        /// <summary>
        /// Marks the episode as watched
        /// </summary>
        public void MarkAsWatched()
        {
            Debug.WriteLine("Marking as watched");
            TraktAPI.TraktAPI.UnwatchEpisode(EpisodeSummary.Show.TVDBID, EpisodeSummary.Show.IMDBID, EpisodeSummary.Show.Title, EpisodeSummary.Show.Year, EpisodeSummary.Episode.Season.ToString(), EpisodeSummary.Episode.Episode.ToString());
            EpisodeSummary.Episode.Watched = true;
            EpisodeSummary.Episode.InWatchList = false;
            UpdateApplicationBar();
        }

        /// <summary>
        /// Prompts the user for a shout
        /// </summary>
        public void CreateShout()
        {
            _shoutViewModel.TVDBID = TVDBID;
            _shoutViewModel.Season = int.Parse(Season);
            _shoutViewModel.Episode = int.Parse(Episode);
            WindowManager.ShowDialog(_shoutViewModel);
        }

        #endregion
    }
}
