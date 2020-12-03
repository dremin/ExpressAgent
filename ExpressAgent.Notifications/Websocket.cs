using PureCloudPlatform.Client.V2.Extensions.Notifications;
using PureCloudPlatform.Client.V2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace ExpressAgent.Notifications
{
    public class Websocket : IDisposable
    {
        private NotificationHandler Handler = new NotificationHandler();
        private List<Tuple<string, Type>> Subscriptions;

        public ConversationEventDelegate ConversationEventDelegate;
        public PresenceEventDelegate PresenceEventDelegate;
        public RoutingStatusEventDelegate RoutingStatusEventDelegate;

        public Websocket(string userId)
        {
            Subscribe(userId);

            Handler.NotificationReceived += Handler_NotificationReceived;
        }

        private void Handler_NotificationReceived(INotificationData notificationData)
        {
            Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                switch (notificationData)
                {
                    case NotificationData<ConversationEventTopicConversation> conversationEvent:
                        if (ConversationEventDelegate != null)
                        {
                            ConversationEventDelegate(conversationEvent);
                        }
                        else
                        {
                            Debug.WriteLine("Websocket: ConversationEventDelegate is null");
                        }
                        break;
                    case NotificationData<PresenceEventUserPresence> presenceEvent:
                        if (PresenceEventDelegate != null)
                        {
                            PresenceEventDelegate(presenceEvent);
                        }
                        else
                        {
                            Debug.WriteLine("Websocket: PresenceEventDelegate is null");
                        }
                        break;
                    case NotificationData<UserRoutingStatusUserRoutingStatus> routingStatusEvent:
                        if (RoutingStatusEventDelegate != null)
                        {
                            RoutingStatusEventDelegate(routingStatusEvent);
                        }
                        else
                        {
                            Debug.WriteLine("Websocket: RoutingStatusEventDelegate is null");
                        }
                        break;
                    case NotificationData<ChannelMetadataNotification> metadataEvent:
                        Debug.WriteLine("Websocket: Received metadata");
                        break;
                    default:
                        Debug.WriteLine($"Websocket: Notification event type not recognized: {notificationData}");
                        break;
                }
            }));
        }

        private void Subscribe(string userId)
        {
            Subscriptions = new List<Tuple<string, Type>>();

            Subscriptions.Add(new Tuple<string, Type>($"v2.users.{userId}.conversations",
                                        typeof(ConversationEventTopicConversation)));
            Subscriptions.Add(new Tuple<string, Type>($"v2.users.{userId}.presence",
                                        typeof(PresenceEventUserPresence)));
            Subscriptions.Add(new Tuple<string, Type>($"v2.users.{userId}.routingStatus",
                                        typeof(UserRoutingStatusUserRoutingStatus)));

            Handler.AddSubscriptions(Subscriptions);

            Debug.WriteLineIf(Handler.WebSocket.IsAlive, "Websocket: Connected successfully");
        }

        public void Dispose()
        {
            Handler.Dispose();

            Debug.WriteLineIf(!Handler.WebSocket.IsAlive, "Websocket: Disconnected successfully");
        }
    }
}
