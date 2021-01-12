using ExpressAgent.Auth;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Model;
using System.Diagnostics;
using System.Linq;
using ExpressAgent.Notifications;
using ExpressAgent.Platform.Models;
using ExpressAgent.Platform.Services;
using PureCloudPlatform.Client.V2.Api;

namespace ExpressAgent.Platform
{
    public class Session : INotifyPropertyChanged, IDisposable
    {
        public ConversationService Conversations { get; set; }
        public PresenceService Presence { get; set; }
        public RoutingService Routing { get; set; }
        public UserService Users { get; set; }
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

        private bool LoggingOut;

        public Session()
        {
            Configuration.Default.ApiClient.setBasePath(PureCloudRegionHosts.us_east_1);

            Conversations = new ConversationService(new ConversationsApi(), this);
            Presence = new PresenceService(new PresenceApi(), this);
            Routing = new RoutingService(new RoutingApi(), this);
            Users = new UserService(new UsersApi(), this);

            if (!AuthSession.Current.HasToken)
            {
                AuthSession.Current.Authenticate();
            }

            AuthSession.Current.PropertyChanged += AuthSession_PropertyChanged;

            Authenticated += Session_Authenticated;
            Unauthenticated += Session_Unauthenticated;
        }

        public void HandleException(ApiException e)
        {
            Debug.WriteLine(e.Message);

            if (e.ErrorCode == 401)
            {
                Unauthenticated?.Invoke(this, new EventArgs());
            }
        }

        private void Session_Authenticated(object sender, EventArgs e)
        {
            NotificationsWebsocket = new Websocket(CurrentUser.Id)
            {
                ConversationEventDelegate = Conversations.HandleConversationEvent,
                PresenceEventDelegate = Presence.HandlePresenceEvent,
                RoutingStatusEventDelegate = Presence.HandleRoutingStatusEvent
            };

            Presence.SetInitialPresence();
            Routing.SetQueueCollection();
            Conversations.SetActiveConversations();
        }

        private void Session_Unauthenticated(object sender, EventArgs e)
        {
            NotificationsWebsocket?.Dispose();

            if (!LoggingOut) AuthSession.Current.Authenticate();
        }

        private void AuthSession_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AuthToken" && AuthSession.Current.HasToken)
            {
                LoggingOut = false;
                Configuration.Default.AccessToken = (sender as AuthSession).AuthToken;

                CurrentUser = Users.GetCurrentUser();

                Debug.WriteLine("Session: Authenticated successfully");

                Authenticated?.Invoke(this, new EventArgs());
            }
        }

        public EventHandler Authenticated;
        public EventHandler Unauthenticated;

        public void Logout()
        {
            LoggingOut = true;
            AuthSession.Current.Logout();
            Unauthenticated?.Invoke(this, new EventArgs());
        }

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
