using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using TraktAPI;
using TraktAPI.TraktModels;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class MovieViewModel : BaseViewModel
    {
        #region Private Parameters

        private string _imdbid;
        private bool _showMainPivot;
        private ImageSource _moviePoster;
        private TraktMovie _movie;
        private List<TraktShout> _shouts;

        #endregion

        public MovieViewModel(INavigationService navigationService) : base(navigationService)
        {
            _shouts = new List<TraktShout>();
            MoviePoster = DefaultPoster;
        }

        #region Public Parameters

        /// <summary>
        /// The IMDBID of the movie
        /// </summary>
        public string IMDBID { get { return _imdbid; } set { _imdbid = value; NotifyOfPropertyChange(() => IMDBID); GetMovieDetails(); } }

        /// <summary>
        /// Whether or not to show the main pivot control
        /// </summary>
        public bool ShowMainPivot { get { return _showMainPivot; } set { _showMainPivot = value; NotifyOfPropertyChange(() => MainPivotVisibility); } }

        /// <summary>
        /// The visibility of the main pivot control
        /// </summary>
        public Visibility MainPivotVisibility { get { return (ShowMainPivot) ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// The movies poster
        /// </summary>
        public ImageSource MoviePoster { get { return _moviePoster; } set { _moviePoster = value; NotifyOfPropertyChange(() => MoviePoster); } }

        /// <summary>
        /// The movie object
        /// </summary>
        public TraktMovie Movie { get { return _movie; } set { _movie = value; UpdateDetails(); } }

        /// <summary>
        /// The visibility of the rate box
        /// </summary>
        public Visibility RateBoxVisibility { get { return (ShowMainPivot && TraktSettings.LoggedIn) ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// The list of shouts for the movie
        /// </summary>
        public List<TraktShout> Shouts { get { return _shouts; } set { _shouts = value; NotifyOfPropertyChange(() => Shouts); } }

        /// <summary>
        /// The visibility of the shouts panel
        /// </summary>
        public Visibility ShowShouts { get { return (Shouts.Count > 0) ? Visibility.Visible : Visibility.Collapsed; } }

        #region Details

        /// <summary>
        /// The title of the movie
        /// </summary>
        public string Title { get { return Movie.TitleAndYear; } }

        /// <summary>
        /// The tag line of the movie
        /// </summary>
        public string TagLine { get { return String.IsNullOrEmpty(Movie.TagLine) ? String.Empty : '"' + Movie.TagLine + '"'; } }

        /// <summary>
        /// The run time of the movie
        /// </summary>
        public string RunTime { get { return Movie.RunTime + " mins"; } }
         
        /// <summary>
        /// The overview for the movie
        /// </summary>
        public string Overview { get { return Movie.Overview; } }
        
        /// <summary>
        /// The certification for the movie
        /// </summary>
        public string Certification { get { return Movie.Certification; } }

        /// <summary>
        /// The rating image to show
        /// </summary>
        public ImageSource RatingImage { get { return (Movie != null) ? (Movie.Ratings.Percentage > 50) ?  LoveImage : HateImage : null; } }

        /// <summary>
        /// The rating percentage
        /// </summary>
        public string RatingPercentage { get { return (Movie != null) ? Movie.Ratings.Percentage + "%" : String.Empty; } }
        
        /// <summary>
        /// How many ratings the movie has
        /// </summary>
        public string RatingCount { get { return (Movie != null) ? Movie.Ratings.Votes + " votes" : String.Empty; } }

        /// <summary>
        /// The love image for the rate box
        /// </summary>
        public ImageSource LoveRateBoxImage { get { return (String.IsNullOrEmpty(Movie.Rating) || Movie.Rating == TraktRateTypes.love.ToString() || Movie.Rating.CompareTo("False") == 0) ? LoveFullImage : LoveFadeImage; } }

        /// <summary>
        /// The hate image for the rate box
        /// </summary>
        public ImageSource HateRateBoxImage { get { return (String.IsNullOrEmpty(Movie.Rating) || Movie.Rating == TraktRateTypes.hate.ToString() || Movie.Rating.CompareTo("False") == 0) ? HateFullImage : HateFadeImage; } }

        /// <summary>
        /// The users rating text for the movie
        /// </summary>
        public string UserRating { get { return Movie.Rating == TraktRateTypes.love.ToString() ? Awesome : (Movie.Rating == TraktRateTypes.hate.ToString() ? Lame : String.Empty); } }
        
        /// <summary>
        /// The opacity of the love image in the rate box
        /// </summary>
        public double LoveRateBoxOpacity { get { return (String.IsNullOrEmpty(Movie.Rating) || Movie.Rating == TraktRateTypes.love.ToString() || Movie.Rating.CompareTo("False") == 0) ? 1d : 0.5d; } }

        /// <summary>
        /// The opacity of the hate image in the rate box
        /// </summary>
        public double HateRateBoxOpacity { get { return (String.IsNullOrEmpty(Movie.Rating) || Movie.Rating == TraktRateTypes.hate.ToString() || Movie.Rating.CompareTo("False") == 0) ? 1d : 0.5d; } }

        /// <summary>
        /// Whether or not to show the watchlist button
        /// </summary>
        public bool ShowWatchListButton { get { return (TraktSettings.LoggedIn && !Movie.Watched && !Movie.InWatchList); } set { Movie.InWatchList = value; UpdateApplicationBar(); } }

        /// <summary>
        /// Whether or not to show the un watchlist button
        /// </summary>
        public bool ShowUnWatchListButton { get { return !ShowWatchListButton && !Movie.Watched; } }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieves the movies details from Trakt as well as shouts
        /// </summary>
        private void GetMovieDetails()
        {
            Debug.WriteLine("Getting movie details");
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetMovie(IMDBID).Subscribe(HandleMovie, HandleError);

            Debug.WriteLine("Getting movie shouts");
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetMovieShouts(IMDBID).Subscribe(HandleShouts, HandleError);
        }

        /// <summary>
        /// Handles updating the display once we get the movie details
        /// </summary>
        /// <param name="movie">The movie details</param>
        private void HandleMovie(TraktMovie movie)
        {
            Debug.WriteLine("Getting the poster");
            var poster = new BitmapImage(new Uri(movie.Images.Poster)){CreateOptions = BitmapCreateOptions.None};

            ProgressBarVisible = true;
            poster.ImageOpened += (sender, args) => { MoviePoster = poster; ProgressBarVisible = false; Debug.WriteLine("Got poster successfully"); };
            poster.ImageFailed += (sender, args) => { ProgressBarVisible = false; Debug.WriteLine("Failed to get poster"); };

            Movie = movie;

            ProgressBarVisible = false;
            ShowMainPivot = true;
        }

        /// <summary>
        /// Handles the updating of shouts
        /// </summary>
        /// <param name="shouts"></param>
        private void HandleShouts(TraktShout[] shouts)
        {
            Debug.WriteLine("Updating Shouts");
            Shouts.AddRange(shouts);
            Shouts = new List<TraktShout>(Shouts);
            ProgressBarVisible = false;
        }

        /// <summary>
        /// Updates the application bar
        /// </summary>
        private void UpdateApplicationBar()
        {
            Debug.WriteLine("Building Application Bar");
            var appBar = new ApplicationBar {IsVisible = TraktSettings.LoggedIn, Opacity = 1};

            if(ShowWatchListButton)
            {
                Debug.WriteLine("Adding Watchlist button");
                var watchListButton = new ApplicationBarIconButton(WatchListButtonUri)
                                          {Text = "Watchlist", IsEnabled = ShowWatchListButton};
                watchListButton.Click += (sender, args) => AddToWatchList();

                appBar.Buttons.Add(watchListButton);
            }

            if(ShowUnWatchListButton)
            {
                Debug.WriteLine("Adding Unwatchlist button");
                var unWatchListButton = new ApplicationBarIconButton(UnWatchListButtonUri)
                                            {Text = "UnWatchlist", IsEnabled = ShowUnWatchListButton};
                unWatchListButton.Click += (sender, args) => RemoveFromWatchList();

                appBar.Buttons.Add(unWatchListButton);
            }

            if(Movie.Watched)
            {
                Debug.WriteLine("Adding unwatch button");
                var unwatchButton = new ApplicationBarIconButton(SeenButtonUri) {Text = "Un-Watch", IsEnabled = Movie.Watched};
                unwatchButton.Click += (sender, args) => MarkAsUnWatched();

                appBar.Buttons.Add(unwatchButton);
            }
            else
            {
                Debug.WriteLine("Adding watch button");
                var watchButton = new ApplicationBarIconButton(SeenButtonUri) { Text = "Watch", IsEnabled = !Movie.Watched };
                watchButton.Click += (sender, args) => MarkAsWatched();

                appBar.Buttons.Add(watchButton);
            }

            Debug.WriteLine("Adding shout button");
            var shoutButton = new ApplicationBarIconButton(ShoutButtonUri)
                                  {Text = "Shout", IsEnabled = TraktSettings.LoggedIn};
            shoutButton.Click += (sender, args) => CreateShout();

            appBar.Buttons.Add(shoutButton);

            ApplicationBar = appBar;
        }

        /// <summary>
        /// Updates the movie details
        /// </summary>
        private void UpdateDetails()
        {
            Debug.WriteLine("Updating movie details");
            NotifyOfPropertyChange(() => Title);
            NotifyOfPropertyChange(() => TagLine);
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
            Movie.Ratings = ratings.Ratings;
            NotifyOfPropertyChange(() => RatingCount);
            NotifyOfPropertyChange(() => RatingImage);
            NotifyOfPropertyChange(() => RatingPercentage);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the clicking on the love image from the rate box
        /// </summary>
        public void Love()
        {
            if(Movie.Rating == TraktRateTypes.love.ToString())
            {
                Debug.WriteLine("Unrating movie");
                TraktAPI.TraktAPI.RateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.unrate.ToString()).Subscribe(UpdateRatings, HandleError);
                Movie.Rating = String.Empty;
            }
            else
            {
                Debug.WriteLine("Rating the movie as loved");
                TraktAPI.TraktAPI.RateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.love.ToString()).Subscribe(UpdateRatings, HandleError);
                Movie.Rating = TraktRateTypes.love.ToString();
            }
            UpdateRateBox();
        }

        /// <summary>
        /// Handles the clicking on the hate image from the rate box
        /// </summary>
        public void Hate()
        {
            if (Movie.Rating == TraktRateTypes.hate.ToString())
            {
                Debug.WriteLine("Unrating movie");
                TraktAPI.TraktAPI.RateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.unrate.ToString()).Subscribe(UpdateRatings, HandleError);
                Movie.Rating = String.Empty;
            }
            else
            {
                Debug.WriteLine("Rating the movie as unloved");
                TraktAPI.TraktAPI.RateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.hate.ToString()).Subscribe(UpdateRatings, HandleError);
                Movie.Rating = TraktRateTypes.hate.ToString();
            }
            UpdateRateBox();
        }

        /// <summary>
        /// Adds the movie to the users watchlist
        /// </summary>
        public void AddToWatchList()
        {
            Debug.WriteLine("Adding to watchlist");
            TraktAPI.TraktAPI.WatchListMovie(Movie.IMDBID, Movie.Title, Movie.Year);
            ShowWatchListButton = false;
        }

        /// <summary>
        /// Removes the movie from the users watchlist
        /// </summary>
        public void RemoveFromWatchList()
        {
            Debug.WriteLine("Removing from watchlist");
            TraktAPI.TraktAPI.UnwatchListMovie(Movie.IMDBID, Movie.Title, Movie.Year);
            ShowWatchListButton = true;
        }

        /// <summary>
        /// Marks the move as unwatched
        /// </summary>
        public void MarkAsUnWatched()
        {
            Debug.WriteLine("Marking as unwatched");
            TraktAPI.TraktAPI.UnwatchMovie(Movie.IMDBID, Movie.Title, Movie.Year);
            Movie.Watched = false;
            UpdateApplicationBar();
        }

        /// <summary>
        /// Marks the movie as watched
        /// </summary>
        public void MarkAsWatched()
        {
            Debug.WriteLine("Marking as watched");
            TraktAPI.TraktAPI.WatchMovie(Movie.IMDBID, Movie.Title, Movie.Year);
            Movie.Watched = true;
            Movie.InWatchList = false;
            UpdateApplicationBar();
        }

        /// <summary>
        /// Prompts the user for a shout
        /// </summary>
        public void CreateShout()
        {
            MessageBox.Show("Shout!");
        }

        #endregion
    }
}