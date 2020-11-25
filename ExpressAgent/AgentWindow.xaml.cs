using ExpressAgent.Platform;
using System.Windows;

namespace ExpressAgent
{
    /// <summary>
    /// Interaction logic for AgentWindow.xaml
    /// </summary>
    public partial class AgentWindow : Window
    {
        public bool QuitOnClose = true;
        private Session Session;

        public AgentWindow(Session session)
        {
            InitializeComponent();
            Session = session;

            DataContext = Session;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            if (QuitOnClose)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
