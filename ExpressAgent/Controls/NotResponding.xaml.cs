using ExpressAgent.Platform;
using ExpressAgent.Platform.Models;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ExpressAgent.Controls
{
    /// <summary>
    /// Interaction logic for NotResponding.xaml
    /// </summary>
    public partial class NotResponding : UserControl
    {
        private bool _isLoaded;

        private Session Session
        {
            get
            {
                return DataContext as Session;
            }
        }

        public NotResponding()
        {
            InitializeComponent();
        }

        private void OnQueueButton_Click(object sender, RoutedEventArgs e)
        {
            Session?.Users.SetUserIdle();
        }

        private void OffQueueButton_Click(object sender, RoutedEventArgs e)
        {
            ExpressPresence offQueue = Session?.Presence.OrgPresences.Where(p => p.SystemPresence == "Available" && p.Primary == true).FirstOrDefault();

            if (offQueue == null)
            {
                Debug.WriteLine("NotResponding: Off Queue presence not found.");
                return;
            }

            Session?.Presence.SetUserPresence(offQueue.Id);
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CurrentRoutingState")
            {
                return;
            }

            if (Session?.Users.CurrentRoutingState == Platform.Enums.RoutingState.NotResponding)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_isLoaded || Session == null)
            {
                return;
            }

            Session.Users.PropertyChanged += User_PropertyChanged;

            if (Session.Users.CurrentRoutingState == Platform.Enums.RoutingState.NotResponding)
            {
                Visibility = Visibility.Visible;
            }

            _isLoaded = true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                return;
            }

            if (Session != null)
            {
                Session.Users.PropertyChanged -= User_PropertyChanged;
            }

            _isLoaded = false;
        }
    }
}
