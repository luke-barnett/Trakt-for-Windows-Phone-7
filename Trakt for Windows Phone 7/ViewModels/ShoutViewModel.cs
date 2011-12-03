using System.Diagnostics;
using System.Windows.Input;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Trakt_for_Windows_Phone_7.Framework;
using Microsoft.Phone.Reactive;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class ShoutViewModel : BaseViewModel
    {
        private string _shoutText = string.Empty;
        private const string Language = "English";

        public ShoutViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container) : base(navigationService, windowManager, container)
        {
        }

        /// <summary>
        /// The inputted text for the shout
        /// </summary>
        public string ShoutText { get { return _shoutText; } set { _shoutText = value; NotifyOfPropertyChange(() => ShoutText); NotifyOfPropertyChange(() => RemaingCharacters); } }

        /// <summary>
        /// The maximum length of the shout
        /// </summary>
        public int MaxShoutLength { get { return 1000; } }

        /// <summary>
        /// The amount of characters remaining
        /// </summary>
        public string RemaingCharacters { get { return string.Format("[{0}/{1}]", ShoutText.Length, MaxShoutLength); } }

        /// <summary>
        /// The language text to show
        /// </summary>
        public string ShoutLanguageText { get { return string.Format("Please write your shout in {0}", Language); } }

        /// <summary>
        /// If the submit button is enabled
        /// </summary>
        public bool SubmitEnabled { get { return !ProgressBarVisible; } }

        /// <summary>
        /// The library type for the shout
        /// </summary>
        public TraktLibraryTypes LibraryType { get; set; }

        /// <summary>
        /// The IMDBID value to reference for movies
        /// </summary>
        public string IMDBID { get; set; }

        /// <summary>
        /// The TVDBID value to reference for shows
        /// </summary>
        public string TVDBID { get; set; }

        /// <summary>
        /// The season value to reference for episodes
        /// </summary>
        public int Season { get; set; }

        /// <summary>
        /// The episode value to reference for episodes
        /// </summary>
        public int Episode { get; set; }


        /// <summary>
        /// Fired when a key is entered on the shout dialog
        /// </summary>
        /// <param name="args">The key event arguments</param>
        public void KeyEntered(KeyEventArgs args)
        {
            NotifyOfPropertyChange(() => RemaingCharacters);
            if(args.Key == Key.Enter)
                SubmitShout();
        }

        /// <summary>
        /// Submits a shout to trakt is enough data is available
        /// </summary>
        public void SubmitShout()
        {
            NotifyOfPropertyChange(() => SubmitEnabled);
            Debug.WriteLine("Shout type identified as {0}", LibraryType.ToString());

            ProgressBarVisible = true;
            if (string.IsNullOrEmpty(ShoutText) && ShoutText.Length == 0)
            {
                Debug.WriteLine("Shout is empty!");
                CloseView(null);
            }

            if(!TraktSettings.LoginStatus.IsLoggedIn)
            {
                Debug.WriteLine("Not logged in");
                CloseView(null);
            }

            switch (LibraryType)
            {
                case TraktLibraryTypes.movies:
                    if(!string.IsNullOrEmpty(IMDBID))
                    {
                        TraktAPI.TraktAPI.MovieShout(IMDBID, ShoutText).Subscribe(CloseView, HandleError);
                    }
                    else
                    {
                        Debug.WriteLine("IMDBID Missing");
                        CloseView(null);
                    }
                    break;
                case TraktLibraryTypes.shows:
                    if(!string.IsNullOrEmpty(TVDBID))
                    {
                        TraktAPI.TraktAPI.ShowShout(TVDBID, ShoutText).Subscribe(CloseView, HandleError);
                    }
                    else
                    {
                        Debug.WriteLine("TVDBID Missing");
                        CloseView(null);
                    }
                    break;
                case TraktLibraryTypes.episodes:
                    if(!string.IsNullOrEmpty(TVDBID) && Season >= 0 && Episode >= 0)
                    {
                        TraktAPI.TraktAPI.EpisodeShout(TVDBID, Season, Episode, ShoutText).Subscribe(CloseView, HandleError);
                    }
                    else
                    {
                        Debug.WriteLine("Episode details missing");
                        CloseView(null);
                    }
                    break;
            }  
        }

        /// <summary>
        /// Closes the view
        /// </summary>
        /// <param name="response">The response recieved</param>
        private void CloseView(TraktResponse response)
        {
            ProgressBarVisible = false;
            Debug.WriteLine("Closing shout view");
            TryClose();
        }
    }
}
