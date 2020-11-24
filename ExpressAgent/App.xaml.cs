using ExpressAgent.Platform;
using System;
using System.Windows;

namespace ExpressAgent
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private AgentWindow Window;
        private Session Session;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Session = new Session();
            Session.Authenticated += Session_Authenticated;
        }

        private void Session_Authenticated(object sender, EventArgs e)
        {
            if (Window != null && Window.IsVisible)
            {
                Window.QuitOnClose = false;
                Window.Close();
            }

            Window = new AgentWindow(Session);
            Window.Show();
        }
    }
}
