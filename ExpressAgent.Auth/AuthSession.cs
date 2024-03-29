﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ExpressAgent.Auth
{
    public class AuthSession : INotifyPropertyChanged
    {
        private string _AuthToken;
        private static AuthSession _Current;

        public static AuthSession Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new AuthSession();
                }

                return _Current;
            }
        }

        public string AuthToken
        {
            get
            {
                return _AuthToken;
            }
            set
            {
                if (value != _AuthToken)
                {
                    _AuthToken = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasToken => !string.IsNullOrEmpty(AuthToken);

        public AuthSession() { }

        public void Authenticate()
        {
            ShowLoginWindow(false);
        }

        public void Logout()
        {
            ShowLoginWindow(true);
        }

        private void ShowLoginWindow(bool logout)
        {
            AuthToken = "";

            LoginWindow window = new LoginWindow(this, logout);
            window.Show();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
