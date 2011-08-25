using Caliburn.Micro;

namespace TraktAPI
{
    [SurviveTombstone]
    public static class TraktSettings
    {
        [SurviveTombstone]
        public static string Username { get; set; }

        [SurviveTombstone]
        public static string Password { get; set; }

        [SurviveTombstone]
        public static bool LoggedIn { get; set; }

        public new static string ToString { get { return string.Format("Username: {0} Password: {1} LoggedIn: {2}", Username, Password, LoggedIn);} }
    }
}
