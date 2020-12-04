using ExpressAgent.Platform.Abstracts;
using ExpressAgent.Platform.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ExpressAgent.Controls
{
    /// <summary>
    /// Interaction logic for Communication.xaml
    /// </summary>
    public partial class Communication : UserControl
    {
        private ExpressConversationParticipantCommunication _Communication
        {
            get
            {
                return DataContext as ExpressConversationParticipantCommunication;
            }
        }

        public Communication()
        {
            InitializeComponent();
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ExpressConversationParticipantCall call)
            {
                call.ToggleMute();
            }
            else
            {
                Debug.WriteLine("Communication: Unable to mute this communication type");
            }
        }

        private void HoldButton_Click(object sender, RoutedEventArgs e)
        {
            _Communication.ToggleHold();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            _Communication.Disconnect();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            _Communication.Pickup();
        }

        private void DeclineButton_Click(object sender, RoutedEventArgs e)
        {
            _Communication.Disconnect();
        }
    }
}
