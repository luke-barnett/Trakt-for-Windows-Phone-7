using System;
using System.Runtime.Serialization;


namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktShout
    {
        [DataMember(Name="inserted")]
        public long Inserted { get; set; }
        [DataMember(Name = "shout")]
        public string Shout { get; set; }
        [DataMember(Name = "user")]
        public TraktUser User { get; set; }

        public string QuoteSurrounded { get { return string.Format("\"{0}\"", Shout); } }
    }
}
