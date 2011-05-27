using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using System.Windows;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class ShowViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public ShowViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }


        private string _TVDBID;
        [SurviveTombstone]
        public string TVDBID { get { return _TVDBID; } set { _TVDBID = value; NotifyOfPropertyChange("TVDBID"); TraktAPI.TraktAPI.getShow(TVDBID).Subscribe(onNext: response => Show = response, onError: error => handleError(error)); TraktAPI.TraktAPI.getSeasonInfo(TVDBID).Subscribe(onNext: response => Seasons = new List<TraktSeasonInfo>(response), onError: error => handleError(error)); TraktAPI.TraktAPI.getShowShouts(TVDBID).Subscribe(onNext: shouts => Shouts = shouts, onError: error => handleError(error)); } }

        
        private TraktShow _show;
        [SurviveTombstone]
        public TraktShow Show { get { return _show; } set { _show = value; updateDisplay(); } }

        private TraktShout[] _Shouts;
        [SurviveTombstone]
        public TraktShout[] Shouts { get { return _Shouts; } set { _Shouts = value; NotifyOfPropertyChange("Shouts"); } }

        public string ShowTitle 
        { 
            get 
            {
                if (Show == null)
                    return "";

                return Show.Title;
            }
        }

        public string RunTime
        {
            get
            {
                if (Show == null)
                    return "";
                return String.Format("{0} min", Show.RunTime);
            }
        }

        public string AirTime
        {
            get
            {
                if (Show == null)
                    return "";
                return String.Format("{0} at {1}", Show.AirDay, Show.AirTime);
            }
        }

        public string Certification
        {
            get
            {
                if (Show == null)
                    return "";

                return Show.Certification;
            }
        }

        public string Country
        {
            get
            {
                if (Show == null)
                    return "";

                return Show.Country;
            }
        }

        public string RatingImage
        {
            get
            {
                if (Show == null || Show.Ratings == null)
                    return "";
                if (Show.Ratings.Percentage >= 50)
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/iconLove.png";
                else
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/iconHate.png";
            }
        }

        public string RatingPercentage
        {
            get
            {
                if (Show == null)
                    return "";
                return Show.Ratings.Percentage + "%";
            }
        }

        public string RatingCount
        {
            get
            {
                if (Show == null)
                    return "";
                return string.Format("{0} votes", Show.Ratings.Votes);
            }

        }

        public string LoggedIn
        {
            get
            {
                if (Show != null && TraktSettings.LoggedIn)
                    return "Visible";
                else
                    return "Collapsed";
            }
        }

        public string UserRatingText
        {
            get
            {
                if (Show == null || Show.Rating == null)
                    return "";
                if (Show.Rating.CompareTo("love") == 0)
                    return "Love it!";
                if (Show.Rating.CompareTo("hate") == 0)
                    return "Lame";
                return "";
            }
        }

        public string LoveImage
        {
            get
            {
                if (Show == null || Show.Rating == null)
                    return "";

                if (Show.Rating.CompareTo("hate") == 0)
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/love_f.png";
                else
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/love.png";
            }
        }

        public string HateImage
        {
            get
            {
                if (Show == null || Show.Rating == null)
                    return "";
                if (Show.Rating.CompareTo("love") == 0)
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/hate_f.png";
                else
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/hate.png";
            }
        }

        private TraktRatings ratings { set { Show.Ratings = value; newRatings(); } }

        public void Love()
        {
            if (Show.Rating.CompareTo("love") == 0)
            {
                TraktAPI.TraktAPI.rateShow(Show.TVDBID, Show.IMDBID, Show.Title, Show.Year, TraktRateTypes.unrate.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Show.Rating = "";
            }
            else
            {
                TraktAPI.TraktAPI.rateShow(Show.TVDBID, Show.IMDBID, Show.Title, Show.Year, TraktRateTypes.love.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Show.Rating = "love";
            }
            NotifyOfPropertyChange("LoveImage");
            NotifyOfPropertyChange("HateImage");
            NotifyOfPropertyChange("UserRatingText");
        }

        public void Hate()
        {
            if (Show.Rating.CompareTo("hate") == 0)
            {
                TraktAPI.TraktAPI.rateShow(Show.TVDBID, Show.IMDBID, Show.Title, Show.Year, TraktRateTypes.unrate.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Show.Rating = "";
            }
            else
            {
                TraktAPI.TraktAPI.rateShow(Show.TVDBID, Show.IMDBID, Show.Title, Show.Year, TraktRateTypes.hate.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Show.Rating = "hate";
            }
            NotifyOfPropertyChange("LoveImage");
            NotifyOfPropertyChange("HateImage");
            NotifyOfPropertyChange("UserRatingText");
        }

        private List<TraktSeasonInfo> _seasons;
        [SurviveTombstone]
        public List<TraktSeasonInfo> Seasons { get { return _seasons; } set { _seasons = value; if(Show != null) NotifyOfPropertyChange("Seasons"); } }

        public TraktSeasonInfo SelectedSeason { get; set; }

        public void ViewSeason()
        {
            if (SelectedSeason != null)
            {
                navigationService.Navigate(new Uri("/Views/Season.xaml?TVDBID=" + Show.TVDBID + "&SeasonNumber=" + SelectedSeason.Season + "&ShowTitle=" + Show.Title, UriKind.Relative));
            }
        }

        public void updateDisplay()
        {
            NotifyOfPropertyChange("Show");
            NotifyOfPropertyChange("ShowTitle");
            NotifyOfPropertyChange("RunTime");
            NotifyOfPropertyChange("AirTime");
            NotifyOfPropertyChange("Certification");
            NotifyOfPropertyChange("Country");
            NotifyOfPropertyChange("RatingImage");
            NotifyOfPropertyChange("RatingPercentage");
            NotifyOfPropertyChange("RatingCount");
            NotifyOfPropertyChange("LoggedIn");
            NotifyOfPropertyChange("UserRatingText");
            NotifyOfPropertyChange("LoveImage");
            NotifyOfPropertyChange("HateImage");
            NotifyOfPropertyChange("Seasons");
        }

        private void newRatings()
        {
            NotifyOfPropertyChange("RatingImage");
            NotifyOfPropertyChange("RatingPercentage");
            NotifyOfPropertyChange("RatingCount");
        }

        public void SendShout()
        {
            navigationService.Navigate(new Uri("/Views/Shout.xaml?ItemType=show&TVDBID=" + TVDBID, UriKind.Relative));
        }

    }
}
