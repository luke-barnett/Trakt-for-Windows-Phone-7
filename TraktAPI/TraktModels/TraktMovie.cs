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
    }
}
