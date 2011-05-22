using System;
using System.Runtime.Serialization;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktShow
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "year")]
        public int Year { get; set; }

        [DataMember(Name = "first_aired")]
        public long FirstAired { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "overview")]
        public string Overview { get; set; }

        [DataMember(Name = "runtime")]
        public int RunTime { get; set; }

        [DataMember(Name = "network")]
        public string Network { get; set; }

        [DataMember(Name = "air_day")]
        public string AirDay { get; set; }

        [DataMember(Name = "air_time")]
        public string AirTime { get; set; }

        [DataMember(Name = "certification")]
        public string Certification { get; set; }

        [DataMember(Name = "imdb_id")]
        public string IMDBID { get; set; }

        [DataMember(Name = "tmdb_id")]
        public string TMDBID { get; set; }

        [DataMember(Name = "tvrage_id")]
        public string TVRAGEID { get; set; }

        [DataMember(Name = "images")]
        public TraktImages Images { get; set; }

        [DataMember(Name = "watchers")]
        public int Watchers { get; set; }
    }
}
