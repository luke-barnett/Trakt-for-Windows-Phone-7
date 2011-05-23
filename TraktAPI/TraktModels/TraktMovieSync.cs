using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktMovieSync
    {
        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "movies")]
        public List<Movie> MovieList { get; set; }

        [DataContract]
        public class Movie
        {
            [DataMember(Name = "imdb_id")]
            public string IMDBID { get; set; }

            [DataMember(Name = "tmdb_id")]
            public string TMDBID { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "year")]
            public int Year { get; set; }
        }
    }
}
