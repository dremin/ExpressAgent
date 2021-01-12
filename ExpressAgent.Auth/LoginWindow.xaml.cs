using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using ININ.PureCloud.OAuthControl;

namespace ExpressAgent.Auth
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool QuitOnClose = true;
        private OAuthWebBrowser AuthBrowser;
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
            AuthBrowser.Navigate($"https://login.{Environment}/logout?client_id={ClientId}&redirect_uri={RedirectUri}");
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
            AuthBrowser = new OAuthWebBrowser
            {
                Dock = DockStyle.Fill,
                RedirectUri = RedirectUri,
                RedirectUriIsFake = true,
                ClientId = ClientId,
                Environment = Environment
            };

            AuthBrowser.Authenticated += AuthBrowser_Authenticated;
            AuthBrowser.ExceptionEncountered += AuthBrowser_ExceptionEncountered;
            AuthBrowser.Navigated += AuthBrowser_Navigated;
            AuthBrowser.Navigating += AuthBrowser_Navigating;

            Panel panel = new Panel { Dock = DockStyle.Fill };
            panel.Controls.Add(AuthBrowser);
            BrowserHost.Child = panel;
        }

        private void AuthBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString().Contains(RedirectUri))
            {
                BrowserHost.Visibility = Visibility.Hidden;
            }
            else
            {
                BrowserHost.Visibility = Visibility.Visible;
            }
        }

        private void AuthBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.ToString().Contains(RedirectUri) && !IsAuthenticating)
            {
                Login();
            }
        }

        private void AuthBrowser_ExceptionEncountered(string source, Exception ex)
        {
            System.Windows.MessageBox.Show("There was a problem signing in to Genesys Cloud. Please try again.");
            Login();
        }

        private void AuthBrowser_Authenticated(string accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                Debug.WriteLine("LoginWindow: Successfully obtained access token");
                Session.AuthToken = accessToken;
                QuitOnClose = false;
                Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (QuitOnClose)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}
