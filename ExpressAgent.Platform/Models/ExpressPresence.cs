using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExpressAgent.Platform.Models
{
    public class ExpressPresence : INotifyPropertyChanged
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

        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                if (value != _Message)
                {
                    _Message = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _SystemPresence;
        public string SystemPresence
        {
            get
            {
                return _SystemPresence;
            }
            set
            {
                if (value != _SystemPresence)
                {
                    _SystemPresence = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _Primary;
        public bool Primary
        {
            get
            {
                return _Primary;
            }
            set
            {
                if (value != _Primary)
                {
                    _Primary = value;
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
