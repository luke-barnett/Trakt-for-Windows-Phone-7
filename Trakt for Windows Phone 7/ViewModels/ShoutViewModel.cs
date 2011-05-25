using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using System.Windows;
using System.IO.IsolatedStorage;
using System.Windows.Input;


namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class ShoutViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public ShoutViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            if (!TraktSettings.LoggedIn)
            {
                MessageBox.Show("You need to be logged in to shout");
                navigationService.GoBack();
            }
        }

        public string ItemType { get; set; }

        public string TVDBID { get; set; }

        public string IMDBID { get; set; }

        public int Season { get; set; }

        public int Episode { get; set; }

        public string _Shout;
        public string Shout { get { return _Shout; } set { _Shout = value; NotifyOfPropertyChange("Shout"); } }

        private bool _sendingShout = false;
        public bool sendingShout { get { return _sendingShout; } set { _sendingShout = value; } }

        public void KeyUpOccurred(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                doShout();
        }

        public void parseResponse(TraktResponse response)
        {
            if (response.Status.CompareTo("success") == 0)
            {
                MessageBox.Show("Shout sent successfully");
                navigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Error occurred in sending the shout");
            }
            sendingShout = false;
            NotifyOfPropertyChange("sendingShout");
        }

        public void doShout()
        {
            if (!sendingShout)
            {
                sendingShout = true;
                NotifyOfPropertyChange("sendingShout");
                if (ItemType != null)
                {
                    if (ItemType.CompareTo(TraktTypes.movie.ToString()) == 0)
                    {
                        if (!(string.IsNullOrEmpty(IMDBID) || string.IsNullOrEmpty(Shout)))
                        {
                            TraktAPI.TraktAPI.movieShout(IMDBID, Shout).Subscribe(onNext: response => parseResponse(response), onError: error => handleError(error));
                        }
                    }
                    else if (ItemType.CompareTo(TraktTypes.show.ToString()) == 0)
                    {
                        if (!(string.IsNullOrEmpty(TVDBID) || string.IsNullOrEmpty(Shout)))
                        {
                            TraktAPI.TraktAPI.showShout(TVDBID, Shout).Subscribe(onNext: response => parseResponse(response), onError: error => handleError(error));
                        }
                    }
                    else if (ItemType.CompareTo(TraktTypes.episode.ToString()) == 0)
                    {
                        if (!(string.IsNullOrEmpty(TVDBID) || string.IsNullOrEmpty(Shout)))
                        {
                            TraktAPI.TraktAPI.episodeShout(TVDBID, Season, Episode, Shout).Subscribe(onNext: response => parseResponse(response), onError: error => handleError(error));
                        }
                    }
                    else
                        MessageBox.Show("Error Occurred, Bad Type");
                }
                else
                {
                    MessageBox.Show("Error Occurred, I don't know what I'm shouting about (Type)");
                }
            }
        }
    }
}
