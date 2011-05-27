using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using System.Windows;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class EpisodeViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public EpisodeViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        private string _TVDBID;
        [SurviveTombstone]
        public string TVDBID { get { return _TVDBID; } set { _TVDBID = value; System.Diagnostics.Debug.WriteLine(TVDBID); } }

        private string _SeasonNumber;
        [SurviveTombstone]
        public string SeasonNumber { get { return _SeasonNumber; } set { _SeasonNumber = value; System.Diagnostics.Debug.WriteLine(SeasonNumber); } }

        private string _EpisodeNumber;
        [SurviveTombstone]
        public string EpisodeNumber { get { return _EpisodeNumber; } set { _EpisodeNumber = value; System.Diagnostics.Debug.WriteLine(EpisodeNumber); TraktAPI.TraktAPI.getEpisodeSummary(TVDBID, SeasonNumber, EpisodeNumber).Subscribe(onNext: response => Episode = response, onError: error => handleError(error)); TraktAPI.TraktAPI.getEpisodeShouts(TVDBID, SeasonNumber, EpisodeNumber).Subscribe(onNext: shouts => Shouts = shouts, onError: error => handleError(error)); } }

        private TraktEpisodeSummary _Episode;
        [SurviveTombstone]
        public TraktEpisodeSummary Episode { get { return _Episode; } set { _Episode = value; updateDisplay(); } }

        private TraktRatings ratings { set { Episode.Episode.Ratings = value; newRatings(); } }

        private TraktShout[] _Shouts;
        [SurviveTombstone]
        public TraktShout[] Shouts { get { return _Shouts; } set { _Shouts = value; NotifyOfPropertyChange("Shouts"); } }

        private void updateDisplay()
        {
            NotifyOfPropertyChange("Episode");
            NotifyOfPropertyChange("EpisodeTitle");
            NotifyOfPropertyChange("AirTime");
            NotifyOfPropertyChange("Country");
            NotifyOfPropertyChange("Certification");
            NotifyOfPropertyChange("CombinedSeasonAndEpisodeText");
            NotifyOfPropertyChange("RatingImage");
            NotifyOfPropertyChange("RatingPercentage");
            NotifyOfPropertyChange("RatingCount");
            NotifyOfPropertyChange("LoggedIn");
            NotifyOfPropertyChange("UserRatingText");
            NotifyOfPropertyChange("LoveImage");
            NotifyOfPropertyChange("HateImage");
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

        public string EpisodeTitle 
        {
            get
            {
                if (Episode == null)
                    return "";
                return Episode.Episode.Title;
            }
        }

        public string AirTime
        {
            get
            {
                if (Episode == null)
                    return "";
                return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Episode.Episode.FirstAired).ToLongDateString();
            }
        }

        public string Country
        {
            get
            {
                if (Episode == null)
                    return "";
                return Episode.Show.Country;
            }
        }

        public string Certification
        {
            get
            {
                if (Episode == null)
                    return "";
                return Episode.Show.Certification;
            }
        }

        public string CombinedSeasonAndEpisodeText
        {
            get
            {
                if (Episode == null)
                    return "";
                return Episode.Episode.CombinedSeasonAndEpisodeText;
            }
        }

        public string RatingImage
        {
            get
            {
                if (Episode == null || Episode.Episode.Ratings == null)
                    return "";
                if (Episode.Episode.Ratings.Percentage >= 50)
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/iconLove.png";
                else
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/iconHate.png";
            }
        }
        public string RatingPercentage
        {
            get
            {
                if (Episode == null)
                    return "";
                return Episode.Episode.Ratings.Percentage + "%";
            }
        }

        public string RatingCount
        {
            get
            {
                if (Episode == null)
                    return "";
                return string.Format("{0} votes", Episode.Episode.Ratings.Votes);
            }

        }

        public string LoggedIn
        {
            get
            {
                if (Episode != null && TraktSettings.LoggedIn)
                    return "Visible";
                else
                    return "Collapsed";
            }
        }

        public string UserRatingText
        {
            get
            {
                if (Episode == null || Episode.Episode.Rating == null)
                    return "";
                if (Episode.Episode.Rating.CompareTo("love") == 0)
                    return "Love it!";
                if (Episode.Episode.Rating.CompareTo("hate") == 0)
                    return "Lame";
                return "";
            }
        }

        public string LoveImage
        {
            get
            {
                if (Episode == null || Episode.Episode.Rating == null)
                    return "";

                if (Episode.Episode.Rating.CompareTo("hate") == 0)
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/love_f.png";
                else
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/love.png";
            }
        }

        public string HateImage
        {
            get
            {
                if (Episode == null || Episode.Episode.Rating == null)
                    return "";
                if (Episode.Episode.Rating.CompareTo("love") == 0)
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/hate_f.png";
                else
                    return "/Trakt%20for%20Windows%20Phone%207;component/Artwork/hate.png";
            }
        }

        public string WatchedThis
        {
            get
            {
                if (Episode == null)
                    return "";
                return Episode.Episode.Watched ? "You've watched This!" : "I've watched this";
            }
        }

        public string WatchList
        {
            get
            {
                if (Episode == null)
                    return "";
                return Episode.Episode.OnWatchList ? "Remove from watchlist" : "Add to watchlist";
            }
        }

        public string showWatchList
        {
            get
            {
                if (Episode == null || Episode.Episode.Watched)
                    return "Collapsed";
                return "Visible";
            }
        }

        public void Love()
        {
            if (Episode.Episode.Rating.CompareTo("love") == 0)
            {
                TraktAPI.TraktAPI.rateEpisode(TVDBID, Episode.Show.IMDBID, EpisodeTitle, Episode.Show.Year, SeasonNumber, EpisodeNumber, TraktRateTypes.unrate.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Episode.Episode.Rating = "";
            }
            else
            {
                TraktAPI.TraktAPI.rateEpisode(TVDBID, Episode.Show.IMDBID, EpisodeTitle, Episode.Show.Year, SeasonNumber, EpisodeNumber, TraktRateTypes.love.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Episode.Episode.Rating = "love";
            }
            NotifyOfPropertyChange("LoveImage");
            NotifyOfPropertyChange("HateImage");
            NotifyOfPropertyChange("UserRatingText");
        }

        public void Hate()
        {
            if (Episode.Episode.Rating.CompareTo("hate") == 0)
            {
                TraktAPI.TraktAPI.rateEpisode(TVDBID, Episode.Show.IMDBID, EpisodeTitle, Episode.Show.Year, SeasonNumber, EpisodeNumber, TraktRateTypes.unrate.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Episode.Episode.Rating = "";
            }
            else
            {
                TraktAPI.TraktAPI.rateEpisode(TVDBID, Episode.Show.IMDBID, EpisodeTitle, Episode.Show.Year, SeasonNumber, EpisodeNumber, TraktRateTypes.hate.ToString()).Subscribe(onNext: response => ratings = response.Ratings, onError: error => handleError(error));
                Episode.Episode.Rating = "hate";
            }
            NotifyOfPropertyChange("LoveImage");
            NotifyOfPropertyChange("HateImage");
            NotifyOfPropertyChange("UserRatingText");
        }

        public void ToggleWatched()
        {
            if (Episode.Episode.Watched)
            {
                TraktAPI.TraktAPI.unwatchEpisode(Episode.Show.TVDBID, Episode.Show.IMDBID, Episode.Show.Title, Episode.Show.Year, Episode.Episode.Season.ToString(), Episode.Episode.Episode.ToString());
                Episode.Episode.Watched = false;
            }
            else
            {
                TraktAPI.TraktAPI.watchEpisode(Episode.Show.TVDBID, Episode.Show.IMDBID, Episode.Show.Title, Episode.Show.Year, Episode.Episode.Season.ToString(), Episode.Episode.Episode.ToString());
                Episode.Episode.Watched = true;
                Episode.Episode.OnWatchList = false;
                NotifyOfPropertyChange("WatchList");
            }
            NotifyOfPropertyChange("WatchedThis");
            NotifyOfPropertyChange("showWatchList");

        }

        public void ToggleWatchList()
        {
            if (Episode.Episode.OnWatchList)
            {
                TraktAPI.TraktAPI.unwatchListEpisode(Episode.Show.TVDBID, Episode.Show.IMDBID, Episode.Show.Title, Episode.Show.Year, Episode.Episode.Season.ToString(), Episode.Episode.Episode.ToString());
                Episode.Episode.OnWatchList = false;
            }
            else
            {
                TraktAPI.TraktAPI.watchListEpisode(Episode.Show.TVDBID, Episode.Show.IMDBID, Episode.Show.Title, Episode.Show.Year, Episode.Episode.Season.ToString(), Episode.Episode.Episode.ToString());
                Episode.Episode.OnWatchList = true;
            }
            NotifyOfPropertyChange("WatchList");
        }

        public void SendShout()
        {
            navigationService.Navigate(new Uri("/Views/Shout.xaml?ItemType=episode&TVDBID=" + TVDBID +"&Season=" + SeasonNumber + "&Episode=" + EpisodeNumber, UriKind.Relative));
        }
    }
}
