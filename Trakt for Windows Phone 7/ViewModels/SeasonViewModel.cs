using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Caliburn.Micro;
using TraktAPI.TraktModels;
using Trakt_for_Windows_Phone_7.Framework;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class SeasonViewModel : BaseViewModel
    {
        private List<UIElement> _episodes;
        private string _tvdbid;
        private string _seasonNumber;

        public SeasonViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container) : base(navigationService, windowManager, container)
        {
            _episodes = new List<UIElement>();
        }

        public string TVDBID { get { return _tvdbid; } set { _tvdbid = value; GetEpisodes(); } }

        public string Season { get { return _seasonNumber; } set { _seasonNumber = value; GetEpisodes(); } }

        public List<UIElement> Episodes { get { return _episodes; } set { _episodes = value; NotifyOfPropertyChange(() => Episodes); } }

        private void GetEpisodes()
        {
            if(string.IsNullOrEmpty(TVDBID) || string.IsNullOrEmpty(Season))
                return;
            Debug.WriteLine("TVDBID: {0} Season {1}", TVDBID, Season);
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetSeason(TVDBID, Season).Subscribe(HandleEpisodes, HandleError);
        }

        private void HandleEpisodes(TraktEpisode[] episodes)
        {
            ProgressBarVisible = false;
            Episodes.Clear();
            Episodes.AddRange((from episode in episodes select GenerateUIElement(episode)));
            Episodes = new List<UIElement>(Episodes);
            Debug.WriteLine("Handled the episodes successfully");
        }

        private UIElement GenerateUIElement(TraktEpisode episode)
        {
            var episodeGrid = new Grid{ Margin = new Thickness(5, 10, 5, 10)};
            episodeGrid.ColumnDefinitions.Add(new ColumnDefinition());
            episodeGrid.ColumnDefinitions.Add(new ColumnDefinition());

            var episodeImage = new Image {Source = DefaultScreen, MaxWidth = 200};

            var imageSource = new BitmapImage(new Uri(episode.Images.Screen)) { CreateOptions = BitmapCreateOptions.None};
            imageSource.ImageOpened += (o, e) =>
                                           {
                                               episodeImage.Source = imageSource;
                                               Debug.WriteLine("Successfully got image for episode {0}", episode.Episode);
                                           };
            imageSource.ImageFailed += (o, e) => Debug.WriteLine("Failed to get image for episode {0}", episode.Episode);

            Grid.SetColumn(episodeImage, 0);
            episodeGrid.Children.Add(episodeImage);

            var episodeDetails = new Grid {Margin = new Thickness(5, 0, 0, 0)};
            episodeDetails.RowDefinitions.Add(new RowDefinition());
            episodeDetails.RowDefinitions.Add(new RowDefinition());

            var episodeTitle = new TextBlock{Text = episode.Title, FontSize = 28, TextWrapping = TextWrapping.Wrap};
            Grid.SetRow(episodeTitle, 0);

            episodeDetails.Children.Add(episodeTitle);

            var episodeValue = new TextBlock {Text = episode.CombinedSeasonAndEpisodeText, FontSize = 18};
            Grid.SetRow(episodeValue, 1);

            episodeDetails.Children.Add(episodeValue);

            Grid.SetColumn(episodeDetails, 1);

            episodeGrid.Children.Add(episodeDetails);

            var gestureListener = GestureService.GetGestureListener(episodeGrid);
            gestureListener.DoubleTap += (o, e) =>
                                             {
                                                 Debug.WriteLine("Navigating to episode {0}", episode.Episode);
                                                 NavigationService.Navigate(new Uri("/Views/EpisodeView.xaml?TVDBID=" + TVDBID + "&Season=" + episode.Season + "&Episode=" + episode.Episode, UriKind.Relative));
                                             };

            return episodeGrid;
        }
    }
}
