using System.Windows;
using Caliburn.Micro;
using Trakt_for_Windows_Phone_7.Framework;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class ShowViewModel : BaseViewModel
    {
        #region Private Parameters

        private string _tvdbid;
        private bool _showMainPivot;

        #endregion

        public ShowViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container) : base(navigationService, windowManager, container)
        {
            ShowMainPivot = true;
        }

        #region Public Parameters

        public string TVDBID { get { return _tvdbid; } set { _tvdbid = value; NotifyOfPropertyChange(() => TVDBID); } }

        public bool ShowMainPivot { get { return _showMainPivot; } set { _showMainPivot = value; NotifyOfPropertyChange(() => MainPivotVisibility); } }

        public Visibility MainPivotVisibility { get { return (ShowMainPivot) ? Visibility.Visible : Visibility.Collapsed; } }

        #endregion

        #region Private Methods
        #endregion

        #region Public Methods
        #endregion
    }
}
