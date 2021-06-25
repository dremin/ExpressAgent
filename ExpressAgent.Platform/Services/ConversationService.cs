using ExpressAgent.Platform.Abstracts;
using ExpressAgent.Platform.Enums;
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
                Debug.WriteLine($"ConversationService: Calling GetConversations");

                ObservableCollection<ExpressConversation> conversations = new ObservableCollection<ExpressConversation>();

                foreach (var conv in ApiInstance.GetConversations().Entities)
                {
                    ExpressConversation expConv = new ExpressConversation(this, conv.Id);

                    if (UpdateExpressConversation(ref expConv, conv))
                    {
                        conversations.Add(expConv);
                    }
                }

                return conversations;
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new ObservableCollection<ExpressConversation>();
        }

        public void SetActiveConversations()
        {
            ActiveConversations = GetActiveConversations();
        }

        public void UpdateCall(string conversationId, Conversation.RecordingStateEnum recordingState)
        {
            try
            {
                Conversation body = new Conversation()
                {
                    RecordingState = recordingState
                };

                Debug.WriteLine($"ConversationService: Calling PatchConversationsCall");

                ApiInstance.PatchConversationsCall(conversationId, body);
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }
        }

        public void UpdateParticipant(string conversationId, string participantId, MediaParticipantRequest body)
        {
            try
            {
                Debug.WriteLine($"ConversationService: Calling PatchConversationParticipant");

                ApiInstance.PatchConversationParticipant(conversationId, participantId, body);
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }
        }

        public void CreateCall(ConversationTarget targetType, string target, string queueId)
        {
            try
            { 
                CreateCallRequest request = new CreateCallRequest();

                switch (targetType)
                {
                    case ConversationTarget.PhoneNumber:
                        request.PhoneNumber = target;
                        break;
                    case ConversationTarget.Queue:
                        request.CallQueueId = target;
                        break;
                    case ConversationTarget.User:
                        request.CallUserId = target;
                        break;
                }

                if (!string.IsNullOrEmpty(queueId))
                {
                    request.CallFromQueueId = queueId;
                }

                ApiInstance.PostConversationsCalls(request);
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }
        }

        public List<WrapupCode> GetConversationParticipantWrapupCodes(string conversationId, string participantId)
        {
            try
            {
                Debug.WriteLine($"ConversationService: Calling GetConversationParticipantWrapupcodes");

                List<WrapupCode> codes = ApiInstance.GetConversationParticipantWrapupcodes(conversationId, participantId);

                return codes;
            }
            catch (ApiException e)
            {
                Session.HandleException(e);

                return new List<WrapupCode>();
            }
        }

        #region Conversion
        public bool UpdateExpressConversation(ref ExpressConversation expConv, dynamic conv)
        {
            // conv must be of type Conversation or ConversationEventTopicConversation
            // the dynamic is less than ideal but Conversation and ConversationEventTopicConversation don't inherit from a common class

            bool isActiveConversation = false;

            expConv.Id = conv.Id;

            foreach (var participant in conv.Participants)
            {
                bool isNewParty = false;
                ExpressConversationParticipant expParty = expConv.Participants.Where(p => p.Id == participant.Id).FirstOrDefault();

                if (expParty == null)
                {
                    expParty = new ExpressConversationParticipant(expConv);
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

                if (participant.Calls != null)
                {
                    foreach (var call in participant.Calls)
                    {
                        bool isNewCall = false;
                        ExpressConversationParticipantCall expCall = (ExpressConversationParticipantCall)expParty.Communications.Where(c => c is ExpressConversationParticipantCall && c.Id == call.Id).FirstOrDefault();

                        if (expCall == null)
                        {
                            expCall = new ExpressConversationParticipantCall(expParty);
                            isNewCall = true;
                        }

                        expCall.Id = call.Id;
                        expCall.State = call.State.ToString().ToLower();
                        expCall.Held = call.Held;
                        expCall.Muted = call.Muted;
                        expCall.ConnectedTime = call.ConnectedTime;

                        if (isNewCall)
                        {
                            expParty.Communications.Add(expCall);
                        }

                        // conversation is active if the communication is not disconnected/terminated, or if awaiting wrapup
                        if (expParty.UserId == Session.CurrentUser.Id
                            && ((expCall.State != "disconnected" && expCall.State != "terminated") || (expParty.WrapupRequired == true && (participant.Wrapup == null || string.IsNullOrEmpty(participant.Wrapup.Code)))))
                        {
                            isActiveConversation = true;
                        }
                    }
                }

                if (isNewParty)
                {
                    expConv.Participants.Add(expParty);

                    // send PropertyChanged notification if this participant became one of the convenience properties
                    if (expConv.AgentParticipant?.Id == expParty.Id)
                    {
                        expConv.OnPropertyChanged("AgentParticipant");
                    }

                    if (expConv.RemoteParticipant?.Id == expParty.Id)
                    {
                        expConv.OnPropertyChanged("RemoteParticipant");
                    }
                }
            }

            return isActiveConversation;
        }
        #endregion

        #region Notification handlers
        public void HandleConversationEvent(NotificationData<ConversationEventTopicConversation> conversationEvent)
        {
            Debug.WriteLine($"ConversationService: Conversation event received for conversation {conversationEvent.EventBody.Id}");

            bool isExisting = false;
            ExpressConversation expConv = ActiveConversations.Where(c => c.Id == conversationEvent.EventBody.Id).FirstOrDefault();

            if (expConv != null)
            {
                isExisting = true;
            }
            else
            {
                expConv = new ExpressConversation(this, conversationEvent.EventBody.Id);
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
