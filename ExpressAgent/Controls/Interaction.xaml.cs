using ExpressAgent.Platform.Models;
using System.Windows;
using System.Windows.Controls;

namespace ExpressAgent.Controls
{
    /// <summary>
    /// Interaction logic for Interaction.xaml
    /// </summary>
    public partial class Interaction : UserControl
    {
        private ExpressConversation Conversation
        {
            get
            {
                return DataContext as ExpressConversation;
            }
        }

        public Interaction()
        {
            InitializeComponent();
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HoldButton_Click(object sender, RoutedEventArgs e)
        {
            Conversation.ToggleHold();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
