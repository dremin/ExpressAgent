using ExpressAgent.Platform.Abstracts;
using ExpressAgent.Platform.Enums;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Extensions.Notifications;
using PureCloudPlatform.Client.V2.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ExpressAgent.Platform.Services
{
    public class UserService : PlatformService<UsersApi>, INotifyPropertyChanged
    {
        private RoutingState _CurrentRoutingState;
        public RoutingState CurrentRoutingState
        {
            get
            {
                return _CurrentRoutingState;
            }
            private set
            {
                if (value != _CurrentRoutingState)
                {
                    _CurrentRoutingState = value;
                    OnPropertyChanged();
                }
            }
        }

        public UserService(UsersApi apiInstance, Session session) : base(apiInstance, session)
        {
        }

        public UserMe GetCurrentUser()
        {
            try
            {
                Debug.WriteLine($"UserService: Calling GetUsersMe");

                return ApiInstance.GetUsersMe(new List<string> { "presence", "routingStatus", "station" });
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new UserMe();
        }

        public RoutingState GetUserRoutingState(string userId)
        {
            try
            {
                Debug.WriteLine($"UserService: Calling GetUserRoutingstatus");

                RoutingStatus routingStatus = ApiInstance.GetUserRoutingstatus(userId);

                return FromUserRoutingStatus(routingStatus);
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return RoutingState.OffQueue;
        }

        public bool SetUserIdle()
        {
            try
            {
                RoutingStatus body = new RoutingStatus
                {
                    Status = RoutingStatus.StatusEnum.Idle
                };

                Debug.WriteLine($"PresenceService: Calling PutUserRoutingstatus");

                RoutingStatus routingStatus = ApiInstance.PutUserRoutingstatus(Session.CurrentUser.Id, body);

                return routingStatus.Status == RoutingStatus.StatusEnum.Idle;
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return false;
        }

        private RoutingState FromUserRoutingStatus(RoutingStatus routingStatus)
        {
            switch (routingStatus.Status)
            {
                case RoutingStatus.StatusEnum.NotResponding:
                    return RoutingState.NotResponding;
                case RoutingStatus.StatusEnum.Idle:
                case RoutingStatus.StatusEnum.Interacting:
                case RoutingStatus.StatusEnum.Communicating:
                    return RoutingState.OnQueue;
                default:
                    return RoutingState.OffQueue;
            }
        }

        private RoutingState FromUserRoutingStatus(UserRoutingStatusRoutingStatus routingStatus)
        {
            switch (routingStatus.Status)
            {
                case UserRoutingStatusRoutingStatus.StatusEnum.NotResponding:
                    return RoutingState.NotResponding;
                case UserRoutingStatusRoutingStatus.StatusEnum.Idle:
                case UserRoutingStatusRoutingStatus.StatusEnum.Interacting:
                case UserRoutingStatusRoutingStatus.StatusEnum.Communicating:
                    return RoutingState.OnQueue;
                default:
                    return RoutingState.OffQueue;
            }
        }

        #region Notification handlers
        public void HandleRoutingStatusEvent(NotificationData<UserRoutingStatusUserRoutingStatus> routingStatusEvent)
        {
            Debug.WriteLine($"UserService: Routing status event received: New routing status is {routingStatusEvent.EventBody.RoutingStatus.Status}");

            CurrentRoutingState = FromUserRoutingStatus(routingStatusEvent.EventBody.RoutingStatus);
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
