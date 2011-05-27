using TraktAPI.TraktModels;
using System.Net;
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

        public static IObservable<TraktMovie[]> getTrendingMovies()
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Trending, TraktLibraryTypes.movies)), parseMovieArray);
        }

        public static IObservable<TraktResponse> testAccount()
        {
            return WebRequestFactory.PostData(new Uri(TraktURIs.TestAccount), parseTraktResponse, GetUserAuthentication());
        }

        public static IObservable<TraktShow[]> getTrendingShows()
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Trending, TraktLibraryTypes.shows)), parseShowArray);
        }

        public static IObservable<TraktMovie> getMovie(string movieTitle)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.MovieDetails, TraktDetailsTypes.summary, movieTitle)), parseMovie, GetUserAuthentication());
        }

        public static IObservable<TraktRateResponse> rateMovie(string IMDBID, string Title, int Year, string Rating)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.RateItem, TraktTypes.movie)), parseRatingResponse, CreateMovieRatingPayload(IMDBID, Title, Year, Rating));
        }

        private static IObservable<TraktResponse> syncMovie(string IMDBID, string Title, int Year, string TraktSyncMode)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.SyncMovieLibrary, TraktSyncMode)), parseTraktResponse, CreateMovieSyncPayload(IMDBID, Title, Year));
        }

        public static IObservable<TraktResponse> watchMovie(string IMDBID, string Title, int Year)
        {
            return syncMovie(IMDBID, Title, Year, TraktSyncModes.seen.ToString());
        }

        public static IObservable<TraktResponse> unwatchMovie(string IMDBID, string Title, int Year)
        {
            return syncMovie(IMDBID, Title, Year, TraktSyncModes.unseen.ToString());
        }

        public static IObservable<TraktResponse> watchListMovie(string IMDBID, string Title, int Year)
        {
            return syncMovie(IMDBID, Title, Year, TraktSyncModes.watchlist.ToString());
        }

        public static IObservable<TraktResponse> unwatchListMovie(string IMDBID, string Title, int Year)
        {
            return syncMovie(IMDBID, Title, Year, TraktSyncModes.unwatchlist.ToString());
        }

        public static IObservable<TraktShow> getShow(string tvdbid)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.ShowDetails, TraktDetailsTypes.summary, tvdbid)), parseShow, GetUserAuthentication());
        }

        public static IObservable<TraktRateResponse> rateShow(string TVDBID, string IMDBID, string Title, int Year, string Rating)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.RateItem, TraktTypes.show)), parseRatingResponse, CreateShowRatingPayload(TVDBID, IMDBID, Title, Year, Rating));
        }

        public static IObservable<TraktSeasonInfo[]> getSeasonInfo(string tvdbid)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.SeasonInfo, tvdbid)), parseSeasonInfo);
        }

        public static IObservable<TraktEpisode[]> getSeason(string tvdbid, string seasonNumber)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Season, tvdbid, seasonNumber)), parseSeason, GetUserAuthentication());
        }

        public static IObservable<TraktEpisodeSummary> getEpisodeSummary(string tvdbid, string seasonNumber, string episodeNumber)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.EpisodeDetails, TraktDetailsTypes.summary, tvdbid, seasonNumber, episodeNumber)), parseEpisodeSummary, GetUserAuthentication());
        }

        public static IObservable<TraktRateResponse> rateEpisode(string TVDBID, string IMDBID, string Title, int Year, string SeasonNumber, string EpisodeNumber, string Rating)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.RateItem, TraktTypes.episode)), parseRatingResponse, CreateEpisodeRatingPayload(TVDBID, IMDBID, Title, Year, SeasonNumber, EpisodeNumber, Rating));
        }

        private static IObservable<TraktResponse> syncEpisode(string TVDBID, string IMDBID, string Title, int Year, string SeasonNumber, string EpisodeNumber, string TraktSyncMode)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.SyncEpisodeLibrary, TraktSyncMode)), parseTraktResponse, CreateEpisodeSyncPayload(TVDBID, IMDBID, Title, Year, SeasonNumber, EpisodeNumber));
        }

        public static IObservable<TraktResponse> watchEpisode(string TVDBID, string IMDBID, string Title, int Year, string SeasonNumber, string EpisodeNumber)
        {
            return syncEpisode(TVDBID, IMDBID, Title, Year, SeasonNumber, EpisodeNumber, TraktSyncModes.seen.ToString());
        }

        public static IObservable<TraktResponse> unwatchEpisode(string TVDBID, string IMDBID, string Title, int Year, string SeasonNumber, string EpisodeNumber)
        {
            return syncEpisode(TVDBID, IMDBID, Title, Year, SeasonNumber, EpisodeNumber, TraktSyncModes.unseen.ToString());
        }

        public static IObservable<TraktResponse> watchListEpisode(string TVDBID, string IMDBID, string Title, int Year, string SeasonNumber, string EpisodeNumber)
        {
            return syncEpisode(TVDBID, IMDBID, Title, Year, SeasonNumber, EpisodeNumber, TraktSyncModes.watchlist.ToString());
        }

        public static IObservable<TraktResponse> unwatchListEpisode(string TVDBID, string IMDBID, string Title, int Year, string SeasonNumber, string EpisodeNumber)
        {
            return syncEpisode(TVDBID, IMDBID, Title, Year, SeasonNumber, EpisodeNumber, TraktSyncModes.unwatchlist.ToString());
        }

        public static IObservable<TraktResponse> createAccount(string Username, string Password, string Email)
        {
            return WebRequestFactory.PostData(new Uri(TraktURIs.CreateAccount), parseTraktResponse, CreateNewAccountPayload(Username, Password, Email));
        }

        public static IObservable<TraktResponse> testAccount(string Username, string Password)
        {
            return WebRequestFactory.PostData(new Uri(TraktURIs.TestAccount), parseTraktResponse, CreateTestAccountPayload(Username, Password));
        }

        public static IObservable<TraktMovie[]> searchMovies(string SearchQuery)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Search, TraktLibraryTypes.movies, SearchQuery)), parseMovieArray);
        }

        public static IObservable<TraktShow[]> searchShows(string SearchQuery)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Search, TraktLibraryTypes.shows, SearchQuery)), parseShowArray);
        }

        public static IObservable<TraktEpisodeSummary[]> searchEpisodes(string SearchQuery)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Search, TraktLibraryTypes.episodes, SearchQuery)), parseEpisodeSummaryArray);
        }

        public static IObservable<TraktMovie[]> getMovieRecommendations()
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Recommendations, TraktLibraryTypes.movies)), parseMovieArray, GetUserAuthentication());
        }

        public static IObservable<TraktShow[]> getShowRecommendations()
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Recommendations, TraktLibraryTypes.shows)), parseShowArray, GetUserAuthentication());
        }

        public static IObservable<TraktMovie[]> getMovieWatchList(string Username)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.WatchList, TraktLibraryTypes.movies, Username)), parseMovieArray, GetUserAuthentication());
        }

        public static IObservable<TraktShow[]> getShowWatchList(string Username)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.WatchList, TraktLibraryTypes.shows, Username)), parseShowArray, GetUserAuthentication());
        }

        public static IObservable<TraktEpisode[]> getAndExtractEpisodeWatchList(string Username)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.WatchList, TraktLibraryTypes.episodes, Username)), parseAndExtractEpisodeWatchList, GetUserAuthentication());
        }

        public static IObservable<TraktShout[]> getShowShouts(string TVDBID)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.ShowSummary, TraktDetailsTypes.shouts, TVDBID)), parseTraktShoutArray);
        }

        public static IObservable<TraktShout[]> getMovieShouts(string IMDBID)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.MovieDetails, TraktDetailsTypes.shouts, IMDBID)), parseTraktShoutArray);
        }

        public static IObservable<TraktShout[]> getEpisodeShouts(string tvdbid, string seasonNumber, string episodeNumber)
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.EpisodeDetails, TraktDetailsTypes.shouts, tvdbid, seasonNumber, episodeNumber)), parseTraktShoutArray);
        }

        public static IObservable<TraktResponse> movieShout(string imdbid, string shout)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Shout, TraktTypes.movie)), parseTraktResponse, CreateMovieShoutPayload(imdbid, shout));
        }

        public static IObservable<TraktResponse> showShout(string tvdbid, string shout)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Shout, TraktTypes.show)), parseTraktResponse, CreateShowShoutPayload(tvdbid, shout));
        }

        public static IObservable<TraktResponse> episodeShout(string tvdbid, int seasonnumber, int episodenumber, string shout)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.Shout, TraktTypes.episode)), parseTraktResponse, CreateEpisodeShoutPayload(tvdbid, seasonnumber, episodenumber, shout));
        }

        private static IObservable<TraktResponse> syncShow(string tvdbid, string traktSyncMode)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.SyncShow, traktSyncMode)), parseTraktResponse, CreateShowSyncPayload(tvdbid));
        }

        public static IObservable<TraktResponse> watchListShow(string tvdbid)
        {
            return syncShow(tvdbid, TraktSyncModes.watchlist.ToString());
        }

        public static IObservable<TraktResponse> unwatchListShow(string tvdbid)
        {
            return syncShow(tvdbid, TraktSyncModes.unwatchlist.ToString());
        }

        #endregion

        #region Parsers
        private static TraktMovie[] parseMovieArray(string json)
        {
            return JsonConvert.DeserializeObject<TraktMovie[]>(json);
        }

        private static TraktShow[] parseShowArray(string json)
        {
            return JsonConvert.DeserializeObject<TraktShow[]>(json);
        }
        
        private static TraktResponse parseTraktResponse(string json)
        {
            System.Diagnostics.Debug.WriteLine(json);
            return JsonConvert.DeserializeObject<TraktResponse>(json);
        }

        private static TraktMovie parseMovie(string json)
        {
            return JsonConvert.DeserializeObject<TraktMovie>(json);
        }

        private static TraktRateResponse parseRatingResponse(string json)
        {
            System.Diagnostics.Debug.WriteLine(json);
            return JsonConvert.DeserializeObject<TraktRateResponse>(json);
        }

        private static TraktShow parseShow(string json)
        {
            return JsonConvert.DeserializeObject<TraktShow>(json);
        }

        private static TraktSeasonInfo[] parseSeasonInfo(string json)
        {
            return JsonConvert.DeserializeObject<TraktSeasonInfo[]>(json);
        }

        private static TraktEpisode[] parseSeason(string json)
        {
            return JsonConvert.DeserializeObject<TraktEpisode[]>(json);
        }

        private static TraktEpisodeSummary parseEpisodeSummary(string json)
        {
            return JsonConvert.DeserializeObject<TraktEpisodeSummary>(json);
        }

        private static TraktEpisodeSummary[] parseEpisodeSummaryArray(string json)
        {
            return JsonConvert.DeserializeObject<TraktEpisodeSummary[]>(json);
        }

        private static TraktEpisode[] parseAndExtractEpisodeWatchList(string json)
        {
            TraktEpisodeWatchList[] asWatchListItems = JsonConvert.DeserializeObject<TraktEpisodeWatchList[]>(json);
            List<TraktEpisode> asEpisodes = new List<TraktEpisode>();

            foreach (TraktEpisodeWatchList watchList in asWatchListItems)
                foreach (TraktEpisode episode in watchList.Episodes)
                {
                    episode.ShowTVDBID = watchList.TVDBID;
                    asEpisodes.Add(episode);
                }
            
            return asEpisodes.ToArray();
        }

        private static TraktShout[] parseTraktShoutArray(string json)
        {
            return JsonConvert.DeserializeObject<TraktShout[]>(json);
        }

        #endregion

        #region Creators
        private static string GetUserAuthentication()
        {
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(new TraktAuthentication() { Username = TraktSettings.Username, Password = TraktSettings.Password }));
            return JsonConvert.SerializeObject(new TraktAuthentication() { Username = TraktSettings.Username, Password = TraktSettings.Password });
        }

        private static string CreateMovieRatingPayload(string IMDBID, string Title, int Year, string Rating)
        {
            return JsonConvert.SerializeObject(new TraktUserMovieRating() { Username = TraktSettings.Username, Password = TraktSettings.Password, IMDBID = IMDBID, Title = Title, Year = Year, Rating = Rating });
        }

        private static string CreateMovieSyncPayload(string IMDBID, string Title, int Year)
        {
            return JsonConvert.SerializeObject(new TraktMovieSync() { UserName = TraktSettings.Username, Password = TraktSettings.Password, MovieList = new List<TraktMovieSync.Movie>(){ new TraktMovieSync.Movie(){ IMDBID = IMDBID, Title = Title, Year = Year }}});
        }

        private static string CreateShowRatingPayload(string TVDBID, string IMDBID, string Title, int Year, string Rating)
        {
            return JsonConvert.SerializeObject(new TraktUserShowRating() { Username = TraktSettings.Username, Password = TraktSettings.Password, TVDBID = TVDBID, IMDBID = IMDBID, Title = Title, Year = Year, Rating = Rating });
        }

        private static string CreateEpisodeRatingPayload(string TVDBID, string IMDBID, string Title, int Year, string SeasonNumber, string EpisodeNumber, string Rating)
        {
            return JsonConvert.SerializeObject(new TraktUserEpisodeRating() { Username = TraktSettings.Username, Password = TraktSettings.Password, TVDBID = TVDBID, IMDBID = IMDBID, Title = Title, Year = Year, EpisodeNumber = EpisodeNumber, SeasonNumber = SeasonNumber, Rating = Rating });
        }

        private static string CreateEpisodeSyncPayload(string TVDBID, string IMDBID, string Title, int Year, string SeasonNumber, string EpisodeNumber)
        {
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(new TraktEpisodeSync() { UserName = TraktSettings.Username, Password = TraktSettings.Password, IMDBID = IMDBID, TVDBID = TVDBID, Title = Title, Year = Year, EpisodeList = new List<TraktEpisodeSync.Episode>() { new TraktEpisodeSync.Episode() { SeasonNumber = SeasonNumber, EpisodeNumber = EpisodeNumber } } }));
            return JsonConvert.SerializeObject(new TraktEpisodeSync() { UserName = TraktSettings.Username, Password = TraktSettings.Password, IMDBID = IMDBID, TVDBID = TVDBID, Title = Title, Year = Year, EpisodeList = new List<TraktEpisodeSync.Episode>() { new TraktEpisodeSync.Episode() { SeasonNumber = SeasonNumber, EpisodeNumber = EpisodeNumber } } });
        }

        private static string CreateNewAccountPayload(string Username, string Password, string Email)
        {
            return JsonConvert.SerializeObject(new TraktAccount() { Username = Username, Password = Password, Email = Email });
        }

        private static string CreateTestAccountPayload(string Username, string Password)
        {
            return JsonConvert.SerializeObject(new TraktAccount() { Username = Username, Password = Password });
        }

        private static string CreateMovieShoutPayload(string imdbid, string shout)
        {
            return JsonConvert.SerializeObject(new TraktMovieShout() { Username = TraktSettings.Username, Password = TraktSettings.Password, IMDBID = imdbid, Shout = shout });
        }

        private static string CreateShowShoutPayload(string tvdbid, string shout)
        {
            return JsonConvert.SerializeObject(new TraktShowShout() { Username = TraktSettings.Username, Password = TraktSettings.Password, TVDBID = tvdbid, Shout = shout });
        }

        private static string CreateEpisodeShoutPayload(string tvdbid, int seasonnumber, int episodenumber, string shout)
        {
            return JsonConvert.SerializeObject(new TraktEpisodeShout() { Username = TraktSettings.Username, Password = TraktSettings.Password, TVDBID = tvdbid, SeasonNumber = seasonnumber, EpisodeNumber = episodenumber, Shout = shout });
        }

        private static string CreateShowSyncPayload(string tvdbid)
        {
            return JsonConvert.SerializeObject(new TraktShowSync() { UserName = TraktSettings.Username, Password = TraktSettings.Password, ShowList = new List<TraktShowSync.Show>() { new TraktShowSync.Show() { TVDBID = tvdbid } } });
        }

        #endregion
    }
}
