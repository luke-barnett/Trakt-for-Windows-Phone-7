using System;
using System.Runtime.Serialization;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktEpisodeShout
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
        [DataMember(Name = "tvdb_id")]
        public string TVDBID { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "year")]
        public int Year { get; set; }
        [DataMember(Name = "shout")]
        public string Shout { get; set; }
        [DataMember(Name = "season")]
        public int SeasonNumber { get; set; }
        [DataMember(Name = "episode")]
        public int EpisodeNumber { get; set; }
    }
}