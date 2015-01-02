using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SpotifyLyrics.Models
{
    public class TrackInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _name;
        private string _artist;
        private string _album;
        private BitmapSource _albumArt;
        private string _currentTimestamp;
        private double _currentTime;
        private string _totalTimestamp;
        private double _totalTime;
        private string _lyrics;
        private bool _isPlaying;

        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged(); }
        }

        public string Artist
        {
            get { return _artist; }
            set { _artist = value; NotifyPropertyChanged(); }
        }

        public string Album
        {
            get { return _album; }
            set { _album = value; NotifyPropertyChanged(); }
        }

        public BitmapSource AlbumArt
        {
            get { return _albumArt; }
            set { _albumArt = value; NotifyPropertyChanged(); }
        }

        public string CurrentTimestamp
        {
            get { return _currentTimestamp; }
            set { _currentTimestamp = value; NotifyPropertyChanged(); }
        }

        public string TotalTimestamp
        {
            get { return _totalTimestamp; }
            set { _totalTimestamp = value; NotifyPropertyChanged(); }
        }

        public double CurrentTime
        {
            get { return _currentTime; }
            set 
            { 
                _currentTime = value;
                CurrentTimestamp = FormatTime(value); 
                NotifyPropertyChanged(); 
            }
        }

        public double TotalTime
        {
            get { return _totalTime; }
            set 
            { 
                _totalTime = value;
                TotalTimestamp = FormatTime(value);
                NotifyPropertyChanged(); 
            }
        }

        public string Lyrics
        {
            get { return _lyrics; }
            set { _lyrics = value; NotifyPropertyChanged(); }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { _isPlaying = value; NotifyPropertyChanged(); }
        }

        private string FormatTime(double sec)
        {
            TimeSpan span = TimeSpan.FromSeconds(sec);
            String secs = span.Seconds.ToString(), mins = span.Minutes.ToString();
            if (secs.Length < 2)
                secs = "0" + secs;
            return mins + ":" + secs;
        }

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public void LoadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            // Fix found here:
            // http://stackoverflow.com/questions/26361020/error-must-create-dependencysource-on-same-thread-as-the-dependencyobject-even
            bs.Freeze();
            Dispatcher.CurrentDispatcher.Invoke(() => AlbumArt = bs);
        }
    }
}