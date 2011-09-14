using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            var textBlock = new TextBlock {Text = episode.Title};
            textBlock.MouseLeftButtonDown += (sender, args) =>
                                                 {
                                                     Debug.WriteLine("Selected {0}", episode.Title);
                                                     NavigationService.Navigate(new Uri("/Views/EpisodeView.xaml?TVDBID=" + TVDBID + "&Season=" + episode.Season + "&Episode=" + episode.Episode, UriKind.Relative));
                                                 };
            return textBlock;
        }
    }
}
