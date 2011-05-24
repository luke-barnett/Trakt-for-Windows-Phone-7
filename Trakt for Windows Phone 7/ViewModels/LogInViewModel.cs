using System;
using Caliburn.Micro;
using TraktAPI;
using TraktAPI.TraktModels;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Security.Cryptography;

namespace Trakt_for_Windows_Phone_7.ViewModels
{
    public class LogInViewModel : BaseViewModel
    {
        readonly INavigationService navigationService;

        public LogInViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            if (string.IsNullOrEmpty(TraktSettings.Username))
            {
                Username = "Username";
            }
            else
            {
                Username = TraktSettings.Username;
            }

            Password = "Password";
            Email = "Email";

            if (TraktSettings.LoggedIn)
            {
                _Wait = false;
                NotifyOfPropertyChange("Wait");
            }
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        private bool _registration = false;
        public string Registration
        {
            get
            {
                if (!_registration || TraktSettings.LoggedIn)
                    return "Collapsed";
                else
                    return "Visible";
            }
        }

        public string LoggedIn
        {
            get
            {
                if (TraktSettings.LoggedIn)
                    return "Visible";
                else
                    return "Collapsed";
            }
        }

        public string LoggingIn
        {
            get
            {
                if (_registration || TraktSettings.LoggedIn)
                    return "Collapsed";
                else
                    return "Visible";
            }
        }

        public void BackToLogIn()
        {
            _registration = false;
            NotifyOfPropertyChange("LoggingIn");
            NotifyOfPropertyChange("Registration");
        }

        public void NewAccount()
        {
            _registration = true;
            NotifyOfPropertyChange("LoggingIn");
            NotifyOfPropertyChange("Registration");
        }

        public void Register()
        {
            _Wait = false;
            NotifyOfPropertyChange("Wait");
            string SHA1 = CalculateSHA1(Password, Encoding.UTF8);
            TraktAPI.TraktAPI.createAccount(Username, SHA1, Email).Subscribe(onNext: response => handleTraktResponse(response, SHA1), onError: error => failedToRegisterWithTrakt(error));
        }

        public void LogIntoTrakt()
        {
            if (Username.CompareTo("Username") != 0)
            {
                _Wait = false;
                NotifyOfPropertyChange("Wait");
                string SHA1 = CalculateSHA1(Password, Encoding.UTF8);
                TraktAPI.TraktAPI.testAccount(Username, SHA1).Subscribe(onNext: response => handleTraktResponse(response, SHA1), onError: error => failedToRegisterWithTrakt(error));
            }
        }

        private void failedToRegisterWithTrakt(Exception e)
        {
            handleError(e);
            MessageBox.Show("Authentication Failed");
            TraktSettings.Username = "";
            TraktSettings.Password = "";
            TraktSettings.LoggedIn = false;
            _Wait = true;
            NotifyOfPropertyChange("Wait");
        }

        public void LogOutOfTrakt()
        {
            TraktSettings.Username = "";
            TraktSettings.Password = "";
            TraktSettings.LoggedIn = false;
            navigationService.GoBack();
        }

        private void handleTraktResponse(TraktResponse response, string SHA1)
        {
            if (response.Status.CompareTo("success") == 0)
            {
                TraktSettings.Username = Username;
                TraktSettings.Password = SHA1;
                TraktSettings.LoggedIn = true;
                navigationService.GoBack();
            }
            else
            {
                if (response.Message.EndsWith("is already a registered username"))
                {
                    MessageBox.Show("Username is taken");
                }
                else
                {
                    MessageBox.Show("Authentication Failed");
                }
                TraktSettings.Username = "";
                TraktSettings.Password = "";
                TraktSettings.LoggedIn = false;
            }
            _Wait = true;
            NotifyOfPropertyChange("Wait");
        }

        public void ClearUsername()
        {
            Username = "";
            NotifyOfPropertyChange("Username");
        }

        public void ClearPassword()
        {
            Password = "";
            NotifyOfPropertyChange("Password");
        }

        public void ClearEmail()
        {
            Email = "";
            NotifyOfPropertyChange("Email");
        }

        private bool _Wait = true;
        public bool Wait { get { return _Wait; } }

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
