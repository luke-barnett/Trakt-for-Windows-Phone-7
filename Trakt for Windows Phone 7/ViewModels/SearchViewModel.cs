using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Trakt_for_Windows_Phone_7.Framework;
using Microsoft.Phone.Reactive;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private bool _showMovies;
        private bool _showShows;
        private bool _showEpisodes;

        private List<UIElement> _shows;
        private List<UIElement> _movies;
        private List<UIElement> _episodes;

        public SearchViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container) : base(navigationService, windowManager, container)
        {
            _shows = new List<UIElement>();
            _movies = new List<UIElement>();
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

        private void GetShows(string searchTerm)
        {
            Debug.WriteLine("Getting Shows");
            ProgressBarVisible = true;
            EnableShows = false;
            //Search
            TraktAPI.TraktAPI.SearchShows(searchTerm).Subscribe(HandleShows, HandleError);
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

        private void GetMovies(string searchTerm)
        {
            Debug.WriteLine("Getting Movies");
            ProgressBarVisible = true;
            EnableMovies = false;
            //Search
            TraktAPI.TraktAPI.SearchMovies(searchTerm).Subscribe(HandleMovies, HandleError);
        }

        private void HandleMovies(TraktMovie[] movies)
        {
            Movies.Clear();
            Movies.AddRange((from movie in movies select GenerateGeneralMovieElement(movie)));
            Movies = new List<UIElement>(Movies);
            ProgressBarVisible = false;
            EnableMovies = true;
            //Search
        }

        #endregion

        #region Episodes

        private void GetEpisodes(string searchTerm)
        {
            Debug.WriteLine("Getting Episodes");
            ProgressBarVisible = true;
            EnableEpisodes = false;
            TraktAPI.TraktAPI.SearchEpisodes(searchTerm).Subscribe(HandleEpisodes, HandleError);
        }

        private void HandleEpisodes(TraktEpisodeSummary[] episodes)
        {
            Episodes.Clear();
            Episodes.AddRange((from episode in episodes select GenerateGeneralEpisodeElement(episode.Episode, episode.Show.TVDBID)));
            Episodes = new List<UIElement>(Episodes);
            ProgressBarVisible = false;
            EnableEpisodes = true;
            Debug.WriteLine("Handled Episodes");
        }

        #endregion
    }
}
