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
            return WebRequestFactory.PostData(new Uri(string.Format(TraktURIs.RateItem, TraktRatingTypes.movie)), parseRatingResponse, CreateRatingPayload(IMDBID, Title, Year, Rating));
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
        #endregion

        #region Creators
        private static string GetUserAuthentication()
        {
            return JsonConvert.SerializeObject(new TraktAuthentication() { Username = TraktSettings.Username, Password = TraktSettings.Password });
        }

        private static string CreateRatingPayload(string IMDBID, string Title, int Year, string Rating)
        {
            return JsonConvert.SerializeObject(new TraktUserRating() { Username = TraktSettings.Username, Password = TraktSettings.Password, IMDBID = IMDBID, Title = Title, Year = Year, Rating = Rating });
        }
        #endregion
    }
}
