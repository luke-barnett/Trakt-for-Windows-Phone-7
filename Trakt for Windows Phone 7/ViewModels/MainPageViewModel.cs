using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using System.Windows;

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
            TraktSettings.LoggedIn = true;
            TraktAPI.TraktAPI.getTrendingMovies().Subscribe(res => Movies = res);
            TraktAPI.TraktAPI.getTrendingShows().Subscribe(res => Shows = res);

        }

        private TraktMovie[] _movies;
        public TraktMovie[] Movies { get { return _movies; } set { _movies = value; NotifyOfPropertyChange("Movies"); } }

        private TraktShow[] _shows;
        public TraktShow[] Shows { get { return _shows; } set { _shows = value; NotifyOfPropertyChange("Shows"); } }


        public void Movie0()
        {
            if(_movies != null && _movies[0] != null)
                navigationService.Navigate(new Uri("/Views/Movie.xaml?MovieTitle=" + _movies[0].IMDBID, UriKind.Relative));
        }

        public void Movie1()
        {
            if (_movies != null && _movies[1] != null)
                navigationService.Navigate(new Uri("/Views/Movie.xaml?MovieTitle=" + _movies[1].IMDBID, UriKind.Relative));
        }

        public void Movie2()
        {
            if (_movies != null && _movies[2] != null)
                navigationService.Navigate(new Uri("/Views/Movie.xaml?MovieTitle=" + _movies[2].IMDBID, UriKind.Relative));
        }

        public void Show0()
        {
        }

        public void Show1()
        {
            
        }

        public void Show2()
        {
            
        }
    }
}