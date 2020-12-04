using ExpressAgent.Platform.Abstracts;
using ExpressAgent.Platform.Models;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Extensions.Notifications;
using PureCloudPlatform.Client.V2.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ExpressAgent.Platform.Services
{
    public class PresenceService : PlatformService<PresenceApi>, INotifyPropertyChanged
    {
        private ObservableCollection<ExpressPresence> _OrgPresences;
        public ObservableCollection<ExpressPresence> OrgPresences
        {
            get
            {
                if (_OrgPresences == null)
                {
                    _OrgPresences = new ObservableCollection<ExpressPresence>(GetPresences());
                }

                return _OrgPresences;
            }
        }

        private ExpressPresence _CurrentPresence;
        public ExpressPresence CurrentPresence
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

        public PresenceService(PresenceApi apiInstance, Session session) : base(apiInstance, session)
        {
        }

        public List<ExpressPresence> GetPresences(int pageNumber = 0)
        {
            try
            {
                Debug.WriteLine($"PresenceService: Calling GetPresencedefinitions");

                OrganizationPresenceEntityListing result = ApiInstance.GetPresencedefinitions(pageNumber);
                List<ExpressPresence> presences = new List<ExpressPresence>();

                foreach(OrganizationPresence presence in result.Entities)
                {
                    presences.Add(FromOrgPresence(presence));
                }

                if (result.PageNumber != null)
                {
                    if (result.PageCount > result.PageNumber)
                    {
                        presences.AddRange(GetPresences((int)result.PageNumber + 1));
                    }
                }

                return presences;
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new List<ExpressPresence>();
        }

        public ExpressPresence GetUserPresence(string userId)
        {
            try
            {
                Debug.WriteLine($"PresenceService: Calling GetUserPresencesPurecloud");

                UserPresence userPresence = ApiInstance.GetUserPresencesPurecloud(userId);

                return FromUserPresence(userPresence);
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new ExpressPresence();
        }

        public bool SetUserPresence(string userId, string presenceId, string message = null)
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

                Debug.WriteLine($"PresenceService: Calling PatchUserPresencesPurecloud");

                UserPresence userPresence = ApiInstance.PatchUserPresencesPurecloud(userId, body);

                return userPresence.PresenceDefinition.Id == presenceId;
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return false;
        }

        public bool SetInitialPresence()
        {
            ExpressPresence availablePresence = OrgPresences.Where(p => p.SystemPresence == "Available" && p.Primary == true).FirstOrDefault();
            return SetUserPresence(Session.CurrentUser.Id, availablePresence.Id);
        }

        #region Conversion
        public ExpressPresence FromUserPresence(UserPresence userPresence)
        {
            ExpressPresence orgPresence = OrgPresences.Where(p => p.Id == userPresence.PresenceDefinition.Id).FirstOrDefault();

            return new ExpressPresence
            {
                Id = orgPresence.Id,
                Name = orgPresence.Name,
                Message = userPresence.Message,
                SystemPresence = orgPresence.SystemPresence,
                Primary = orgPresence.Primary
            };
        }

        public ExpressPresence FromOrgPresence(OrganizationPresence orgPresence)
        {
            return new ExpressPresence
            {
                Id = orgPresence.Id,
                Name = GetPresenceName(orgPresence),
                SystemPresence = orgPresence.SystemPresence,
                Primary = orgPresence.Primary ?? false
            };
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
        #endregion

        #region Notification handlers
        public void HandlePresenceEvent(NotificationData<PresenceEventUserPresence> presenceEvent)
        {
            if (presenceEvent.EventBody.Source == "PURECLOUD")
            {
                // because the presence name doesn't come through in the notification, match up with our presence list using the ID
                ExpressPresence orgPresence = OrgPresences.Where(p => p.Id == presenceEvent.EventBody.PresenceDefinition.Id).FirstOrDefault();

                Debug.WriteLine($"PresenceService: Presence event received: New presence is {orgPresence.Name} Message: {presenceEvent.EventBody.Message}");

                if (orgPresence.Id == CurrentPresence.Id)
                {
                    // same presence, just a new message
                    CurrentPresence.Message = presenceEvent.EventBody.Message;
                }
                else
                {
                    CurrentPresence = new ExpressPresence
                    {
                        Id = orgPresence.Id,
                        Name = orgPresence.Name,
                        Message = presenceEvent.EventBody.Message,
                        SystemPresence = orgPresence.SystemPresence,
                        Primary = orgPresence.Primary
                    };
                }
            }
            else
            {
                Debug.WriteLine($"PresenceService: Ignoring presence update from unknown source {presenceEvent.EventBody.Source}");
            }
        }

        public void HandleRoutingStatusEvent(NotificationData<UserRoutingStatusUserRoutingStatus> routingStatusEvent)
        {
            Debug.WriteLine($"PresenceService: Routing status event received: New routing status is {routingStatusEvent.EventBody.RoutingStatus.Status}");

        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
