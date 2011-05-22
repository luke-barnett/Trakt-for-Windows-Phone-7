using TraktAPI.TraktModels;
using System.Net;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TraktAPI
{
    public static class TraktAPI
    {
        #region Enumerables

        public enum TraktLibraryTypes
        {
            movies,
            shows
        }

        #endregion

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
        #endregion

        #region Creators
        private static string GetUserAuthentication()
        {
            return JsonConvert.SerializeObject(new TraktAuthentication() { Username = TraktSettings.Username, Password = TraktSettings.Password });
        }
        #endregion
    }
}
