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

        public string ShoutText { get { return _shoutText; } set { _shoutText = value; NotifyOfPropertyChange(() => ShoutText); NotifyOfPropertyChange(() => RemaingCharacters); } }

        public int MaxShoutLength { get { return 1000; } }

        public string RemaingCharacters { get { return string.Format("{0} characters remaining", MaxShoutLength - ShoutText.Length); } }

        public string ShoutLanguageText { get { return string.Format("Please write your shout in {0}", Language); } }

        public bool SubmitEnabled { get { return !ProgressBarVisible; } }

        public TraktLibraryTypes LibraryType { get; set; }

        public string IMDBID { get; set; }

        public string TVDBID { get; set; }

        public int Season { get; set; }

        public int Episode { get; set; }


        public void KeyEntered(KeyEventArgs args)
        {
            NotifyOfPropertyChange(() => RemaingCharacters);
            if(args.Key == Key.Enter)
                SubmitShout();
        }

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

            if(!TraktSettings.LoggedIn)
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

        private void CloseView(TraktResponse response)
        {
            ProgressBarVisible = false;
            Debug.WriteLine("Closing shout view");
            TryClose();
        }
    }
}
