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

        /// <summary>
        /// If input is ready for entering or not
        /// </summary>
        public bool Ready { get { return !ProgressBarVisible; } }

        /// <summary>
        /// Whether or not to show the default view
        /// </summary>
        public bool ShowDefaultView { get { return _showDefaultView; } set { _showDefaultView = value; NotifyOfPropertyChange(() => DefaultView); NotifyOfPropertyChange(() => RegistrationView); } }

        /// <summary>
        /// The visibility of the default view
        /// </summary>
        public Visibility DefaultView { get { return (ShowDefaultView) ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// The visibility of the registration view
        /// </summary>
        public Visibility RegistrationView { get { return (!ShowDefaultView) ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// The username entered
        /// </summary>
        public string Username { get { return _username; } set { _username = value; NotifyOfPropertyChange(() => Username); } }

        /// <summary>
        /// The password entered
        /// </summary>
        public string Password { get { return _password; } set { _password = value; NotifyOfPropertyChange(() => Password); } }

        /// <summary>
        /// The email entered
        /// </summary>
        public string Email { get { return _email; } set { _email = value; NotifyOfPropertyChange(() => Email); } }

        /// <summary>
        /// Changes the view to registration
        /// </summary>
        public void NewAccount()
        {
            ShowDefaultView = false;
        }

        /// <summary>
        /// Attempts to log in
        /// </summary>
        public void LogIn()
        {
            ProgressBarVisible = true;
            NotifyOfPropertyChange(() => Ready);
            TraktAPI.TraktAPI.TestAccount(Username, CalculateSHA1(Password, Encoding.UTF8)).Subscribe(AccountSuccess, AccountFailed);
        }

        /// <summary>
        /// Changes the view to default
        /// </summary>
        public void Back()
        {
            ShowDefaultView = true;
        }

        /// <summary>
        /// Attempts to register
        /// </summary>
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

        /// <summary>
        /// Fired if the account was successful
        /// </summary>
        /// <param name="response">The response recieved</param>
        public void AccountSuccess(TraktResponse response)
        {
            TraktSettings.Username = Username;
            TraktSettings.Password = CalculateSHA1(Password, Encoding.UTF8);
            TraktSettings.LoggedIn = true;
            SaveSettings();
            ProgressBarVisible = false;
            (Container.GetInstance(typeof (MainPageViewModel), "MainPageViewModel") as MainPageViewModel).SetUpApplicationBar();
            TryClose();
        }

        /// <summary>
        /// Fired if the account failed
        /// </summary>
        /// <param name="exception">The exception thrown</param>
        public void AccountFailed(Exception exception)
        {
            TraktSettings.LoggedIn = false;
            TraktSettings.Password = String.Empty;
            ProgressBarVisible = false;
            TryClose();
        }

        /// <summary>
        /// Calculates a SHA1 Hash for a string
        /// </summary>
        /// <param name="text">The string to hash</param>
        /// <param name="enc">The encoding to use</param>
        /// <returns>The resulting hash string</returns>
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
