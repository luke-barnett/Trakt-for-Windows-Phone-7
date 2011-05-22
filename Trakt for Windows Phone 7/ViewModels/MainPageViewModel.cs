using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;

namespace Trakt_for_Windows_Phone_7.ViewModels
{


    public class MainPageViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public MainPageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            TraktSettings.Username = "Onenottoforget";
            TraktSettings.Password = "14499cef17c09db01929ca1458dedb4f15b85ce1";
            TraktAPI.TraktAPI.getTrendingMovies().Subscribe(res => Movies = res);
            TraktAPI.TraktAPI.getTrendingShows().Subscribe(res => Shows = res);
        }

        private TraktMovie[] _movies;
        public TraktMovie[] Movies { get { return _movies; } set { _movies = value; NotifyOfPropertyChange("Movies"); } }

        private TraktShow[] _shows;
        public TraktShow[] Shows { get { return _shows; } set { _shows = value; NotifyOfPropertyChange("Shows"); } }

    }
}