using System;
using System.Diagnostics;
using System.Windows;

namespace ExpressAgent.Auth
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool QuitOnClose = true;
        private AuthSession Session;
        private string ClientId = "6a53129d-0f25-479f-89d7-63d91faa24b0";
        private string Environment = "mypurecloud.com";
        private string RedirectUri = "http://expressagent/";
        private bool IsAuthenticating;
        private bool PerformLogout;

        public LoginWindow(AuthSession session, bool performLogout)
        {
            InitializeComponent();
            Session = session;
            PerformLogout = performLogout;

            SetupBrowser();
        }

        public void Login()
        {
            Debug.WriteLine("LoginWindow: Logging in...");

            IsAuthenticating = true;
            AuthBrowser?.BeginImplicitGrant();
        }

        public void Logout()
        {
            Debug.WriteLine("LoginWindow: Logging out...");

            IsAuthenticating = false;
            AuthBrowser.Source = new Uri($"https://login.{Environment}/logout?client_id={ClientId}&redirect_uri={RedirectUri}");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (PerformLogout)
            {
                Logout();
            }
            else
            {
                Login();
            }
        }

        private void SetupBrowser()
        {
            AuthBrowser.Config = new GenesysCloudOAuthWebView.Core.OAuthConfig
            {
                RedirectUri = RedirectUri,
                RedirectUriIsFake = true,
                ClientId = ClientId,
                Environment = Environment
            };

            AuthBrowser.Authenticated += AuthBrowser_Authenticated;
            AuthBrowser.ExceptionEncountered += AuthBrowser_ExceptionEncountered;
            AuthBrowser.NavigationCompleted += AuthBrowser_NavigationCompleted;
        }

        private void AuthBrowser_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!IsAuthenticating)
            {
                Login();
            }
        }

        private void AuthBrowser_ExceptionEncountered(string source, Exception ex)
        {
            MessageBox.Show("There was a problem signing in to Genesys Cloud. Please try again.");
            Login();
        }

        private void AuthBrowser_Authenticated(GenesysCloudOAuthWebView.Core.OAuthResponse response)
        {
            if (!string.IsNullOrEmpty(response.AccessToken))
            {
                Debug.WriteLine("LoginWindow: Successfully obtained access token");
                Session.AuthToken = response.AccessToken;
                QuitOnClose = false;
                Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (QuitOnClose)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
