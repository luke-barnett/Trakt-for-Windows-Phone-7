using System;
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

        #endregion

        public MovieViewModel(INavigationService navigationService) : base(navigationService)
        {
            MoviePoster = DefaultPoster;
        }

        #region Public Parameters

        public string IMDBID { get { return _imdbid; } set { _imdbid = value; NotifyOfPropertyChange(() => IMDBID); GetMovieDetails(); } }

        public bool ShowMainPivot { get { return _showMainPivot; } set { _showMainPivot = value; NotifyOfPropertyChange(() => MainPivotVisibility); } }

        public Visibility MainPivotVisibility { get { return (ShowMainPivot) ? Visibility.Visible : Visibility.Collapsed; } }

        public ImageSource MoviePoster { get { return _moviePoster; } set { _moviePoster = value; NotifyOfPropertyChange(() => MoviePoster); } }

        public TraktMovie Movie { get { return _movie; } set { _movie = value; UpdateDetails(); } }

        public Visibility RateBoxVisibility { get { return (ShowMainPivot && TraktSettings.LoggedIn) ? Visibility.Visible : Visibility.Collapsed; } }

        #region Details

        public string Title { get { return Movie.TitleAndYear; } }

        public string TagLine { get { return String.IsNullOrEmpty(Movie.TagLine) ? String.Empty : '"' + Movie.TagLine + '"'; } }

        public string RunTime { get { return Movie.RunTime + " mins"; } }

        public string Overview { get { return Movie.Overview; } }

        public string Certification { get { return Movie.Certification; } }

        public ImageSource RatingImage { get { return (Movie != null) ? (Movie.Ratings.Percentage > 50) ?  LoveImage : HateImage : null; } }

        public string RatingPercentage { get { return (Movie != null) ? Movie.Ratings.Percentage + "%" : String.Empty; } }

        public string RatingCount { get { return (Movie != null) ? Movie.Ratings.Votes + " votes" : String.Empty; } }

        public ImageSource LoveRateBoxImage { get { return (String.IsNullOrEmpty(Movie.Rating) || Movie.Rating == TraktRateTypes.love.ToString() || Movie.Rating.CompareTo("False") == 0) ? LoveFullImage : LoveFadeImage; } }

        public ImageSource HateRateBoxImage { get { return (String.IsNullOrEmpty(Movie.Rating) || Movie.Rating == TraktRateTypes.hate.ToString() || Movie.Rating.CompareTo("False") == 0) ? HateFullImage : HateFadeImage; } }

        public string UserRating { get { return Movie.Rating == TraktRateTypes.love.ToString() ? Awesome : (Movie.Rating == TraktRateTypes.hate.ToString() ? Lame : String.Empty); } }

        public double LoveRateBoxOpacity { get { return (String.IsNullOrEmpty(Movie.Rating) || Movie.Rating == TraktRateTypes.love.ToString() || Movie.Rating.CompareTo("False") == 0) ? 1d : 0.5d; } }

        public double HateRateBoxOpacity { get { return (String.IsNullOrEmpty(Movie.Rating) || Movie.Rating == TraktRateTypes.hate.ToString() || Movie.Rating.CompareTo("False") == 0) ? 1d : 0.5d; } }

        public bool ShowWatchListButton { get { return (TraktSettings.LoggedIn && !Movie.Watched && !Movie.InWatchList); } set { Movie.InWatchList = value; UpdateApplicationBar(); } }

        public bool ShowUnWatchListButton { get { return !ShowWatchListButton && !Movie.Watched; } }

        #endregion

        #endregion

        #region Private Methods

        private void GetMovieDetails()
        {
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetMovie(IMDBID).Subscribe(HandleMovie, HandleError);
        }

        private void HandleMovie(TraktMovie movie)
        {
            var poster = new BitmapImage(new Uri(movie.Images.Poster)){CreateOptions = BitmapCreateOptions.None};
            ProgressBarVisible = true;
            poster.ImageOpened += (sender, args) => { MoviePoster = poster; ProgressBarVisible = false; };
            poster.ImageFailed += (sender, args) => { ProgressBarVisible = false; };

            Movie = movie;

            ProgressBarVisible = false;
            ShowMainPivot = true;
        }

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

        private void UpdateDetails()
        {
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

        private void UpdateRateBox()
        {
            NotifyOfPropertyChange(() => LoveRateBoxImage);
            NotifyOfPropertyChange(() => LoveRateBoxOpacity);
            NotifyOfPropertyChange(() => HateRateBoxImage);
            NotifyOfPropertyChange(() => HateRateBoxOpacity);
            NotifyOfPropertyChange(() => UserRating);
        }

        private void UpdateRatings(TraktRateResponse ratings)
        {
            NotifyOfPropertyChange(() => RatingCount);
            NotifyOfPropertyChange(() => RatingImage);
            NotifyOfPropertyChange(() => RatingPercentage);
        }

        #endregion

        #region Public Methods

        public void Love()
        {
            if(Movie.Rating == TraktRateTypes.love.ToString())
            {
                TraktAPI.TraktAPI.RateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.unrate.ToString()).Subscribe(UpdateRatings, HandleError);
                Movie.Rating = String.Empty;
            }
            else
            {
                TraktAPI.TraktAPI.RateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.love.ToString()).Subscribe(UpdateRatings, HandleError);
                Movie.Rating = TraktRateTypes.love.ToString();
            }
            UpdateRateBox();
        }

        public void Hate()
        {
            if (Movie.Rating == TraktRateTypes.hate.ToString())
            {
                TraktAPI.TraktAPI.RateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.unrate.ToString()).Subscribe(UpdateRatings, HandleError);
                Movie.Rating = String.Empty;
            }
            else
            {
                TraktAPI.TraktAPI.RateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.hate.ToString()).Subscribe(UpdateRatings, HandleError);
                Movie.Rating = TraktRateTypes.hate.ToString();
            }
            UpdateRateBox();
        }

        public void AddToWatchList()
        {
            TraktAPI.TraktAPI.WatchListMovie(Movie.IMDBID, Movie.Title, Movie.Year);
            ShowWatchListButton = false;
        }

        public void RemoveFromWatchList()
        {
            TraktAPI.TraktAPI.UnwatchListMovie(Movie.IMDBID, Movie.Title, Movie.Year);
            ShowWatchListButton = true;
        }

        public void MarkAsUnWatched()
        {
            TraktAPI.TraktAPI.UnwatchMovie(Movie.IMDBID, Movie.Title, Movie.Year);
            Movie.Watched = false;
            UpdateApplicationBar();
        }

        public void MarkAsWatched()
        {
            TraktAPI.TraktAPI.WatchMovie(Movie.IMDBID, Movie.Title, Movie.Year);
            Movie.Watched = true;
            Movie.InWatchList = false;
            UpdateApplicationBar();
        }

        public void CreateShout()
        {
            MessageBox.Show("Shout!");
        }

        #endregion
    }
}