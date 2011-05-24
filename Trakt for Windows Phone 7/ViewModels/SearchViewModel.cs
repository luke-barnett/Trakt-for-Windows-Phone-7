using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using System.Windows;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public SearchViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        private string _SearchString;
        public string SearchString { get { return _SearchString; } set { _SearchString = value; StartSearch(); } }

        private void StartSearch()
        {
            NotifyOfPropertyChange("SearchString");
            NotifyOfPropertyChange("WhatWeAreSearchingFor");
        }

        public string WhatWeAreSearchingFor
        {
            get
            {
                if (SearchString == null)
                {
                    return "";
                }
                else
                    return string.Format("Searching for \"{0}\"", SearchString);
            }
        }
    }
}

