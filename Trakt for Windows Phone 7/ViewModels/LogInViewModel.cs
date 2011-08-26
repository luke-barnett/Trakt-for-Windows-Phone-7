using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Trakt_for_Windows_Phone_7.Framework;
using Microsoft.Phone.Reactive;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class LogInViewModel : BaseViewModel
    {
        private bool _showDefaultView = true;
        private string _username;
        private string _password;
        private string _email;

        public LogInViewModel(INavigationService navigationService, IWindowManager windowManager, PhoneContainer container) : base(navigationService, windowManager, container)
        {
            Username = TraktSettings.Username;
        }

        public bool Ready { get { return !ProgressBarVisible; } }

        public bool ShowDefaultView { get { return _showDefaultView; } set { _showDefaultView = value; NotifyOfPropertyChange(() => DefaultView); NotifyOfPropertyChange(() => RegistrationView); } }

        public Visibility DefaultView { get { return (ShowDefaultView) ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility RegistrationView { get { return (!ShowDefaultView) ? Visibility.Visible : Visibility.Collapsed; } }

        public string Username { get { return _username; } set { _username = value; NotifyOfPropertyChange(() => Username); } }

        public string Password { get { return _password; } set { _password = value; NotifyOfPropertyChange(() => Password); } }

        public string Email { get { return _email; } set { _email = value; NotifyOfPropertyChange(() => Email); } }

        public void NewAccount()
        {
            ShowDefaultView = false;
        }

        public void LogIn()
        {
            ProgressBarVisible = true;
            NotifyOfPropertyChange(() => Ready);
            TraktAPI.TraktAPI.TestAccount(Username, CalculateSHA1(Password, Encoding.UTF8)).Subscribe(AccountSuccess, AccountFailed);
        }

        public void Back()
        {
            ShowDefaultView = true;
        }

        public void Register()
        {
            if(String.IsNullOrEmpty(Username))
            {
                MessageBox.Show("Username cannot be empty");
                return;
            }

            if (String.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Password cannot be empty");
                return;
            }

            if (String.IsNullOrEmpty(Email))
            {
                MessageBox.Show("Email cannot be empty");
                return;
            }

            ProgressBarVisible = true;
            NotifyOfPropertyChange(() => Ready);
            TraktAPI.TraktAPI.CreateAccount(Username, CalculateSHA1(Password, Encoding.UTF8), Email).Subscribe(AccountSuccess, AccountFailed);
        }

        public void AccountSuccess(TraktResponse response)
        {
            TraktSettings.Username = Username;
            TraktSettings.Password = CalculateSHA1(Password, Encoding.UTF8);
            TraktSettings.LoggedIn = true;
            ProgressBarVisible = false;
            (Container.GetInstance(typeof (MainPageViewModel), "MainPageViewModel") as MainPageViewModel).SetUpApplicationBar();
            TryClose();
        }

        public void AccountFailed(Exception exception)
        {
            TraktSettings.LoggedIn = false;
            TraktSettings.Password = String.Empty;
            ProgressBarVisible = false;
            TryClose();
        }

        private static string CalculateSHA1(string text, Encoding enc)
        {
            var buffer = enc.GetBytes(text);
            var hashData = new SHA1Managed().ComputeHash(buffer);

            var hash = string.Empty;

            foreach (var b in hashData)
                hash += b.ToString("X2");

            System.Diagnostics.Debug.WriteLine(hash);
            return hash;
        }
    }
}
