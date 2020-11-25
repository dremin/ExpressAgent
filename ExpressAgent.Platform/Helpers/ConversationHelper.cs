using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Extensions.Notifications;
using PureCloudPlatform.Client.V2.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ExpressAgent.Platform
{
    public class ConversationHelper
    {
        private ConversationsApi ConversationsApi = new ConversationsApi();
        private Session Session;

        public ObservableCollection<Conversation> ActiveConversations = new ObservableCollection<Conversation>();

        public ConversationHelper(Session session)
        {
            Session = session;
        }

        public ObservableCollection<Conversation> GetActiveConversations()
        {
            try
            {
                Debug.WriteLine($"Conversations: Calling GetConversations");

                ActiveConversations = new ObservableCollection<Conversation>(ConversationsApi.GetConversations().Entities);

                return ActiveConversations;
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new ObservableCollection<Conversation>();
        }

        public void HandleConversationEvent(NotificationData<ConversationEventTopicConversation> conversationEvent)
        {
            Debug.WriteLine($"Websocket: Conversation event received for conversation {conversationEvent.EventBody.Id}");

        }
    }
}
