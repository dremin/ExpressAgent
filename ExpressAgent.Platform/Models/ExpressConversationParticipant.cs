using ExpressAgent.Platform.Abstracts;
using PureCloudPlatform.Client.V2.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExpressAgent.Platform.Models
{
    public class ExpressConversationParticipant : INotifyPropertyChanged
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

        private bool? _WrapupRequired;
        public bool? WrapupRequired
        {
            get
            {
                return _WrapupRequired;
            }
            set
            {
                if (value != _WrapupRequired)
                {
                    _WrapupRequired = value;
                    OnPropertyChanged();
                }
            }
        }

        private int? _WrapupTimeoutMs;
        public int? WrapupTimeoutMs
        {
            get
            {
                return _WrapupTimeoutMs;
            }
            set
            {
                if (value != _WrapupTimeoutMs)
                {
                    _WrapupTimeoutMs = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _StartAcwTime;
        public DateTime? StartAcwTime
        {
            get
            {
                return _StartAcwTime;
            }
            set
            {
                if (value != _StartAcwTime)
                {
                    _StartAcwTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Address;
        public string Address
        {
            get
            {
                return _Address;
            }
            set
            {
                if (value != _Address)
                {
                    _Address = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Purpose;
        public string Purpose
        {
            get
            {
                return _Purpose;
            }
            set
            {
                if (value != _Purpose)
                {
                    _Purpose = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _UserId;
        public string UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                if (value != _UserId)
                {
                    _UserId = value;
                    OnPropertyChanged();
                }
            }
        }

        private Queue _Queue;
        public Queue Queue
        {
            get
            {
                return _Queue;
            }
            set
            {
                if (value != _Queue)
                {
                    _Queue = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ExpressConversationParticipantCommunication> _Communications;
        public ObservableCollection<ExpressConversationParticipantCommunication> Communications
        {
            get
            {
                if (_Communications == null)
                {
                    _Communications = new ObservableCollection<ExpressConversationParticipantCommunication>();
                }

                return _Communications;
            }
            set
            {
                if (value != _Communications)
                {
                    _Communications = value;
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
