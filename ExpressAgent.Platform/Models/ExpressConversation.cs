using ExpressAgent.Platform.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ExpressAgent.Platform.Models
{
    public class ExpressConversation : INotifyPropertyChanged
    {
        private string _Id;
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (value != _Id)
                {
                    _Id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _RecordingState;
        public string RecordingState
        {
            get
            {
                return _RecordingState;
            }
            set
            {
                if (value != _RecordingState)
                {
                    _RecordingState = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ExpressConversationParticipant> _Participants;
        public ObservableCollection<ExpressConversationParticipant> Participants
        {
            get
            {
                if (_Participants == null)
                {
                    _Participants = new ObservableCollection<ExpressConversationParticipant>();
                }

                return _Participants;
            }
            set
            {
                if (value != _Participants)
                {
                    _Participants = value;
                    OnPropertyChanged();
                }
            }
        }

        public ExpressConversationParticipant RemoteParticipant
        {
            get
            {
                // first participant that does not have our user id
                return Participants.Where(p => p.UserId != _AgentUserId).FirstOrDefault();
            }
        }

        public ExpressConversationParticipant AgentParticipant
        {
            get
            {
                // last participant that has our user id
                return Participants.Where(p => p.UserId == _AgentUserId).LastOrDefault();
            }
        }

        private string _AgentUserId => _ConversationService.Session.CurrentUser.Id;
        private ConversationService _ConversationService;
        public ConversationService ConversationService
        {
            get
            {
                return _ConversationService;
            }
        }

        public ExpressConversation(ConversationService conversationService, string conversationId)
        {
            _ConversationService = conversationService;
            Id = conversationId;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        internal virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
