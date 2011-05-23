using System;
using System.Runtime.Serialization;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktMovie
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "year")]
        public int Year { get; set; }

        [DataMember(Name = "released")]
        public long Released { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "trailer")]
        public string Trailer { get; set; }

        [DataMember(Name = "runtime")]
        public int RunTime { get; set; }

        [DataMember(Name = "tagline")]
        public string TagLine { get; set; }

        [DataMember(Name = "overview")]
        public string Overview { get; set; }

        [DataMember(Name = "certification")]
        public string Certification { get; set; }

        [DataMember(Name = "imdb_id")]
        public string IMDBID { get; set; }

        [DataMember(Name = "tmdb_id")]
        public string TMDBID { get; set; }

        [DataMember(Name = "images")]
        public TraktImages Images { get; set; }

        [DataMember(Name = "watchers")]
        public int Watchers { get; set; }

        [DataMember(Name = "top_watchers")]
        public TraktWatcher[] TopWatchers { get; set; }

        [DataMember(Name = "ratings")]
        public TraktRatings Ratings { get; set; }

        [DataMember(Name = "stats")]
        public TraktStats Stats { get; set; }

        [DataMember(Name = "watched")]
        public bool Watched { get; set; }

        [DataMember(Name = "plays")]
        public long Plays { get; set; }

        [DataMember(Name = "rating")]
        public string Rating { get; set; }

        [DataMember(Name = "on_watchlist")]
        public bool OnWatchList { get; set; }

        [DataMember(Name = "in_collection")]
        public bool InCollection { get; set; }
    }
}
