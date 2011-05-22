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
        public TraktMovie Movie { get { return _movie; } set { _movie = value; NotifyOfPropertyChange("Movie"); } }

        private string _movieTitle;
        public string MovieTitle { get { return _movieTitle; } set { _movieTitle = value; TraktAPI.TraktAPI.getMovie(MovieTitle).Subscribe(movie => Movie = movie); System.Diagnostics.Debug.WriteLine(MovieTitle); } }
    }
}
