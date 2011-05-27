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
    public class RecommendationsViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public RecommendationsViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            if (TraktSettings.LoggedIn)
                StartSearch();
            else
            {
                MessageBox.Show("You need to be logged in to view recommendations");
                navigationService.GoBack();
            }
        }

        private TraktMovie[] _MovieResults;
        [SurviveTombstone]
        public TraktMovie[] MovieResults { get { return _MovieResults; } set { _MovieResults = value; NotifyOfPropertyChange("MovieResults"); ResultsObtained(); } }

        private TraktShow[] _ShowResults;
        [SurviveTombstone]
        public TraktShow[] ShowResults { get { return _ShowResults; } set { _ShowResults = value; NotifyOfPropertyChange("ShowResults"); ResultsObtained(); } }

        public TraktMovie SelectedMovie { get; set; }

        public TraktShow SelectedShow { get; set; }


        private void StartSearch()
        {
            TraktAPI.TraktAPI.getMovieRecommendations().Subscribe(onNext: movies => MovieResults = movies, onError: error => handleError(error));
            TraktAPI.TraktAPI.getShowRecommendations().Subscribe(onNext: shows => ShowResults = shows, onError: error => handleError(error));
        }

        private void ResultsObtained()
        {
            NotifyOfPropertyChange("showMovieResults");
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

        public string showShowResults
        {
            get
            {
                if (ShowResults == null)
                    return "Collapsed";
                return "Visible";
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

    }
}

