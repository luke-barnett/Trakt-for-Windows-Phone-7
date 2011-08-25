using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
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
        #endregion
    }
}