using ExpressAgent.Platform.Models;
using PureCloudPlatform.Client.V2.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ExpressAgent.Platform.Abstracts
{
    public abstract class ExpressConversationParticipantCommunication : INotifyPropertyChanged
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

        private string _State;
        public string State
        {
            get
            {
                return _State;
            }
            set
            {
                if (value != _State)
                {
                    _State = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool? _Held;
        public bool? Held
        {
            get
            {
                return _Held;
            }
            set
            {
                if (value != _Held)
                {
                    _Held = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _ConnectedTime;
        public DateTime? ConnectedTime
        {
            get
            {
                return _ConnectedTime;
            }
            set
            {
                if (value != _ConnectedTime)
                {
                    _ConnectedTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private ExpressConversationParticipant _Participant;
        public ExpressConversationParticipant Participant
        {
            get
            {
                return _Participant;
            }
        }

        public ExpressConversationParticipantCommunication(ExpressConversationParticipant participant)
        {
            _Participant = participant;
        }

        public void ToggleHold()
        {
            if (Participant == null)
            {
                Debug.WriteLine($"ExpressConversationParticipantCommunication: Unable to toggle hold state due to missing participant on {Id}");
                return;
            }

            MediaParticipantRequest body = new MediaParticipantRequest()
            {
                Held = !Held ?? true
            };

            Participant.Conversation.ConversationService.UpdateParticipant(Participant.Conversation.Id, Participant.Id, body);
        }

        public void Pickup()
        {
            if (Participant == null)
            {
                Debug.WriteLine($"ExpressConversationParticipantCommunication: Unable to pickup due to missing participant on {Id}");
                return;
            }

            MediaParticipantRequest body = new MediaParticipantRequest()
            {
                State = MediaParticipantRequest.StateEnum.Connected
            };

            Participant.Conversation.ConversationService.UpdateParticipant(Participant.Conversation.Id, Participant.Id, body);
        }

        public void Disconnect()
        {
            if (Participant == null)
            {
                Debug.WriteLine($"ExpressConversationParticipantCommunication: Unable to disconnect due to missing participant on {Id}");
                return;
            }

            MediaParticipantRequest body = new MediaParticipantRequest()
            {
                State = MediaParticipantRequest.StateEnum.Disconnected
            };

            Participant.Conversation.ConversationService.UpdateParticipant(Participant.Conversation.Id, Participant.Id, body);
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
