using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Extensions.Notifications;
using PureCloudPlatform.Client.V2.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ExpressAgent.Platform
{
    public class PresenceHelper : INotifyPropertyChanged
    {
        private PresenceApi PresenceApi = new PresenceApi();
        private Session Session;

        private ObservableCollection<OrganizationPresence> _OrgPresences;
        public ObservableCollection<OrganizationPresence> OrgPresences
        {
            get
            {
                if (_OrgPresences == null)
                {
                    _OrgPresences = new ObservableCollection<OrganizationPresence>(GetPresences().Entities);
                }

                return _OrgPresences;
            }
        }

        private UserPresence _CurrentPresence;
        public UserPresence CurrentPresence
        {
            get
            {
                if (_CurrentPresence == null)
                {
                    _CurrentPresence = GetUserPresence(Session.CurrentUser.Id);
                }

                return _CurrentPresence;
            }
            set
            {
                if (value != _CurrentPresence)
                {
                    _CurrentPresence = value;
                    OnPropertyChanged();
                }
            }
        }

        public PresenceHelper(Session session)
        {
            Session = session;
        }

        public OrganizationPresenceEntityListing GetPresences(int pageNumber = 0)
        {
            try
            {
                Debug.WriteLine($"Presence: Calling GetPresencedefinitions");

                OrganizationPresenceEntityListing result = PresenceApi.GetPresencedefinitions(pageNumber);

                if (result.PageNumber != null)
                {
                    if (result.PageCount > result.PageNumber)
                    {
                        result.Entities.AddRange(GetPresences((int)result.PageNumber + 1).Entities);
                    }
                }

                return result;
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new OrganizationPresenceEntityListing();
        }

        public UserPresence GetUserPresence(string userId)
        {
            try
            {
                Debug.WriteLine($"Presence: Calling GetUserPresencesPurecloud");

                UserPresence userPresence = PresenceApi.GetUserPresencesPurecloud(userId);

                if (string.IsNullOrEmpty(userPresence.Name))
                {
                    OrganizationPresence presence = OrgPresences.Where(p => p.Id == userPresence.PresenceDefinition.Id).First();

                    userPresence.Name = GetPresenceName(presence);
                }

                return userPresence;
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new UserPresence();
        }

        public UserPresence SetUserPresence(string userId, string presenceId)
        {
            try
            {
                UserPresence body = new UserPresence
                {
                    PresenceDefinition = new PresenceDefinition
                    {
                        Id = presenceId
                    }
                };

                Debug.WriteLine($"Presence: Calling PatchUserPresencesPurecloud");

                return PresenceApi.PatchUserPresencesPurecloud(userId, body);
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new UserPresence();
        }

        public UserPresence SetUserPresence(string userId, string presenceId, string message)
        {
            try
            {
                UserPresence body = new UserPresence
                {
                    PresenceDefinition = new PresenceDefinition
                    {
                        Id = presenceId
                    },
                    Message = message
                };

                Debug.WriteLine($"Presence: Calling PatchUserPresencesPurecloud");

                return PresenceApi.PatchUserPresencesPurecloud(userId, body);
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new UserPresence();
        }

        private string GetPresenceName(OrganizationPresence presence)
        {
            string name = presence.Name;

            if (string.IsNullOrEmpty(name) && presence.LanguageLabels != null && presence.LanguageLabels.ContainsKey("en"))
            {
                name = presence.LanguageLabels["en"];
            }
            else if (string.IsNullOrEmpty(name))
            {
                name = presence.SystemPresence;
            }

            return name;
        }

        public void HandlePresenceEvent(NotificationData<PresenceEventUserPresence> presenceEvent)
        {
            Debug.WriteLine($"Websocket: Presence event received: New presence is {presenceEvent.EventBody.PresenceDefinition.Id} Message: {presenceEvent.EventBody.Message}");

            if (presenceEvent.EventBody.Source == "PURECLOUD")
            {
                OrganizationPresence presence = OrgPresences.Where(p => p.Id == presenceEvent.EventBody.PresenceDefinition.Id).First();

                UserPresence userPresence = new UserPresence(GetPresenceName(presence), 
                                                             presenceEvent.EventBody.Source, 
                                                             presence.Primary, 
                                                             new PresenceDefinition(presence.Id, 
                                                                                    presence.SystemPresence), 
                                                             presenceEvent.EventBody.Message, 
                                                             presenceEvent.EventBody.ModifiedDate);
                CurrentPresence = userPresence;
            }
        }

        public void HandleRoutingStatusEvent(NotificationData<UserRoutingStatusUserRoutingStatus> routingStatusEvent)
        {
            Debug.WriteLine($"Websocket: Routing status event received: New routing status is {routingStatusEvent.EventBody.RoutingStatus.Status}");

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
