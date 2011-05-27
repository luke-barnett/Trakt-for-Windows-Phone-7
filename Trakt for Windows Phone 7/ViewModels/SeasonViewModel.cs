using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using System.Windows;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class SeasonViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public SeasonViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        private string _TVDBID;
        private bool bothValues = false;
        [SurviveTombstone]
        public string TVDBID { get { return _TVDBID; } set { _TVDBID = value; if(bothValues) TraktAPI.TraktAPI.getSeason(TVDBID, SeasonNumber).Subscribe(onNext: result => Season = result, onError: error => handleError(error)); bothValues = true;} }

        private string _SeasonNumber;
        [SurviveTombstone]
        public string SeasonNumber { get { return _SeasonNumber; } set { _SeasonNumber = value; if (bothValues) TraktAPI.TraktAPI.getSeason(TVDBID, SeasonNumber).Subscribe(onNext: result => Season = result, onError: error => handleError(error)); bothValues = true; } }

        private TraktEpisode[] _Season;
        [SurviveTombstone]
        public TraktEpisode[] Season { get { return _Season; } set { _Season = value; updateDisplay(); } }

        private string _ShowTitle;
        public string ShowTitle { get { return _ShowTitle; } set { _ShowTitle = value; NotifyOfPropertyChange("ShowTitle"); } }

        public TraktEpisode SelectedEpisode { get; set; }

        private void updateDisplay()
        {
            NotifyOfPropertyChange("Season");
        }

        public void ViewEpisode()
        {
            if(SelectedEpisode != null)
                navigationService.Navigate(new Uri("/Views/Episode.xaml?TVDBID=" + TVDBID + "&SeasonNumber=" + SeasonNumber + "&EpisodeNumber=" + SelectedEpisode.Episode, UriKind.Relative));
        }
    }
}
