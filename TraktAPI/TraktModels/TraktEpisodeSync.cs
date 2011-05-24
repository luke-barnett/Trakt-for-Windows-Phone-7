using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktEpisodeSync
    {
        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "imdb_id")]
        public string IMDBID { get; set; }

        [DataMember(Name = "tvdb_id")]
        public string TVDBID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "year")]
        public int Year { get; set; }

        [DataMember(Name = "episodes")]
        public List<Episode> EpisodeList { get; set; }

        [DataContract]
        public class Episode
        {
            [DataMember(Name = "season")]
            public string SeasonNumber { get; set; }

            [DataMember(Name = "episode")]
            public string EpisodeNumber { get; set; }
        }
    }
}
