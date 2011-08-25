using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Controls;
using TraktAPI.TraktModels;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Private Parameters

        private readonly INavigationService _navigationService;
        private List<PivotItem> _pivotItems;

        #endregion
        
        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _pivotItems = new List<PivotItem>();
            StartLoading();
        }
        #region Public Parameters

        public List<PivotItem> PivotItems { get { return _pivotItems; } set { _pivotItems = value; NotifyOfPropertyChange(()=> PivotItems); }}

        #endregion

        #region Private Methods

        private void StartLoading()
        {
            Debug.WriteLine("Starting loading of trending items");
            GetTrendingMovies();
        }

        private void GetTrendingMovies()
        {
            Debug.WriteLine("Getting trending movies");
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetTrendingMovies().Subscribe(UpdateDisplayForMovies, HandleError);
        }

        private void UpdateDisplayForMovies(TraktMovie[] trendingMovies)
        {
            Debug.WriteLine("Received trending movies, processing");
            PivotItems.Clear();
            for (int i = 0; i < trendingMovies.Length; i++)
            {
                var trendingMovie = trendingMovies[i];
                var pivotMovie = new PivotItem();

                var movieGrid = new Grid();
                var gestureListener = GestureService.GetGestureListener(movieGrid);
                gestureListener.DoubleTap += (sender, args) => MovieSelected(trendingMovie);

                var image = new Image { Source = new BitmapImage(new Uri(trendingMovie.Images.Poster)) };

                movieGrid.Children.Add(image);

                pivotMovie.Content = movieGrid;
                PivotItems.Add(pivotMovie);
                Debug.WriteLine("Added trending movie {0}", trendingMovie.TitleAndYear);
            }
            PivotItems = new List<PivotItem>(PivotItems);
            ProgressBarVisible = false;
            Debug.WriteLine("Finished processing");
        }

        private void MovieSelected(TraktMovie selectedMovie)
        {
            Debug.WriteLine("Captured a double tap event on {0}", selectedMovie.TitleAndYear);
        }

        private void GetTrendingShows()
        {
            Debug.WriteLine("Getting trending shows");
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetTrendingShows().Subscribe(UpdateDisplayForShows, HandleError);
        }

        private void UpdateDisplayForShows(TraktShow[] trendingShows)
        {
            Debug.WriteLine("Received trending shows, processing");
            PivotItems.Clear();
            for(int i = 0; i < trendingShows.Length; i++)
            {
                var trendingShow = trendingShows[i];
                var pivotShow = new PivotItem();

                var showGrid = new Grid();
                var gestureListener = GestureService.GetGestureListener(showGrid);
                gestureListener.DoubleTap += (sender, args) => ShowSelected(trendingShow);

                var image = new Image { Source = new BitmapImage(new Uri(trendingShow.Images.Poster)) };

                showGrid.Children.Add(image);

                pivotShow.Content = showGrid;
                PivotItems.Add(pivotShow);
                Debug.WriteLine("Added trending show {0}", trendingShow.TitleAndYear);
            }
            PivotItems = new List<PivotItem>(PivotItems);
            ProgressBarVisible = false;
            Debug.WriteLine("Finished processing");
        }

        private void ShowSelected(TraktShow selectedShow)
        {
            Debug.WriteLine("Captured a double tap event on {0}", selectedShow.TitleAndYear);
        }

        #endregion

        #region Public Methods

        public void BtnGetTrendingMovies()
        {
            GetTrendingMovies();
        }

        public void BtnGetTrendingShows()
        {
            GetTrendingShows();
        }

        #endregion
    }
}