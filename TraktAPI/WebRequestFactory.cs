using System;
using System.Net;
using System.Linq;
using System.IO;
using Microsoft.Phone.Reactive;
using System.Text;

namespace TraktAPI
{
    public class WebRequestFactory
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
            WebClient wc = CreatePostWebClient(uri, postData);
            return (from e in Observable.FromEvent<UploadStringCompletedEventArgs>(wc, "UploadStringCompleted")
                    select generator(e.EventArgs.Result)).ObserveOnDispatcher();
            
        }

        private static WebRequest CreateWebRequest(Uri uri)
        {
            var ret = (HttpWebRequest)WebRequest.Create(uri);
            ret.AllowReadStreamBuffering = false;
            return ret;
        }

        public static WebClient CreatePostWebClient(Uri uri, string postData)
        {
            var wc = new WebClient();
            wc.AllowReadStreamBuffering = false;
            wc.UploadStringAsync(uri, postData);
            return wc;
        }
    }
}