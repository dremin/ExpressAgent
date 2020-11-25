using PureCloudPlatform.Client.V2.Extensions.Notifications;
using PureCloudPlatform.Client.V2.Model;

namespace ExpressAgent.Notifications
{
    public delegate void ConversationEventDelegate(NotificationData<ConversationEventTopicConversation> conversationEvent);

    public delegate void PresenceEventDelegate(NotificationData<PresenceEventUserPresence> presenceEvent);

    public delegate void RoutingStatusEventDelegate(NotificationData<UserRoutingStatusUserRoutingStatus> routingStatusEvent);
}
