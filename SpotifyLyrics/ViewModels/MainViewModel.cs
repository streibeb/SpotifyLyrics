using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using SpotifyAPI.SpotifyLocalAPI;
using SpotifyLyrics.Models;

namespace SpotifyLyrics.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private SpotifyLocalAPIClass _spotify;
        private SpotifyMusicHandler _mh;
        private SpotifyEventHandler _eh;
        private LyricWrapper _lw;

        private TrackInfo _track;
        public TrackInfo Track
        {
            get { return _track; }
            set { _track = value; NotifyPropertyChanged(); }
        }

        RelayCommand _playCommand;
        public ICommand PlayCommand
        {
            get {
                if (_playCommand == null)
                {
                    _playCommand = new RelayCommand(param => this.Play());
                }
                return _playCommand; 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainViewModel()
        {
            Initialize();
        }

        public void Initialize()
        {
            _lw = new LyricWrapper();
            _spotify = new SpotifyLocalAPIClass();
            if (!SpotifyLocalAPIClass.IsSpotifyRunning())
            {
                _spotify.RunSpotify();
                Thread.Sleep(5000);
            }

            if (!SpotifyLocalAPIClass.IsSpotifyWebHelperRunning())
            {
                _spotify.RunSpotifyWebHelper();
                Thread.Sleep(4000);
            }

            if (!_spotify.Connect())
            {
                Boolean retry = true;
                while (retry)
                {
                    if (
                        MessageBox.Show("SpotifyLocalAPIClass couldn't load!", "Error", MessageBoxButton.OKCancel) ==
                        System.Windows.MessageBoxResult.Cancel)
                    {
                        if (_spotify.Connect())
                            retry = false;
                        else
                            retry = true;
                    }
                    else
                    {
                        this.Close();
                        return;
                    }
                }
            }

            _spotify.Update();
            _mh = _spotify.GetMusicHandler();
            _eh = _spotify.GetEventHandler();
        }

        public void Close()
        {
            Application.Current.Shutdown();
        }

        public void LoadData()
        {
            Track = new TrackInfo();

            Track.CurrentTime = 0;
            Track.TotalTime = (int)_mh.GetCurrentTrack().GetLength();
            Track.LoadBitmap(_mh.GetCurrentTrack().GetAlbumArt(AlbumArtSize.SIZE_160));
            Track.Name = _mh.GetCurrentTrack().GetTrackName();
            Track.Artist = _mh.GetCurrentTrack().GetArtistName();
            Track.Album = _mh.GetCurrentTrack().GetAlbumName();
            Track.Lyrics = _lw.GetLyrics(_mh.GetCurrentTrack());
            Track.IsPlaying = _mh.IsPlaying();

            _eh.OnTrackChange += TrackChange;
            _eh.OnTrackTimeChange += TimeChange;
            _eh.OnPlayStateChange += PlayStateChange;
            _eh.ListenForEvents(true);
        }

        private void Play()
        {
            if (!_mh.IsPlaying())
            {
                _mh.Play();
            }
            else
            {
                _mh.Pause();
            }
        }

        private void TrackChange(TrackChangeEventArgs e)
        {
            Track.CurrentTime = 0;
            Track.TotalTime = (int)e.new_track.GetLength();
            Track.LoadBitmap(_mh.GetCurrentTrack().GetAlbumArt(AlbumArtSize.SIZE_160));
            Track.Name = e.new_track.GetTrackName();
            Track.Artist = e.new_track.GetArtistName();
            Track.Album = e.new_track.GetAlbumName();
            Track.Lyrics = _lw.GetLyrics(e.new_track);
        }

        private void TimeChange(TrackTimeChangeEventArgs e)
        {
            Track.CurrentTime = (int)e.track_time;
        }

        private void PlayStateChange(PlayStateEventArgs e)
        {
            Track.IsPlaying = e.playing;
        }
    }
}
