using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using System.Windows;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class MovieViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public MovieViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        private TraktMovie _movie;
        [SurviveTombstone]
        public TraktMovie Movie { get { return _movie; } set { _movie = value; updateDisplay(); } }

        private string _IMDBID;
        [SurviveTombstone]
        public string IMDBID { get { return _IMDBID; } set { _IMDBID = value; TraktAPI.TraktAPI.getMovie(IMDBID).Subscribe(onNext: movie => Movie = movie, onError: error => handleError(error)); TraktAPI.TraktAPI.getMovieShouts(IMDBID).Subscribe(onNext: shouts => Shouts = shouts, onError: error => handleError(error)); System.Diagnostics.Debug.WriteLine(IMDBID); } }

        private TraktShout[] _Shouts;
        [SurviveTombstone]
        public TraktShout[] Shouts { get { return _Shouts; } set { _Shouts = value; NotifyOfPropertyChange("Shouts"); } }

        public String Year
        {
            get
            {
                if (Movie == null)
                    return "";
                else
                    return "Year : " + Movie.Year;
            }
        }

        public String RunTime
        {
            get
            {
                if (Movie == null || (Movie != null && Movie.RunTime == 0))
                    return "";
                else
                    return Movie.RunTime + " min";
            }
        }

        public String Certification
        {
            get
            {
                if (Movie == null)
                    return "";
                return Movie.Certification;
            }
        }

        public string LoggedIn
        {
            get
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Logged in ? {0}", TraktSettings.LoggedIn));
                if (Movie != null && TraktSettings.LoggedIn)
                    return "Visible";
                else
                    return "Collapsed";
            }
        }

        public string TagLine
        {
            get
            {
                if (Movie == null || (Movie != null && string.IsNullOrEmpty(Movie.TagLine)))
                    return "";
                return string.Format("\"{0}\"", Movie.TagLine);
            }
        }

        public string LoveImage
        {
            get
            {
                if (Movie == null || Movie.Rating == null)
                    return "";

                if (Movie.Rating.CompareTo("hate") == 0)
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/love_f.png";
                else
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/love.png";
            }
        }

        public string HateImage
        {
            get
            {
                if (Movie == null || Movie.Rating == null)
                    return "";
                if (Movie.Rating.CompareTo("love") == 0)
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/hate_f.png";
                else
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/hate.png";
            }
        }

        public string UserRatingText
        {
            get
            {
                System.Diagnostics.Debug.WriteLine(Movie);
                if (Movie == null || Movie.Rating == null)
                    return "";
                System.Diagnostics.Debug.WriteLine(Movie.Rating);
                if (Movie.Rating.CompareTo("love") == 0)
                    return "Love it!";
                if (Movie.Rating.CompareTo("hate") == 0)
                    return "Lame";
                return "";
            }
        }

        public string RatingPercentage
        {
            get
            {
                if (Movie == null || Movie.Ratings == null)
                    return "";
                return Movie.Ratings.Percentage + "%";
            }
        }

        public string MovieTitleAndYear
        {
            get
            {
                if (Movie == null)
                    return "";
                return String.Format("{0} ({1})",Movie.Title,Movie.Year);
            }
        }

        public string RatingImage
        {
            get
            {
                if (Movie == null || Movie.Ratings == null)
                    return "";
                if (Movie.Ratings.Percentage >= 50)
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/iconLove.png";
                else
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/iconHate.png";
            }
        }

        public string RatingCount
        {
            get
            {
                if (Movie == null)
                    return "";
                return string.Format("{0} votes", Movie.Ratings.Votes);
            }

        }

        public string WatchedThis 
        { 
            get 
            {
                if (Movie == null)
                    return "";
                return Movie.Watched ? "You've watched This!" : "I've watched this"; 
            } 
        }

        public string WatchList
        {
            get
            {
                if (Movie == null)
                    return "";
                return Movie.OnWatchList ? "Remove from watchlist" : "Add to watchlist";
            }
        }

        public string showWatchList
        {
            get
            {
                if (Movie == null || Movie.Watched)
                    return "Collapsed";
                return "Visible";
            }
        }

        [SurviveTombstone]
        private TraktRatings ratings { set { Movie.Ratings = value; newRatings(); } }

        public void Love()
        {
            if (Movie.Rating.CompareTo("love") == 0)
            {
                TraktAPI.TraktAPI.rateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.unrate.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Movie.Rating = "";
            }
            else
            {
                TraktAPI.TraktAPI.rateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.love.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Movie.Rating = "love";
            }
            NotifyOfPropertyChange("LoveImage");
            NotifyOfPropertyChange("HateImage");
            NotifyOfPropertyChange("UserRatingText");
        }

        public void Hate()
        {
            if (Movie.Rating.CompareTo("hate") == 0)
            {
                TraktAPI.TraktAPI.rateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.unrate.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Movie.Rating = "";
            }
            else
            {
                TraktAPI.TraktAPI.rateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.hate.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Movie.Rating = "hate";
            }
            NotifyOfPropertyChange("LoveImage");
            NotifyOfPropertyChange("HateImage");
            NotifyOfPropertyChange("UserRatingText");
        }

        private void updateDisplay()
        {
            NotifyOfPropertyChange("LoggedIn");
            NotifyOfPropertyChange("Movie");
            NotifyOfPropertyChange("Year");
            NotifyOfPropertyChange("RunTime");
            NotifyOfPropertyChange("LoveImage");
            NotifyOfPropertyChange("HateImage");
            NotifyOfPropertyChange("RatingPercentage");
            NotifyOfPropertyChange("MovieTitleAndYear");
            NotifyOfPropertyChange("Certification");
            NotifyOfPropertyChange("UserRatingText");
            NotifyOfPropertyChange("RatingImage");
            NotifyOfPropertyChange("RatingCount");
            NotifyOfPropertyChange("TagLine");
            NotifyOfPropertyChange("WatchedThis");
            NotifyOfPropertyChange("WatchList");
            NotifyOfPropertyChange("showWatchList");
        }

        private void newRatings()
        {
            NotifyOfPropertyChange("RatingImage");
            NotifyOfPropertyChange("RatingPercentage");
            NotifyOfPropertyChange("RatingCount");
        }

        public void ToggleWatched()
        {
            if (Movie.Watched)
            {
                TraktAPI.TraktAPI.unwatchMovie(Movie.IMDBID, Movie.Title, Movie.Year);
                Movie.Watched = false;
            }
            else
            {
                TraktAPI.TraktAPI.watchMovie(Movie.IMDBID, Movie.Title, Movie.Year);
                Movie.Watched = true;
                Movie.OnWatchList = false;
                NotifyOfPropertyChange("WatchList");
            }
            NotifyOfPropertyChange("WatchedThis");
            NotifyOfPropertyChange("showWatchList");
        }

        public void ToggleWatchList()
        {
            if (Movie.OnWatchList)
            {
                TraktAPI.TraktAPI.unwatchListMovie(Movie.IMDBID, Movie.Title, Movie.Year);
                Movie.OnWatchList = false;
            }
            else
            {
                TraktAPI.TraktAPI.watchListMovie(Movie.IMDBID, Movie.Title, Movie.Year);
                Movie.OnWatchList = true;
            }
            NotifyOfPropertyChange("WatchList");
        }

        public void SendShout()
        {
            navigationService.Navigate(new Uri("/Views/Shout.xaml?ItemType=movie&IMDBID=" + IMDBID, UriKind.Relative));
        }
    }
}
