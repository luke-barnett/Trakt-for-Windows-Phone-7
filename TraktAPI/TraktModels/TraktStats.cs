using System;
using System.Runtime.Serialization;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktStats
    {
        [DataMember(Name="watchers")]
        public long Watchers { get; set; }

        [DataMember(Name="plays")]
        public long Plays { get; set; }
    }
}
