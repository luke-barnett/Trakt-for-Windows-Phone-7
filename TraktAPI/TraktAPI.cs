using TraktAPI.TraktModels;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TraktAPI
{
    #region Enumerables

    public enum TraktLibraryTypes
    {
        movies,
        shows,
        episodes
    }

    public enum TraktDetailsTypes
    {
        summary,
        watchingnow,
        shouts
    }

    public enum TraktRateTypes
    {
        love,
        hate,
        unrate
    }

    public enum TraktTypes
    {
        episode,
        show,
        movie
    }

    public enum TraktSyncModes
    {
        seen,
        unseen,
        watchlist,
        unwatchlist
    }

    #endregion

    public static class TraktAPI
    {
        #region APICalls

        public static IObservable<TraktMovie[]> GetTrendingMovies()
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Trending, TraktLibraryTypes.movies)), ParseMovieArray);
        }

        public static IObservable<TraktResponse> TestAccount()
        {
            return WebRequestFactory.PostData(new Uri(TraktURIs.TestAccount), ParseTraktResponse, GetUserAuthentication());
        }

        public static IObservable<TraktShow[]> GetTrendingShows()
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Trending, TraktLibraryTypes.shows)), ParseShowArray);
        }

        public static IObservable<TraktMovie> GetMovie(string movieTitle)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.MovieDetails, TraktDetailsTypes.summary, movieTitle)), ParseMovie, GetUserAuthentication());
        }

        public static IObservable<TraktRateResponse> RateMovie(string imdbid, string title, int year, string rating)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.RateItem, TraktTypes.movie)), ParseRatingResponse, CreateMovieRatingPayload(imdbid, title, year, rating));
        }

        private static IObservable<TraktResponse> SyncMovie(string imdbid, string title, int year, string traktSyncMode)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.SyncMovieLibrary, traktSyncMode)), ParseTraktResponse, CreateMovieSyncPayload(imdbid, title, year));
        }

        public static IObservable<TraktResponse> WatchMovie(string imdbid, string title, int year)
        {
            return SyncMovie(imdbid, title, year, TraktSyncModes.seen.ToString());
        }

        public static IObservable<TraktResponse> UnwatchMovie(string imdbid, string title, int year)
        {
            return SyncMovie(imdbid, title, year, TraktSyncModes.unseen.ToString());
        }

        public static IObservable<TraktResponse> WatchListMovie(string imdbid, string title, int year)
        {
            return SyncMovie(imdbid, title, year, TraktSyncModes.watchlist.ToString());
        }

        public static IObservable<TraktResponse> UnwatchListMovie(string imdbid, string title, int year)
        {
            return SyncMovie(imdbid, title, year, TraktSyncModes.unwatchlist.ToString());
        }

        public static IObservable<TraktShow> GetShow(string tvdbid)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.ShowDetails, TraktDetailsTypes.summary, tvdbid)), ParseShow, GetUserAuthentication());
        }

        public static IObservable<TraktRateResponse> RateShow(string tvdbid, string imdbid, string title, int year, string rating)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.RateItem, TraktTypes.show)), ParseRatingResponse, CreateShowRatingPayload(tvdbid, imdbid, title, year, rating));
        }

        public static IObservable<TraktSeasonInfo[]> GetSeasonInfo(string tvdbid)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.SeasonInfo, tvdbid)), ParseSeasonInfo);
        }

        public static IObservable<TraktEpisode[]> GetSeason(string tvdbid, string seasonNumber)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Season, tvdbid, seasonNumber)), ParseSeason, GetUserAuthentication());
        }

        public static IObservable<TraktEpisodeSummary> GetEpisodeSummary(string tvdbid, string seasonNumber, string episodeNumber)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.EpisodeDetails, TraktDetailsTypes.summary, tvdbid, seasonNumber, episodeNumber)), ParseEpisodeSummary, GetUserAuthentication());
        }

        public static IObservable<TraktRateResponse> RateEpisode(string tvdbid, string imdbid, string title, int year, string seasonNumber, string episodeNumber, string rating)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.RateItem, TraktTypes.episode)), ParseRatingResponse, CreateEpisodeRatingPayload(tvdbid, imdbid, title, year, seasonNumber, episodeNumber, rating));
        }

        private static IObservable<TraktResponse> SyncEpisode(string tvdbid, string imdbid, string title, int year, string seasonNumber, string episodeNumber, string traktSyncMode)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.SyncEpisodeLibrary, traktSyncMode)), ParseTraktResponse, CreateEpisodeSyncPayload(tvdbid, imdbid, title, year, seasonNumber, episodeNumber));
        }

        public static IObservable<TraktResponse> WatchEpisode(string tvdbid, string imdbid, string title, int year, string seasonNumber, string episodeNumber)
        {
            return SyncEpisode(tvdbid, imdbid, title, year, seasonNumber, episodeNumber, TraktSyncModes.seen.ToString());
        }

        public static IObservable<TraktResponse> UnwatchEpisode(string tvdbid, string imdbid, string title, int year, string seasonNumber, string episodeNumber)
        {
            return SyncEpisode(tvdbid, imdbid, title, year, seasonNumber, episodeNumber, TraktSyncModes.unseen.ToString());
        }

        public static IObservable<TraktResponse> WatchListEpisode(string tvdbid, string imdbid, string title, int year, string seasonNumber, string episodeNumber)
        {
            return SyncEpisode(tvdbid, imdbid, title, year, seasonNumber, episodeNumber, TraktSyncModes.watchlist.ToString());
        }

        public static IObservable<TraktResponse> UnwatchListEpisode(string tvdbid, string imdbid, string title, int year, string seasonNumber, string episodeNumber)
        {
            return SyncEpisode(tvdbid, imdbid, title, year, seasonNumber, episodeNumber, TraktSyncModes.unwatchlist.ToString());
        }

        public static IObservable<TraktResponse> CreateAccount(string username, string password, string email)
        {
            return WebRequestFactory.PostData(new Uri(TraktURIs.CreateAccount), ParseTraktResponse, CreateNewAccountPayload(username, password, email));
        }

        public static IObservable<TraktResponse> TestAccount(string username, string password)
        {
            return WebRequestFactory.PostData(new Uri(TraktURIs.TestAccount), ParseTraktResponse, CreateTestAccountPayload(username, password));
        }

        public static IObservable<TraktMovie[]> SearchMovies(string searchQuery)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Search, TraktLibraryTypes.movies, searchQuery)), ParseMovieArray);
        }

        public static IObservable<TraktShow[]> SearchShows(string searchQuery)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Search, TraktLibraryTypes.shows, searchQuery)), ParseShowArray);
        }

        public static IObservable<TraktEpisodeSummary[]> SearchEpisodes(string searchQuery)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Search, TraktLibraryTypes.episodes, searchQuery)), ParseEpisodeSummaryArray);
        }

        public static IObservable<TraktMovie[]> GetMovieRecommendations()
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Recommendations, TraktLibraryTypes.movies)), ParseMovieArray, GetUserAuthentication());
        }

        public static IObservable<TraktShow[]> GetShowRecommendations()
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Recommendations, TraktLibraryTypes.shows)), ParseShowArray, GetUserAuthentication());
        }

        public static IObservable<TraktMovie[]> GetMovieWatchList(string username)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.WatchList, TraktLibraryTypes.movies, username)), ParseMovieArray, GetUserAuthentication());
        }

        public static IObservable<TraktShow[]> GetShowWatchList(string username)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.WatchList, TraktLibraryTypes.shows, username)), ParseShowArray, GetUserAuthentication());
        }

        public static IObservable<TraktEpisode[]> GetAndExtractEpisodeWatchList(string username)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.WatchList, TraktLibraryTypes.episodes, username)), ParseAndExtractEpisodeWatchList, GetUserAuthentication());
        }

        public static IObservable<TraktShout[]> GetShowShouts(string tvdbid)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.ShowSummary, TraktDetailsTypes.shouts, tvdbid)), ParseTraktShoutArray);
        }

        public static IObservable<TraktShout[]> GetMovieShouts(string imdbid)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.MovieDetails, TraktDetailsTypes.shouts, imdbid)), ParseTraktShoutArray);
        }

        public static IObservable<TraktShout[]> GetEpisodeShouts(string tvdbid, string seasonNumber, string episodeNumber)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.EpisodeDetails, TraktDetailsTypes.shouts, tvdbid, seasonNumber, episodeNumber)), ParseTraktShoutArray);
        }

        public static IObservable<TraktResponse> MovieShout(string imdbid, string shout)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Shout, TraktTypes.movie)), ParseTraktResponse, CreateMovieShoutPayload(imdbid, shout));
        }

        public static IObservable<TraktResponse> ShowShout(string tvdbid, string shout)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Shout, TraktTypes.show)), ParseTraktResponse, CreateShowShoutPayload(tvdbid, shout));
        }

        public static IObservable<TraktResponse> EpisodeShout(string tvdbid, int seasonnumber, int episodenumber, string shout)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Shout, TraktTypes.episode)), ParseTraktResponse, CreateEpisodeShoutPayload(tvdbid, seasonnumber, episodenumber, shout));
        }

        private static IObservable<TraktResponse> SyncShow(string tvdbid, string traktSyncMode)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.SyncShow, traktSyncMode)), ParseTraktResponse, CreateShowSyncPayload(tvdbid));
        }

        public static IObservable<TraktResponse> WatchListShow(string tvdbid)
        {
            return SyncShow(tvdbid, TraktSyncModes.watchlist.ToString());
        }

        public static IObservable<TraktResponse> UnwatchListShow(string tvdbid)
        {
            return SyncShow(tvdbid, TraktSyncModes.unwatchlist.ToString());
        }

        #endregion

        #region Parsers
        private static TraktMovie[] ParseMovieArray(string json)
        {
            return JsonConvert.DeserializeObject<TraktMovie[]>(json);
        }

        private static TraktShow[] ParseShowArray(string json)
        {
            return JsonConvert.DeserializeObject<TraktShow[]>(json);
        }
        
        private static TraktResponse ParseTraktResponse(string json)
        {
            System.Diagnostics.Debug.WriteLine(json);
            return JsonConvert.DeserializeObject<TraktResponse>(json);
        }

        private static TraktMovie ParseMovie(string json)
        {
            return JsonConvert.DeserializeObject<TraktMovie>(json);
        }

        private static TraktRateResponse ParseRatingResponse(string json)
        {
            System.Diagnostics.Debug.WriteLine(json);
            return JsonConvert.DeserializeObject<TraktRateResponse>(json);
        }

        private static TraktShow ParseShow(string json)
        {
            return JsonConvert.DeserializeObject<TraktShow>(json);
        }

        private static TraktSeasonInfo[] ParseSeasonInfo(string json)
        {
            return JsonConvert.DeserializeObject<TraktSeasonInfo[]>(json);
        }

        private static TraktEpisode[] ParseSeason(string json)
        {
            return JsonConvert.DeserializeObject<TraktEpisode[]>(json);
        }

        private static TraktEpisodeSummary ParseEpisodeSummary(string json)
        {
            return JsonConvert.DeserializeObject<TraktEpisodeSummary>(json);
        }

        private static TraktEpisodeSummary[] ParseEpisodeSummaryArray(string json)
        {
            return JsonConvert.DeserializeObject<TraktEpisodeSummary[]>(json);
        }

        private static TraktEpisode[] ParseAndExtractEpisodeWatchList(string json)
        {
            var asWatchListItems = JsonConvert.DeserializeObject<TraktEpisodeWatchList[]>(json);
            var asEpisodes = new List<TraktEpisode>();

            foreach (TraktEpisodeWatchList watchList in asWatchListItems)
                foreach (TraktEpisode episode in watchList.Episodes)
                {
                    episode.ShowTVDBID = watchList.TVDBID;
                    asEpisodes.Add(episode);
                }
            
            return asEpisodes.ToArray();
        }

        private static TraktShout[] ParseTraktShoutArray(string json)
        {
            return JsonConvert.DeserializeObject<TraktShout[]>(json);
        }

        #endregion

        #region Creators
        private static string GetUserAuthentication()
        {
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(new TraktAuthentication { Username = TraktSettings.Username, Password = TraktSettings.Password }));
            return JsonConvert.SerializeObject(new TraktAuthentication { Username = TraktSettings.Username, Password = TraktSettings.Password });
        }

        private static string CreateMovieRatingPayload(string imdbid, string title, int year, string rating)
        {
            return JsonConvert.SerializeObject(new TraktUserMovieRating { Username = TraktSettings.Username, Password = TraktSettings.Password, IMDBID = imdbid, Title = title, Year = year, Rating = rating });
        }

        private static string CreateMovieSyncPayload(string imdbid, string title, int year)
        {
            return JsonConvert.SerializeObject(new TraktMovieSync { UserName = TraktSettings.Username, Password = TraktSettings.Password, MovieList = new List<TraktMovieSync.Movie> { new TraktMovieSync.Movie { IMDBID = imdbid, Title = title, Year = year }}});
        }

        private static string CreateShowRatingPayload(string tvdbid, string imdbid, string title, int year, string rating)
        {
            return JsonConvert.SerializeObject(new TraktUserShowRating { Username = TraktSettings.Username, Password = TraktSettings.Password, TVDBID = tvdbid, IMDBID = imdbid, Title = title, Year = year, Rating = rating });
        }

        private static string CreateEpisodeRatingPayload(string tvdbid, string imdbid, string title, int year, string seasonNumber, string episodeNumber, string rating)
        {
            return JsonConvert.SerializeObject(new TraktUserEpisodeRating { Username = TraktSettings.Username, Password = TraktSettings.Password, TVDBID = tvdbid, IMDBID = imdbid, Title = title, Year = year, EpisodeNumber = episodeNumber, SeasonNumber = seasonNumber, Rating = rating });
        }

        private static string CreateEpisodeSyncPayload(string tvdbid, string imdbid, string title, int year, string seasonNumber, string episodeNumber)
        {
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(new TraktEpisodeSync { UserName = TraktSettings.Username, Password = TraktSettings.Password, IMDBID = imdbid, TVDBID = tvdbid, Title = title, Year = year, EpisodeList = new List<TraktEpisodeSync.Episode> { new TraktEpisodeSync.Episode { SeasonNumber = seasonNumber, EpisodeNumber = episodeNumber } } }));
            return JsonConvert.SerializeObject(new TraktEpisodeSync { UserName = TraktSettings.Username, Password = TraktSettings.Password, IMDBID = imdbid, TVDBID = tvdbid, Title = title, Year = year, EpisodeList = new List<TraktEpisodeSync.Episode> { new TraktEpisodeSync.Episode { SeasonNumber = seasonNumber, EpisodeNumber = episodeNumber } } });
        }

        private static string CreateNewAccountPayload(string username, string password, string email)
        {
            return JsonConvert.SerializeObject(new TraktAccount { Username = username, Password = password, Email = email });
        }

        private static string CreateTestAccountPayload(string username, string password)
        {
            return JsonConvert.SerializeObject(new TraktAccount { Username = username, Password = password });
        }

        private static string CreateMovieShoutPayload(string imdbid, string shout)
        {
            return JsonConvert.SerializeObject(new TraktMovieShout { Username = TraktSettings.Username, Password = TraktSettings.Password, IMDBID = imdbid, Shout = shout });
        }

        private static string CreateShowShoutPayload(string tvdbid, string shout)
        {
            return JsonConvert.SerializeObject(new TraktShowShout { Username = TraktSettings.Username, Password = TraktSettings.Password, TVDBID = tvdbid, Shout = shout });
        }

        private static string CreateEpisodeShoutPayload(string tvdbid, int seasonnumber, int episodenumber, string shout)
        {
            return JsonConvert.SerializeObject(new TraktEpisodeShout { Username = TraktSettings.Username, Password = TraktSettings.Password, TVDBID = tvdbid, SeasonNumber = seasonnumber, EpisodeNumber = episodenumber, Shout = shout });
        }

        private static string CreateShowSyncPayload(string tvdbid)
        {
            return JsonConvert.SerializeObject(new TraktShowSync { UserName = TraktSettings.Username, Password = TraktSettings.Password, ShowList = new List<TraktShowSync.Show> { new TraktShowSync.Show { TVDBID = tvdbid } } });
        }

        #endregion
    }
}
