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
            Session?.Presence.SetUserPresence((sender as ComboBox).SelectedValue as string, Session?.Presence.CurrentPresence.Message);
        }

        private void LogoutMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Session?.Logout();
        }

        private void UserStackPanel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UserStackPanel.ContextMenu.IsOpen = true;
        }
    }
}
