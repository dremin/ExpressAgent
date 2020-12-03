using System;
using System.ComponentModel;
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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
