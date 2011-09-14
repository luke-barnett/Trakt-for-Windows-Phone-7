using System.Windows;
using Caliburn.Micro;
using TraktAPI;
using Trakt_for_Windows_Phone_7.Framework;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class RecommendationsViewModel : BaseViewModel
    {
        public RecommendationsViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container) : base(navigationService, windowManager, container)
        {
            if(!TraktSettings.LoggedIn)
            {
                MessageBox.Show("Need to be logged in for this feature");
                NavigationService.GoBack();
            }
        }
    }
}
