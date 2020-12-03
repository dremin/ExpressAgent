using ExpressAgent.Platform.Abstracts;
using ExpressAgent.Platform.Models;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Extensions.Notifications;
using PureCloudPlatform.Client.V2.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace ExpressAgent.Platform.Services
{
    public class ConversationService : PlatformService<ConversationsApi>
    {
        public ObservableCollection<ExpressConversation> ActiveConversations { get; set; }

        public ConversationService(ConversationsApi apiInstance, Session session) : base(apiInstance, session)
        {
            ActiveConversations = new ObservableCollection<ExpressConversation>();
        }

        public ObservableCollection<ExpressConversation> GetActiveConversations()
        {
            try
            {
                Debug.WriteLine($"Conversations: Calling GetConversations");

                //ActiveConversations = new ObservableCollection<ExpressConversation>(ApiInstance.GetConversations().Entities);

                return ActiveConversations;
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new ObservableCollection<ExpressConversation>();
        }

        #region Conversion
        public bool UpdateExpressConversation(ref ExpressConversation expConv, ConversationEventTopicConversation conv)
        {
            bool isActiveConversation = false;

            expConv.Id = conv.Id;

            if (expConv.Participants == null)
            {
                expConv.Participants = new ObservableCollection<ExpressConversationParticipant>();
            }

            foreach (var participant in conv.Participants)
            {
                bool isNewParty = false;
                ExpressConversationParticipant expParty = expConv.Participants.Where(p => p.Id == participant.Id).FirstOrDefault();

                if (expParty == null)
                {
                    expParty = new ExpressConversationParticipant();
                    isNewParty = true;
                }

                expParty.Id = participant.Id;
                expParty.WrapupRequired = participant.WrapupRequired;
                expParty.WrapupTimeoutMs = participant.WrapupTimeoutMs;
                expParty.StartAcwTime = participant.StartAcwTime;
                expParty.Purpose = participant.Purpose;
                expParty.Address = participant.Address;
                expParty.Name = participant.Name;
                expParty.UserId = participant.UserId;
                expParty.Queue = Session.Routing.Queues.Where(q => q.Id == participant.QueueId).FirstOrDefault();

                if (expParty.Calls == null)
                {
                    expParty.Calls = new ObservableCollection<ExpressConversationParticipantCall>();
                }

                foreach (var call in participant.Calls)
                {
                    bool isNewCall = false;
                    ExpressConversationParticipantCall expCall = expParty.Calls.Where(c => c.Id == call.Id).FirstOrDefault();

                    if (expCall == null)
                    {
                        expCall = new ExpressConversationParticipantCall();
                        isNewCall = true;
                    }

                    expCall.Id = call.Id;
                    expCall.State = call.State.ToString().ToLower();
                    expCall.Held = call.Held;
                    expCall.Muted = call.Muted;
                    expCall.ConnectedTime = call.ConnectedTime;

                    if (isNewCall)
                    {
                        expParty.Calls.Add(expCall);
                    }

                    // conversation is active if the communication is not disconnected, or if awaiting wrapup
                    if (expParty.UserId == Session.CurrentUser.Id
                        && ((expCall.State != "disconnected" && expCall.State != "terminated") || (expParty.WrapupRequired == true && participant.Wrapup == null) ))
                    {
                        isActiveConversation = true;
                    }
                }

                if (isNewParty)
                {
                    expConv.Participants.Add(expParty);
                }
            }

            return isActiveConversation;
        }
        #endregion

        #region Notification handlers
        public void HandleConversationEvent(NotificationData<ConversationEventTopicConversation> conversationEvent)
        {
            Debug.WriteLine($"Websocket: Conversation event received for conversation {conversationEvent.EventBody.Id}");

            bool isExisting = false;
            ExpressConversation expConv = ActiveConversations.Where(c => c.Id == conversationEvent.EventBody.Id).FirstOrDefault();

            if (expConv != null)
            {
                isExisting = true;
            }
            else
            {
                expConv = new ExpressConversation(conversationEvent.EventBody.Id, Session.CurrentUser.Id);
            }

            if (UpdateExpressConversation(ref expConv, conversationEvent.EventBody))
            {
                // active conversation, add it to collection if it isn't already
                if (!isExisting)
                {
                    ActiveConversations.Add(expConv);
                }
            }
            else
            {
                // not an active conversation, remove it from collection if it exists
                if (isExisting)
                {
                    ActiveConversations.Remove(expConv);
                }
            }
        }
        #endregion
    }
}
