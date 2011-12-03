using System;
using System.Net;
using System.Linq;
using System.IO;
using Microsoft.Phone.Reactive;
using System.Text;
using System.ComponentModel;

namespace TraktAPI
{
    public static class WebRequestFactory
    {
        public static IObservable<T> GetData<T>(Uri uri, Func<string, T> generator)
        {
            System.Diagnostics.Debug.WriteLine(uri);
            return (from request in Observable.Return(CreateWebRequest(uri))
                    from response in Observable.FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse)()
                    select generator(new StreamReader(response.GetResponseStream()).ReadToEnd())).ObserveOnDispatcher();
        }

        public static IObservable<T> PostData<T>(Uri uri, Func<string, T> generator, String postData)
        {
            System.Diagnostics.Debug.WriteLine(uri);
            var request = WebRequest.CreateHttp(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var fetchRequestStream = Observable.FromAsyncPattern<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream);
            var fetchResponse = Observable.FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse);

            return fetchRequestStream().SelectMany(stream =>
                                                       {
                                                           using (var writer = new StreamWriter(stream))
                                                               writer.Write(postData);
                                                           return fetchResponse();
                                                       }).Select(result =>
                                                                     {
                                                                         var response = (HttpWebResponse) result;
                                                                         return generator(
                                                                             new StreamReader(
                                                                                 response.GetResponseStream()).ReadToEnd
                                                                                 ());
                                                                     }).ObserveOnDispatcher();
        }

        private static WebRequest CreateWebRequest(Uri uri)
        {
            var ret = (HttpWebRequest)WebRequest.Create(uri);
            ret.AllowReadStreamBuffering = false;
            return ret;
        }

        public static WebClient CreatePostWebClient(Uri uri)
        {
            var wc = new WebClient();
            wc.AllowReadStreamBuffering = false;
            return wc;
        }

        public static IObservable<IEvent<T>> ThrowIfError<T>(this IObservable<IEvent<T>> observable)
            where T : AsyncCompletedEventArgs
        {
            return observable.SelectMany(
                o =>
                {
                    if (o.EventArgs.Error != null)
                    {
                        throw o.EventArgs.Error;
                    }

                    return Observable.Return(o).ThrowIfError();
                });
        }
    }
}