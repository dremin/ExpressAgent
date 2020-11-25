using ExpressAgent.Auth;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Model;
using System.Diagnostics;
using System.Linq;
using ExpressAgent.Notifications;

namespace ExpressAgent.Platform
{
    public class Session : INotifyPropertyChanged, IDisposable
    {
        public ConversationHelper Conversations { get; set; }
        public PresenceHelper Presence { get; set; }
        public RoutingHelper Routing { get; set; }
        public UserHelper Users { get; set; }
        public Websocket NotificationsWebsocket;

        private UserMe _CurrentUser;
        public UserMe CurrentUser
        {
            get
            {
                return _CurrentUser;
            }
            set
            {
                if (value != _CurrentUser)
                {
                    _CurrentUser = value;
                    OnPropertyChanged();
                }
            }
        }

        public Session()
        {
            Configuration.Default.ApiClient.setBasePath(PureCloudRegionHosts.us_east_1);

            Conversations = new ConversationHelper(this);
            Presence = new PresenceHelper(this);
            Routing = new RoutingHelper(this);
            Users = new UserHelper(this);

            if (!AuthSession.Current.HasToken)
            {
                AuthSession.Current.Authenticate();
            }

            AuthSession.Current.PropertyChanged += AuthSession_PropertyChanged;

            Authenticated += Session_Authenticated;
        }

        public void HandleException(ApiException e)
        {
            Debug.WriteLine(e.Message);

            if (e.ErrorCode == 401)
            {
                AuthSession.Current.Authenticate();
            }
        }

        private void SetInitialPresence()
        {
            OrganizationPresence availablePresence = Presence.OrgPresences.Where(p => p.SystemPresence == "Available" && p.Primary == true).First();
            Presence.SetUserPresence(CurrentUser.Id, availablePresence.Id);
        }

        private void Session_Authenticated(object sender, EventArgs e)
        {
            NotificationsWebsocket = new Websocket(CurrentUser.Id)
            {
                ConversationEventDelegate = Conversations.HandleConversationEvent,
                PresenceEventDelegate = Presence.HandlePresenceEvent,
                RoutingStatusEventDelegate = Presence.HandleRoutingStatusEvent
            };

            SetInitialPresence();
            Conversations.GetActiveConversations();
        }

        private void AuthSession_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AuthToken" && AuthSession.Current.HasToken)
            {
                Configuration.Default.AccessToken = (sender as AuthSession).AuthToken;

                CurrentUser = Users.GetCurrentUser();

                Debug.WriteLine("Session: Authenticated successfully");

                Authenticated?.Invoke(this, new EventArgs());
            }
        }

        public EventHandler Authenticated;
        public EventHandler Unauthenticated;

        public void Dispose()
        {
            NotificationsWebsocket?.Dispose();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
