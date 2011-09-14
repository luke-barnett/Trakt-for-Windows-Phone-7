using System.Diagnostics;
using Caliburn.Micro;
using Trakt_for_Windows_Phone_7.Framework;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class EpisodeViewModel : BaseViewModel
    {
        private string _tvdbid;
        private string _season;
        private string _episode;

        public EpisodeViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container) : base(navigationService, windowManager, container)
        {
        }

        public string TVDBID { get { return _tvdbid; } set { _tvdbid = value; GetEpisode(); } }
        public string Season { get { return _season; } set { _season = value; GetEpisode(); } }
        public string Episode { get { return _episode; } set { _episode = value; GetEpisode(); } }

        private void GetEpisode()
        {
            if (string.IsNullOrEmpty(TVDBID) || string.IsNullOrEmpty(Season) || string.IsNullOrEmpty(Episode))
                return;
            Debug.WriteLine("TVDBID {0} Season {1} Episode {2}", TVDBID, Season, Episode);
        }
    }
}
