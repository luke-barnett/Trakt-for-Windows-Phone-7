using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Reactive;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Trakt_for_Windows_Phone_7.Framework;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class ShowViewModel : BaseViewModel
    {
        #region Private Parameters

        private string _tvdbid;
        private bool _showMainPivot;
        private ImageSource _showPoster;
        private TraktShow _show;
        private List<TraktShout> _shouts;
        private readonly ShoutViewModel _shoutViewModel;

        #endregion

        public ShowViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container, ShoutViewModel shoutViewModel) : base(navigationService, windowManager, container)
        {
            _shoutViewModel = shoutViewModel;
            _shoutViewModel.TVDBID = TVDBID;
            _shoutViewModel.LibraryType = TraktLibraryTypes.shows;
            _shouts = new List<TraktShout>();
            ShowPoster = DefaultPoster;
            ShowMainPivot = false;
        }

        #region Public Parameters

        public string TVDBID { get { return _tvdbid; } set { _tvdbid = value; NotifyOfPropertyChange(() => TVDBID); GetShowDetails(); } }

        public bool ShowMainPivot { get { return _showMainPivot; } set { _showMainPivot = value; NotifyOfPropertyChange(() => MainPivotVisibility); } }

        public Visibility MainPivotVisibility { get { return (ShowMainPivot) ? Visibility.Visible : Visibility.Collapsed; } }

        public ImageSource ShowPoster { get { return _showPoster; } set { _showPoster = value; NotifyOfPropertyChange(() => ShowPoster); } }

        public TraktShow Show { get { return _show; } set { _show = value; NotifyOfPropertyChange(() => Show); UpdateDetails(); } }

        #endregion

        #region Private Methods

        private void GetShowDetails()
        {
            Debug.WriteLine("Getting show details");
            ProgressBarVisible = true;
            TraktAPI.TraktAPI.GetShow(TVDBID).Subscribe(HandleShow, HandleError);
        }

        private void HandleShow(TraktShow show)
        {
            Debug.WriteLine("Getting the poster");
            var poster = new BitmapImage(new Uri(show.Images.Poster)) {CreateOptions = BitmapCreateOptions.None};

            ProgressBarVisible = true;
            poster.ImageOpened += (sender, args) => { ShowPoster = poster; ProgressBarVisible = false; Debug.WriteLine("Got poster successfully"); };
            poster.ImageFailed += (sender, args) => { ProgressBarVisible = false; Debug.WriteLine("Failed to get poster"); };

            Show = show;

            ProgressBarVisible = false;
            ShowMainPivot = true;
        }

        private void UpdateDetails()
        {
            
        }

        #endregion

        #region Public Methods
        #endregion
    }
}
