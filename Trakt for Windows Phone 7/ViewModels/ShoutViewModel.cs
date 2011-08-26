using System.Windows.Input;
using Caliburn.Micro;
using Trakt_for_Windows_Phone_7.Framework;

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

        public void KeyEntered(KeyEventArgs args)
        {
            NotifyOfPropertyChange(() => RemaingCharacters);
            if(args.Key == Key.Enter)
                SubmitShout();
        }

        public void SubmitShout()
        {
            TryClose();
        }
    }
}
