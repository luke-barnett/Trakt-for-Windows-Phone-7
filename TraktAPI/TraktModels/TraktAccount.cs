using System;
using System.Runtime.Serialization;


namespace TraktAPI.TraktModels
{
    [DataContract]
    public class TraktAccount
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
        [DataMember(Name = "email")]
        public string Email { get; set; }
    }
}
