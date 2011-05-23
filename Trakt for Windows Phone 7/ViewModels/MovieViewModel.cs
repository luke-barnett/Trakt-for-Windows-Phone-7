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
        public TraktMovie Movie { get { return _movie; } set { _movie = value; updateDisplay(); } }

        private string _movieTitle;
        public string MovieTitle { get { return _movieTitle; } set { _movieTitle = value; TraktAPI.TraktAPI.getMovie(MovieTitle).Subscribe(movie => Movie = movie); System.Diagnostics.Debug.WriteLine(MovieTitle); } }

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

        public string ShowRating
        {
            get
            {
                if (TraktSettings.LoggedIn)
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
                if (Movie == null)
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
                if (Movie == null)
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
                if (Movie == null)
                    return "";
                if (Movie.Rating.CompareTo("love") == 0)
                    return "Love it!";
                if (Movie.Rating.CompareTo("hate") == 0)
                    return "Lame :(";
                return "";
            }
        }

        public string RatingPercentage
        {
            get
            {
                if (Movie == null)
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

        private TraktRatings ratings { set { Movie.Ratings = value; newRatings(); } }

        public void Love()
        {
            if (Movie.Rating.CompareTo("love") == 0)
            {
                TraktAPI.TraktAPI.rateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.unrate.ToString());//.Subscribe(response => ratings = response.Ratings);
                Movie.Rating = "";
            }
            else
            {
                TraktAPI.TraktAPI.rateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.love.ToString()).Subscribe(response => ratings = response.Ratings);
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
                TraktAPI.TraktAPI.rateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.hate.ToString());//.Subscribe(response => ratings = response.Ratings);
                Movie.Rating = "";
            }
            else
            {
                TraktAPI.TraktAPI.rateMovie(Movie.IMDBID, Movie.Title, Movie.Year, TraktRateTypes.hate.ToString()).Subscribe(response => ratings = response.Ratings);
                Movie.Rating = "hate";
            }
            NotifyOfPropertyChange("LoveImage");
            NotifyOfPropertyChange("HateImage");
            NotifyOfPropertyChange("UserRatingText");
        }

        private void updateDisplay()
        {
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
        }

        private void newRatings()
        {
            NotifyOfPropertyChange("RatingImage");
            NotifyOfPropertyChange("RatingPercentage");
            NotifyOfPropertyChange("RatingCount");
        }
    }
}
