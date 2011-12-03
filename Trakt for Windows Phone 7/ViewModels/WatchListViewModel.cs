using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using TraktAPI;
using TraktAPI.TraktModels;
using Trakt_for_Windows_Phone_7.Framework;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class WatchListViewModel : BaseViewModel
    {
        private bool _showMovies;
        private bool _showShows;
        private bool _showEpisodes;

        private List<UIElement> _shows;
        private List<UIElement> _movies;
        private List<UIElement> _episodes;

        public WatchListViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container) : base(navigationService, windowManager, container)
        {
            if (!TraktSettings.LoginStatus.IsLoggedIn)
            {
                MessageBox.Show("Need to be logged in for this feature");
                NavigationService.GoBack();
            }

            _shows = new List<UIElement>();
            _movies = new List<UIElement>();
            _episodes = new List<UIElement>();

            GetShows();
            GetMovies();
            GetEpisodes();

            ApplicationBar = new ApplicationBar();
        }

        public bool EnableShows { get { return _showShows; } set { _showShows = value; NotifyOfPropertyChange(() => EnableShows); NotifyOfPropertyChange(() => ShowsVisibility); NotifyOfPropertyChange(() => MainPivotVisibility); } }

        public bool EnableMovies { get { return _showMovies; } set { _showMovies = value; NotifyOfPropertyChange(() => EnableMovies); NotifyOfPropertyChange(() => MoviesVisibility); NotifyOfPropertyChange(() => MainPivotVisibility); } }

        public bool EnableEpisodes { get { return _showEpisodes; } set { _showEpisodes = value; NotifyOfPropertyChange(() => EnableEpisodes); NotifyOfPropertyChange(() => MoviesVisibility); NotifyOfPropertyChange(() => MainPivotVisibility); } }

        public Visibility MainPivotVisibility { get { return (EnableShows || EnableMovies || EnableEpisodes) ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility ShowsVisibility { get { return (EnableShows) ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility MoviesVisibility { get { return (EnableMovies) ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility EpisodesVisibility { get { return (EnableEpisodes) ? Visibility.Visible : Visibility.Collapsed; } }

        public List<UIElement> Shows { get { return _shows; } set { _shows = value; NotifyOfPropertyChange(() => Shows); } }

        public List<UIElement> Movies { get { return _movies; } set { _movies = value; NotifyOfPropertyChange(() => Movies); } }

        public List<UIElement> Episodes { get { return _episodes; } set { _episodes = value; NotifyOfPropertyChange(() => Episodes); } }


        #region Shows

        private void GetShows()
        {
            Debug.WriteLine("Getting Shows");
            ProgressBarVisible = true;
            EnableShows = false;
            TraktAPI.TraktAPI.GetShowWatchList(TraktSettings.Username).Subscribe(HandleShows, HandleError);
        }

        private void HandleShows(TraktShow[] shows)
        {
            Shows.Clear();
            Shows.AddRange((from show in shows select GenerateGeneralShowElement(show)));
            Shows = new List<UIElement>(Shows);
            ProgressBarVisible = false;
            EnableShows = true;
            Debug.WriteLine("Handled Shows");
        }


        #endregion

        #region Movies

        private void GetMovies()
        {
            Debug.WriteLine("Getting Movies");
            ProgressBarVisible = true;
            EnableMovies = false;
            TraktAPI.TraktAPI.GetMovieWatchList(TraktSettings.Username).Subscribe(HandleMovies, HandleError);
        }

        private void HandleMovies(TraktMovie[] movies)
        {
            Movies.Clear();
            Movies.AddRange((from movie in movies select GenerateGeneralMovieElement(movie)));
            Movies = new List<UIElement>(Movies);
            ProgressBarVisible = false;
            EnableMovies = true;
            Debug.WriteLine("Handled Movies");
        }

        #endregion

        #region Episodes

        private void GetEpisodes()
        {
            Debug.WriteLine("Getting Episodes");
            ProgressBarVisible = true;
            EnableEpisodes = false;
            TraktAPI.TraktAPI.GetAndExtractEpisodeWatchList(TraktSettings.Username).Subscribe(HandleEpisodes, HandleError);
        }

        private void HandleEpisodes(TraktEpisode[] episodes)
        {
            Episodes.Clear();
            Episodes.AddRange((from episode in episodes select GenerateGeneralEpisodeElement(episode, episode.ShowTVDBID)));
            Episodes = new List<UIElement>(Episodes);
            ProgressBarVisible = false;
            EnableEpisodes = true;
            Debug.WriteLine("Handled Episodes");
        }

        #endregion
    }
}
