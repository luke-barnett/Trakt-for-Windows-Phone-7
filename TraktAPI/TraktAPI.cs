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
        shows
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

    public enum TraktRatingTypes
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
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Trending, TraktLibraryTypes.movies)), parseTrendingMovies);
        }

        public static IObservable<TraktResponse> testAccount()
        {
            return WebRequestFactory.PostData(new Uri(TraktURIs.TestAccount), parseTraktResponse, GetUserAuthentication());
        }

        public static IObservable<TraktShow[]> getTrendingShows()
        {
            return WebRequestFactory.GetData(new Uri(string.Format(TraktURIs.Trending, TraktLibraryTypes.shows)), parseTrendingShows);
        }

        public static IObservable<TraktMovie> getMovie(string movieTitle)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.MovieDetails, TraktDetailsTypes.summary, movieTitle)), parseMovie, GetUserAuthentication());
        }

        public static IObservable<TraktRateResponse> rateMovie(string IMDBID, string Title, int Year, string Rating)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.RateItem, TraktRatingTypes.movie)), parseRatingResponse, CreateMovieRatingPayload(IMDBID, Title, Year, Rating));
        }

        public static IObservable<TraktResponse> syncMovie(string IMDID, string Title, int Year, string TraktSyncMode)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.SyncMovieLibrary, TraktSyncMode)), parseTraktResponse, CreateMovieSyncPayload(IMDID, Title, Year));
        }

        public static IObservable<TraktResponse> watchMovie(string IMDID, string Title, int Year)
        {
            return syncMovie(IMDID, Title, Year, TraktSyncModes.seen.ToString());
        }

        public static IObservable<TraktResponse> unwatchMovie(string IMDID, string Title, int Year)
        {
            return syncMovie(IMDID, Title, Year, TraktSyncModes.unseen.ToString());
        }

        public static IObservable<TraktResponse> watchListMovie(string IMDID, string Title, int Year)
        {
            return syncMovie(IMDID, Title, Year, TraktSyncModes.watchlist.ToString());
        }

        public static IObservable<TraktResponse> unwatchListMovie(string IMDID, string Title, int Year)
        {
            return syncMovie(IMDID, Title, Year, TraktSyncModes.unwatchlist.ToString());
        }

        public static IObservable<TraktShow> getShow(string tvdbid)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.ShowDetails, TraktDetailsTypes.summary, tvdbid)), parseShow, GetUserAuthentication());
        }

        public static IObservable<TraktRateResponse> rateShow(string TVDBID, string IMDBID, string Title, int Year, string Rating)
        {
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.RateItem, TraktRatingTypes.show)), parseRatingResponse, CreateShowRatingPayload(TVDBID, IMDBID, Title, Year, Rating));
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
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.RateItem, TraktRatingTypes.episode)), parseRatingResponse, CreateEpisodeRatingPayload(TVDBID, IMDBID, Title, Year, SeasonNumber, EpisodeNumber, Rating));
        }

        #endregion

        #region Parsers
        private static TraktMovie[] parseTrendingMovies(string json)
        {
            return JsonConvert.DeserializeObject<TraktMovie[]>(json);
        }

        private static TraktShow[] parseTrendingShows(string json)
        {
            return JsonConvert.DeserializeObject<TraktShow[]>(json);
        }
        
        private static TraktResponse parseTraktResponse(string json)
        {
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
        #endregion

        #region Creators
        private static string GetUserAuthentication()
        {
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
            return JsonConvert.SerializeObject(new TraktUserEpisodeRating() { Username = TraktSettings.Username, Password = TraktSettings.Password, TVDBID = TVDBID, IMDBID = IMDBID, Title = Title, Year = Year, EpisodeNumber = EpisodeNumber, SeasonNumber = SeasonNumber, Rating = Rating }); ;
        }

        #endregion
    }
}
