using System;
using System.Runtime.Serialization;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktEpisode
    {
        [DataMember(Name = "season")]
        public int Season { get; set; }

        [DataMember(Name = "episode")]
        public int Episode { get; set; }

        [DataMember(Name = "number")]
        public int Number { get { return Episode; } set { Episode = value; } }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "overview")]
        public string Overview { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "first_aired")]
        public long FirstAired { get; set; }

        [DataMember(Name = "images")]
        public TraktImages Images { get; set; }

        [DataMember(Name = "ratings")]
        public TraktRatings Ratings { get; set; }

        [DataMember(Name = "watched")]
        public bool Watched { get; set; }

        [DataMember(Name = "rating")]
        public string Rating { get; set; }

        [DataMember(Name = "plays")]
        public int Plays { get; set; }

        public bool OnWatchList { get; set; }

        public string ShowTVDBID { get; set; }

        public string CombinedSeasonAndEpisodeText
        {
            get
            {
                return string.Format("Season {0} Episode {1}", Season, Episode);
            }
        }

        public Uri Uri { get { return new Uri("/Views/Episode.xaml?TVDBID=" + ShowTVDBID + "&SeasonNumber=" + Season + "&EpisodeNumber=" + Episode, UriKind.Relative); } }
    }
}
