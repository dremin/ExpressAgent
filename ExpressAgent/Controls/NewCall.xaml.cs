using ExpressAgent.Platform;
using System.Windows;
using System.Windows.Controls;

namespace ExpressAgent.Controls
{
    /// <summary>
    /// Interaction logic for NewCall.xaml
    /// </summary>
    public partial class NewCall : UserControl
    {
        private Session Session
        {
            get
            {
                return DataContext as Session;
            }
        }

        public static DependencyProperty ParentUserBarProperty = DependencyProperty.Register("ParentUserBar", typeof(UserBar), typeof(NewCall));

        public UserBar ParentUserBar
        {
            get { return (UserBar)GetValue(ParentUserBarProperty); }
            set { SetValue(ParentUserBarProperty, value); }
        }

        public NewCall()
        {
            InitializeComponent();
        }

        private void PlaceCallButton_Click(object sender, RoutedEventArgs e)
        {
            Session?.Conversations.CreateCall(Platform.Enums.ConversationTarget.PhoneNumber, TargetTextBox.Text, string.Empty);

            if (ParentUserBar != null)
            {
                ParentUserBar.NewCallButton.IsChecked = false;
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                TargetTextBox.Clear();
                TargetTextBox.Focus();
            }
        }
    }
}
