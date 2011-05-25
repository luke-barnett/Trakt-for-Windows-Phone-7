namespace TraktAPI
{
    public static class TraktURIs
    {
        public const string ApiKey = "93c06e6abc162a469009d05891adbbc159ed42e2";
        public const string CreateAccount = @"http://api.trakt.tv/account/create/" + ApiKey;
        public const string TestAccount = @"http://api.trakt.tv/account/test/" + ApiKey;
        public const string CalendarShows = @"http://api.trakt.tv/user/calendar/shows.json/" + ApiKey + @"/{0}/{1}/{2}";
        //Generic Movie
        public const string MovieDetails = @"http://api.trakt.tv/movie/{0}.json/" + ApiKey + @"/{1}";
        //User Movie
        public const string SyncMovieLibrary = @"http://api.trakt.tv/movie/{0}/" + ApiKey;
        //Rating
        public const string RateItem = @"http://api.trakt.tv/rate/{0}/" + ApiKey;
        //Recommendations
        public const string Recommendations = @"http://api.trakt.tv/recommendations/{0}/" + ApiKey;
        //Search
        public const string Search = @"http://api.trakt.tv/search/{0}.json/" + ApiKey + @"/{1}";
        //Trending
        public const string Trending = @"http://api.trakt.tv/{0}/trending.json/" + ApiKey;
        //Shout
        public const string Shout = @"http://api.trakt.tv/shout/{0}/" + ApiKey;
        //WatchList
        public const string WatchList = @"http://api.trakt.tv/user/watchlist/{0}.json/" + ApiKey + @"/{1}";
        //Generic Show
        public const string ShowDetails = @"http://api.trakt.tv/show/{0}.json/" + ApiKey + @"/{1}";
        public const string ShowSummary = @"http://api.trakt.tv/show/{0}.json/" + ApiKey + @"/{1}/extended";
        public const string EpisodeDetails = @"http://api.trakt.tv/show/episode/{0}.json/" + ApiKey + @"/{1}/{2}/{3}";
        public const string SeasonInfo = @"http://api.trakt.tv/show/seasons.json/" + ApiKey + @"/{0}";
        public const string Season = @"http://api.trakt.tv/show/season.json/" + ApiKey + @"/{0}/{1}";
        //User Show
        public const string SyncEpisodeLibrary = @"http://api.trakt.tv/show/episode/{0}/" + ApiKey;
        public const string SynyShow = @"http://api.trakt.tv/show/{0}/" + ApiKey;
        //User
        public const string UserCalendarShows = @"http://api.trakt.tv/user/calendar/shows.json/" + ApiKey + @"/{0}/{1}/{2}";
        public const string UserFriends = @"http://api.trakt.tv/user/friends.json/" + ApiKey + @"/{0}";
        public const string UserLibrary = @"http://api.trakt.tv/user/library/{0}/all.json/" + ApiKey + @"/{1}";
        public const string UserProfile = @"http://api.trakt.tv/user/profile.json/" + ApiKey + @"/{0}/{1}";
        public const string UserWatchList = @"http://api.trakt.tv/user/watchlist/{0}.json/" + ApiKey + "/{1}";

    }
}
