using ExpressAgent.Auth;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Extensions;
using System.Diagnostics;

namespace ExpressAgent.Platform
{
    public class Session : INotifyPropertyChanged
    {
        public ConversationsApi ConversationsApi = new ConversationsApi();
        public RoutingApi RoutingApi = new RoutingApi();
        public UsersApi UsersApi = new UsersApi();

        public Session()
        {
            Configuration.Default.ApiClient.setBasePath(PureCloudRegionHosts.us_east_1);

            if (!AuthSession.Current.HasToken)
            {
                AuthSession.Current.Authenticate();
            }

            AuthSession.Current.PropertyChanged += AuthSession_PropertyChanged;

            Authenticated += Session_Authenticated;
        }

        private void Session_Authenticated(object sender, EventArgs e)
        {
            // sub to conv and presence
            // set presence
            // get existing conv list
        }

        private void AuthSession_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AuthToken" && AuthSession.Current.HasToken)
            {
                Configuration.Default.AccessToken = (sender as AuthSession).AuthToken;

                Debug.WriteLine("Session: Authenticated successfully");
                Authenticated?.Invoke(this, new EventArgs());
            }
        }

        public EventHandler Authenticated;
        public EventHandler Unauthenticated;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
