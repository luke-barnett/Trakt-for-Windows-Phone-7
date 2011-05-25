using System;
using System.Runtime.Serialization;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktEpisodeSummary
    {
        [DataMember(Name = "show")]
        public TraktShow Show { get; set; }

        [DataMember(Name = "episode")]
        public TraktEpisode Episode { get; set; }

        public Uri Uri { get { return new Uri("/Views/Episode.xaml?TVDBID=" + Show.TVDBID + "&SeasonNumber=" + Episode.Season + "&EpisodeNumber=" + Episode.Episode, UriKind.Relative); } }
    }
}
