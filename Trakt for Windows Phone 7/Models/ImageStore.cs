using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
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
        private readonly Queue<string> _downloadQueue;
        private bool _downloadsRunning;

        public ImageStore(ImageStoreType type)
        {
            _internalStore = new Dictionary<string, BitmapImage>();
            _downloadQueue = new Queue<string>();
            Type = type;
        }

        public BitmapImage this[string key]
        {
            get
            {
                if (!_internalStore.ContainsKey(key))
                {
                    //Add to Download Queue
                    _downloadQueue.Enqueue(key);
                    DownloadImages();

                    if (Type == ImageStoreType.Poster)
                        return (BitmapImage)new ImageSourceConverter().ConvertFromString(@"..\artwork\poster-small.jpg");

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

        private void DownloadImages()
        {
            if (_downloadsRunning)
                return;
            _downloadsRunning = true;
            var wc = new WebClient();
            var currentDownload = _downloadQueue.Dequeue();

            wc.OpenReadCompleted += (sender, args) =>
            {
                if (args.Error == null && !args.Cancelled)
                {
                    var image = new BitmapImage();
                    image.SetSource(args.Result);
                    _internalStore[currentDownload] = image;
                    FirePropertyChanged(currentDownload);
                }
                if (_downloadQueue.Count > 0)
                {
                    currentDownload = _downloadQueue.Dequeue();
                    wc.OpenReadAsync(new Uri(currentDownload), wc);
                }
                else
                    _downloadsRunning = false;
            };

            wc.OpenReadAsync(new Uri(currentDownload), wc);


        }

        private void FirePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

    }
}
