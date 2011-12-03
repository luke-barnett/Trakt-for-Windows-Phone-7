using Caliburn.Micro;
using TraktAPI.TraktModels;

namespace TraktAPI
{
    [SurviveTombstone]
    public static class TraktSettings
    {
        private static readonly TraktLoginStatus PrivateLoginStatus = new TraktLoginStatus();

        [SurviveTombstone]
        public static string Username { get; set; }

        [SurviveTombstone]
        public static string Password { get; set; }

        [SurviveTombstone]
        public static TraktLoginStatus LoginStatus { get { return PrivateLoginStatus; } }

        public new static string ToString { get { return string.Format("Username: {0} Password: {1} LoggedIn: {2}", Username, Password, LoginStatus);} }
    }
}
