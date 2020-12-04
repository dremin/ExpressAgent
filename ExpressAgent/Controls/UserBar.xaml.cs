using ExpressAgent.Platform;
using System.Windows.Controls;

namespace ExpressAgent.Controls
{
    /// <summary>
    /// Interaction logic for UserBar.xaml
    /// </summary>
    public partial class UserBar : UserControl
    {
        private Session Session
        {
            get
            {
                return DataContext as Session;
            }
        }

        public UserBar()
        {
            InitializeComponent();
        }

        private void PresenceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Session?.Presence.SetUserPresence(Session.CurrentUser.Id, (sender as ComboBox).SelectedValue as string);
        }
    }
}
