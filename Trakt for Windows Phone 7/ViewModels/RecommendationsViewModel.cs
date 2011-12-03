using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Phone.Shell;
using TraktAPI;
using TraktAPI.TraktModels;
using Trakt_for_Windows_Phone_7.Framework;
using Microsoft.Phone.Reactive;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class RecommendationsViewModel : BaseViewModel
    {
        private bool _showMovies;
        private bool _showShows;

        private List<UIElement> _shows;
        private List<UIElement> _movies;

        public RecommendationsViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container) : base(navigationService, windowManager, container)
        {
            if(!TraktSettings.LoginStatus.IsLoggedIn)
            {
                MessageBox.Show("Need to be logged in for this feature");
                NavigationService.GoBack();
            }

            _shows = new List<UIElement>();
            _movies = new List<UIElement>();

            GetShows();
            GetMovies();

            ApplicationBar = new ApplicationBar();

        }

        public bool EnableShows { get { return _showShows; } set { _showShows = value; NotifyOfPropertyChange(() => EnableShows); NotifyOfPropertyChange(() => ShowsVisibility); NotifyOfPropertyChange(() =>MainPivotVisibility); } }

        public bool EnableMovies { get { return _showMovies; } set { _showMovies = value; NotifyOfPropertyChange(() => EnableMovies); NotifyOfPropertyChange(() => MoviesVisibility); NotifyOfPropertyChange(() => MainPivotVisibility); } }

        public Visibility MainPivotVisibility { get { return (EnableShows || EnableMovies) ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility ShowsVisibility { get { return (EnableShows) ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility MoviesVisibility { get { return (EnableMovies) ? Visibility.Visible : Visibility.Collapsed; } }

        public List<UIElement> Shows { get { return _shows; } set { _shows = value; NotifyOfPropertyChange(() => Shows); } }

        public List<UIElement> Movies { get { return _movies; } set { _movies = value; NotifyOfPropertyChange(() => Movies); } }


        #region Shows

        private void GetShows()
        {
            ProgressBarVisible = true;
            EnableShows = false;
            TraktAPI.TraktAPI.GetShowRecommendations().Subscribe(HandleShows, HandleError);
        }

        private void HandleShows(TraktShow[] shows)
        {
            Shows.Clear();
            Shows.AddRange((from show in shows select GenerateGeneralShowElement(show)));
            Shows = new List<UIElement>(Shows);
            ProgressBarVisible = false;
            EnableShows = true;
        }


        #endregion

        #region Movies

        private void GetMovies()
        {
            ProgressBarVisible = true;
            EnableMovies = false;
            TraktAPI.TraktAPI.GetMovieRecommendations().Subscribe(HandleMovies, HandleError);
        }

        private void HandleMovies(TraktMovie[] movies)
        {
            Movies.Clear();
            Movies.AddRange((from movie in movies select GenerateGeneralMovieElement(movie)));
            Movies = new List<UIElement>(Movies);
            ProgressBarVisible = false;
            EnableMovies = true;
        }

        #endregion
    }
}
