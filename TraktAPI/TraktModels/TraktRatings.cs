using System;
using System.Runtime.Serialization;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktRatings
    {
        [DataMember(Name = "percentage")]
        public int Percentage { get; set; }

        [DataMember(Name = "votes")]
        public long Votes { get; set; }

        [DataMember(Name = "loved")]
        public long Loved { get; set; }

        [DataMember(Name = "hated")]
        public long Hated { get; set; }
    }
}
