using System.Runtime.Serialization;

namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktAuthentication
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }
    }
}
