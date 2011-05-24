using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using System.Windows;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class EpisodeViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public EpisodeViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        private string _TVDBID;
        public string TVDBID { get { return _TVDBID; } set { _TVDBID = value; System.Diagnostics.Debug.WriteLine(TVDBID); } }

        private string _SeasonNumber;
        public string SeasonNumber { get { return _SeasonNumber; } set { _SeasonNumber = value; System.Diagnostics.Debug.WriteLine(SeasonNumber); } }

        private string _EpisodeNumber;
        public string EpisodeNumber { get { return _EpisodeNumber; } set { _EpisodeNumber = value; System.Diagnostics.Debug.WriteLine(EpisodeNumber); } }

        public string EpisodeTitle { get; set; }
    }
}
