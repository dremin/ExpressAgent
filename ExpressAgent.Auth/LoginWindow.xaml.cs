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

        public LoginWindow(AuthSession session)
        {
            InitializeComponent();
            Session = session;

            SetupBrowser();
        }

        public void Login()
        {
            AuthBrowser?.BeginImplicitGrant();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void SetupBrowser()
        {
            AuthBrowser = new OAuthWebBrowser
            {
                Dock = DockStyle.Fill,
                RedirectUri = "http://expressagent/",
                RedirectUriIsFake = true,
                ClientId = ClientId,
                Environment = Environment
            };

            AuthBrowser.Authenticated += AuthBrowser_Authenticated;
            AuthBrowser.ExceptionEncountered += AuthBrowser_ExceptionEncountered;
            AuthBrowser.Navigated += AuthBrowser_Navigated;

            Panel panel = new Panel { Dock = DockStyle.Fill };
            panel.Controls.Add(AuthBrowser);
            BrowserHost.Child = panel;
        }

        private void AuthBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            BrowserHost.Visibility = Visibility.Visible;
        }

        private void AuthBrowser_ExceptionEncountered(string source, Exception ex)
        {
            System.Windows.MessageBox.Show("There was a problem sigining in to Genesys Cloud. Please try again.");
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
