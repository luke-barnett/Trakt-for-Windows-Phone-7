using System.Runtime.Serialization;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktImages
    {
        [DataMember(Name = "poster")]
        public string Poster { get; set; }

        [DataMember(Name = "fanart")]
        public string Fanart { get; set; }
    }
}
