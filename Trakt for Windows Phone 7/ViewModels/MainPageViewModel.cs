namespace Trakt_for_Windows_Phone_7.ViewModels
{
    using System;
    using Caliburn.Micro;

    public class MainPageViewModel
    {
        readonly INavigationService navigationService;

        public MainPageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

    }
}