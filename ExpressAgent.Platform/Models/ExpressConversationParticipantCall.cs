using ExpressAgent.Platform.Abstracts;
using PureCloudPlatform.Client.V2.Model;
using System.Diagnostics;

namespace ExpressAgent.Platform.Models
{
    public class ExpressConversationParticipantCall : ExpressConversationParticipantCommunication
    {
        private bool? _Muted;
        public bool? Muted
        {
            get
            {
                return _Muted;
            }
            set
            {
                if (value != _Muted)
                {
                    _Muted = value;
                    OnPropertyChanged();
                }
            }
        }

        public ExpressConversationParticipantCall(ExpressConversationParticipant participant) : base(participant)
        { }

        public void ToggleMute()
        {
            if (Participant == null)
            {
                Debug.WriteLine($"ExpressConversationParticipantCall: Unable to toggle mute state due to missing participant on {Id}");
                return;
            }

            MediaParticipantRequest body = new MediaParticipantRequest()
            {
                Muted = !Muted ?? true
            };

            Participant.Conversation.ConversationService.UpdateParticipant(Participant.Conversation.Id, Participant.Id, body);
        }
    }
}
