using System.Runtime.Serialization;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktSeasonInfo
    {
        [DataMember(Name = "season")]
        public int Season { get; set; }
        [DataMember(Name = "episodes")]
        public int Episodes { get; set; }
        [DataMember(Name = "url")]
        public string Url { get; set; }
        [DataMember(Name = "images")]
        public TraktImages Images { get; set; }

        public string AsString 
        { 
            get 
            {
                if (Season == 0)
                    return "Specials";
                return string.Format("Season {0}", Season); 
            } 
        }
    }
}
