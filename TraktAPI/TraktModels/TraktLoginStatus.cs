using System.ComponentModel;

namespace TraktAPI.TraktModels
{
    public class TraktLoginStatus : INotifyPropertyChanged
    {
        private bool _loggedIn;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsLoggedIn
        {
            get { return _loggedIn; }
            set
            {
                var firePropertyEvent = _loggedIn != value;
                _loggedIn = value;
                if(firePropertyEvent)
                    FirePropertyChanged("IsLoggedIn");
            }
        }

        private void FirePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
