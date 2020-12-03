using ExpressAgent.Platform.Abstracts;

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
    }
}
