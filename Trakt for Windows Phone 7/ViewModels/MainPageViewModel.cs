using Caliburn.Micro;
using System.IO.IsolatedStorage;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private IsolatedStorageSettings _userSettings;

        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _userSettings = IsolatedStorageSettings.ApplicationSettings;
        }
    }
}