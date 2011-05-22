using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class BaseViewModel : PropertyChangedBase
    {
        public string ApplicationName { get { return "Trakt 7"; } }
    }
}
