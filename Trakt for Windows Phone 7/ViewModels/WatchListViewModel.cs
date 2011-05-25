using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class WatchListViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public WatchListViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            if (TraktSettings.LoggedIn)
                StartSearch();
            else
            {
                MessageBox.Show("You need to be logged in to view your watch list");
                navigationService.GoBack();
            }
        }

        private TraktMovie[] _MovieResults;
        public TraktMovie[] MovieResults { get { return _MovieResults; } set { _MovieResults = value; NotifyOfPropertyChange("MovieResults"); ResultsObtained(); } }

        private TraktShow[] _ShowResults;
        public TraktShow[] ShowResults { get { return _ShowResults; } set { _ShowResults = value; NotifyOfPropertyChange("ShowResults"); ResultsObtained(); } }

        private TraktEpisode[] _EpisodeResults;
        public TraktEpisode[] EpisodeResults { get { return _EpisodeResults; } set { _EpisodeResults = value; NotifyOfPropertyChange("EpisodeResults"); ResultsObtained(); } }

        public TraktMovie SelectedMovie { get; set; }

        public TraktShow SelectedShow { get; set; }

        public TraktEpisode SelectedEpisode { get; set; }

        private void StartSearch()
        {
            TraktAPI.TraktAPI.getMovieWatchList(TraktSettings.Username).Subscribe(onNext: movies => MovieResults = movies, onError: error => handleError(error));
            TraktAPI.TraktAPI.getShowWatchList(TraktSettings.Username).Subscribe(onNext: shows => ShowResults = shows, onError: error => handleError(error));
            TraktAPI.TraktAPI.getAndExtractEpisodeWatchList(TraktSettings.Username).Subscribe(onNext: episodes => EpisodeResults = episodes, onError: error => handleError(error));
        }

        private void ResultsObtained()
        {
            NotifyOfPropertyChange("showMovieResults");
            NotifyOfPropertyChange("showShowResults");
            NotifyOfPropertyChange("showEpisodeResults");
        }

        public string showMovieResults
        {
            get
            {
                if (MovieResults == null)
                    return "Collapsed";
                return "Collapsed";
            }
        }

        public string showShowResults
        {
            get
            {
                if (ShowResults == null)
                    return "Collapsed";
                return "Visible";
            }
        }

        public string showEpisodeResults
        {
            get
            {
                if (EpisodeResults == null)
                    return "Collapsed";
                return "Collapsed";
            }
        }

        public void GoToMovie()
        {
            navigationService.Navigate(SelectedMovie.Uri);
        }

        public void GoToShow()
        {
            navigationService.Navigate(SelectedShow.Uri);
        }

        public void GoToEpisode()
        {

            navigationService.Navigate(SelectedEpisode.Uri);
        }

    }
}

