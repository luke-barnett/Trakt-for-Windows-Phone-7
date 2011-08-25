﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        private List<PivotItem> _pivotItems;
        private String _trendingType;

        #endregion
        
        public MainPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _pivotItems = new List<PivotItem>();
            StartLoading();
        }

        #region Public Parameters

        /// <summary>
        /// The List of items for the pivot control
        /// </summary>
        public List<PivotItem> PivotItems { get { return _pivotItems; } set { _pivotItems = value; NotifyOfPropertyChange(()=> PivotItems); }}

        /// <summary>
        /// The type of trending currently being showen
        /// </summary>
        public string TrendingType { get { return _trendingType; } set { _trendingType = value; NotifyOfPropertyChange(() => TrendingType); NotifyOfPropertyChange(() => ShowTrendingType); } }

        public Visibility ShowTrendingType { get { return string.IsNullOrEmpty(TrendingType) ? Visibility.Collapsed : Visibility.Visible; } }

        #endregion

        #region Private Methods

        /// <summary>
        /// Starts the loading from a fresh state
        /// </summary>
        private void StartLoading()
        {
            Debug.WriteLine("Starting loading of trending items");
            GetTrendingMovies();
        }

        /// <summary>
        /// Generates the UI Elements to display
        /// </summary>
        /// <param name="poster">The poster to load</param>
        /// <param name="title">The title to use</param>
        /// <returns>The set of elements to use</returns>
        private IEnumerable<UIElement> GenerateUiElements(Uri poster, string title)
        {
            var elements = new List<UIElement>();

            var image = new Image { Source = DefaultPoster };

            ProgressBarVisible = true;
            var bitmap = new BitmapImage(poster) { CreateOptions = BitmapCreateOptions.None };
            bitmap.ImageOpened += (sender, args) => { image.Source = bitmap; ProgressBarVisible = false; };
            bitmap.ImageFailed += (sender, args) => { Debug.WriteLine("Failed to load poster for {0}", title); ProgressBarVisible = false; };

            elements.Add(image);

            var textBlock = new TextBlock
            {
                Text = title,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 32,
                Margin = new Thickness { Bottom = 5d, Top = 5d }
            };

            var textGrid = new Grid
            {
                Background = new SolidColorBrush(Colors.DarkGray) { Opacity = 0.8d },
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            textGrid.Children.Add(textBlock);
            elements.Add(textGrid);

            return elements;
        }

        #region Trending Movies

        /// <summary>
        /// Gets trending movies
        /// </summary>
        private void GetTrendingMovies()
        {
            Debug.WriteLine("Getting trending movies");
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetTrendingMovies().Subscribe(UpdateDisplayForMovies, HandleError);
        }

        /// <summary>
        /// Updates the pivot control for trending movies
        /// </summary>
        /// <param name="trendingMovies">The list of movies to use for the display</param>
        private void UpdateDisplayForMovies(TraktMovie[] trendingMovies)
        {
            Debug.WriteLine("Received trending movies, processing");
            PivotItems.Clear();
            for (var i = 0; i < 10; i++)
            {
                var trendingMovie = trendingMovies[i];
                var pivotMovie = new PivotItem();

                var movieGrid = new Grid();
                var gestureListener = GestureService.GetGestureListener(movieGrid);
                gestureListener.DoubleTap += (sender, args) => MovieSelected(trendingMovie);

                foreach (var uiElement in GenerateUiElements(new Uri(trendingMovie.Images.Poster), trendingMovie.TitleAndYear))
                {
                    movieGrid.Children.Add(uiElement);
                }

                pivotMovie.Content = movieGrid;
                PivotItems.Add(pivotMovie);
                Debug.WriteLine("Added trending movie {0}", trendingMovie.TitleAndYear);
            }
            PivotItems = new List<PivotItem>(PivotItems);
            TrendingType = "Trending Movies";
            ProgressBarVisible = false;
            Debug.WriteLine("Finished processing");
        }

        /// <summary>
        /// Handles double tapping on a movie
        /// </summary>
        /// <param name="selectedMovie">The movie that was selected</param>
        private void MovieSelected(TraktMovie selectedMovie)
        {
            Debug.WriteLine("Captured a double tap event on {0}", selectedMovie.TitleAndYear);
            NavigationService.Navigate(new Uri("/Views/MovieView.xaml?IMDBID=" + selectedMovie.IMDBID, UriKind.Relative));
        }

        #endregion

        #region Trending Shows

        /// <summary>
        /// Gets trending shows
        /// </summary>
        private void GetTrendingShows()
        {
            Debug.WriteLine("Getting trending shows");
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetTrendingShows().Subscribe(UpdateDisplayForShows, HandleError);
        }

        /// <summary>
        /// Updates the pivot control for trending shows
        /// </summary>
        /// <param name="trendingShows">The list of trending shows to use for the display</param>
        private void UpdateDisplayForShows(TraktShow[] trendingShows)
        {
            Debug.WriteLine("Received trending shows, processing");
            PivotItems.Clear();
            for(var i = 0; i < 10; i++)
            {
                var trendingShow = trendingShows[i];
                var pivotShow = new PivotItem();

                var showGrid = new Grid();
                var gestureListener = GestureService.GetGestureListener(showGrid);
                gestureListener.DoubleTap += (sender, args) => ShowSelected(trendingShow);

                foreach (var uiElement in GenerateUiElements(new Uri(trendingShow.Images.Poster), trendingShow.TitleAndYear))
                {
                    showGrid.Children.Add(uiElement);
                }

                pivotShow.Content = showGrid;
                PivotItems.Add(pivotShow);
                Debug.WriteLine("Added trending show {0}", trendingShow.TitleAndYear);
            }
            PivotItems = new List<PivotItem>(PivotItems);
            TrendingType = "Trending Shows";
            ProgressBarVisible = false;
            Debug.WriteLine("Finished processing");
        }

        /// <summary>
        /// Handles double tapping on a show
        /// </summary>
        /// <param name="selectedShow">The show selected</param>
        private void ShowSelected(TraktShow selectedShow)
        {
            Debug.WriteLine("Captured a double tap event on {0}", selectedShow.TitleAndYear);
            NavigationService.Navigate(new Uri("/Views/ShowView.xaml?TVDBID=" + selectedShow.IMDBID, UriKind.Relative));
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets trending movies
        /// </summary>
        public void BtnGetTrendingMovies()
        {
            GetTrendingMovies();
        }

        /// <summary>
        /// Gets trending shows
        /// </summary>
        public void BtnGetTrendingShows()
        {
            GetTrendingShows();
        }

        #endregion
    }
}