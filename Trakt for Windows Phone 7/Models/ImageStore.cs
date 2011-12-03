using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Trakt_for_Windows_Phone_7.Models
{
    public enum ImageStoreType
    {
        Poster,
        Screen
    }

    public class ImageStore : INotifyPropertyChanged
    {
        private readonly Dictionary<string, BitmapImage> _internalStore;

        public ImageStore(ImageStoreType type)
        {
            _internalStore = new Dictionary<string, BitmapImage>();
            Type = type;
        }

        public BitmapImage this[string key]
        {
            get
            {
                if (!_internalStore.ContainsKey(key))
                {
                    //Add to Download Queue

                    if (Type == ImageStoreType.Poster)
                        return (BitmapImage)new ImageSourceConverter().ConvertFromString(@"..\artwork\poster-small.jpg");
                    else
                        return (BitmapImage)new ImageSourceConverter().ConvertFromString(@"..\artwork\episode-screen.jpg");
                }

                return _internalStore[key];
            }
            set { _internalStore[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return _internalStore.Keys; }
        }

        public ICollection<BitmapImage> Values
        {
            get { return _internalStore.Values; }
        }

        public void Clear()
        {
            _internalStore.Clear();
        }

        public ImageStoreType Type { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
