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
    public class SearchViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public SearchViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        private string _SearchString;
        [SurviveTombstone]
        public string SearchString { get { return _SearchString; } set { _SearchString = value;} }

        private TraktMovie[] _MovieResults;
        [SurviveTombstone]
        public TraktMovie[] MovieResults { get { return _MovieResults; } set { _MovieResults = value; NotifyOfPropertyChange("MovieResults"); SearchResultsObtained(); } }

        private TraktShow[] _ShowResults;
        [SurviveTombstone]
        public TraktShow[] ShowResults { get { return _ShowResults; } set { _ShowResults = value; NotifyOfPropertyChange("ShowResults"); SearchResultsObtained(); } }

        private TraktEpisodeSummary[] _EpisodeResults;
        [SurviveTombstone]
        public TraktEpisodeSummary[] EpisodeResults { get { return _EpisodeResults; } set { _EpisodeResults = value; NotifyOfPropertyChange("EpisodeResults"); SearchResultsObtained(); } }

        public TraktMovie SelectedMovie { get; set; }

        public TraktShow SelectedShow { get; set; }

        public TraktEpisodeSummary SelectedEpisode { get; set; }

        private void StartSearch()
        {
            NotifyOfPropertyChange("SearchString");
            NotifyOfPropertyChange("WhatWeAreSearchingFor");
            string urlEncoded = SearchString;
            while (urlEncoded.Contains(" "))
                urlEncoded = urlEncoded.Replace(" ", "+");
            System.Diagnostics.Debug.WriteLine(urlEncoded);
            TraktAPI.TraktAPI.searchMovies(urlEncoded).Subscribe(onNext: movies => MovieResults = movies, onError: error => handleError(error));
            TraktAPI.TraktAPI.searchShows(urlEncoded).Subscribe(onNext: shows => ShowResults = shows, onError: error => handleError(error));
            TraktAPI.TraktAPI.searchEpisodes(urlEncoded).Subscribe(onNext: episodes => EpisodeResults = episodes, onError: error => handleError(error));
        }

        private void SearchResultsObtained()
        {
            NotifyOfPropertyChange("showMovieResults");
            NotifyOfPropertyChange("showEpisodeResults");
            NotifyOfPropertyChange("showShowResults");
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

        public string showEpisodeResults
        {
            get
            {
                if (EpisodeResults == null)
                    return "Collapsed";
                return "Visible";
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

        public string WhatWeAreSearchingFor
        {
            get
            {
                if (SearchString == null)
                {
                    return "";
                }
                else
                    return string.Format("Search results for \"{0}\"", SearchString);
            }
        }

        public void KeyUpOccured(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartSearch();
            }
        }

        public void GoToMovie()
        {
            if(SelectedMovie != null)
                navigationService.Navigate(SelectedMovie.Uri);
        }

        public void GoToShow()
        {
            if(SelectedShow != null)
               navigationService.Navigate(SelectedShow.Uri);
        }

        public void GoToEpisode()
        {
            if(SelectedEpisode != null)
                navigationService.Navigate(SelectedEpisode.Uri);
        }
    }
}

