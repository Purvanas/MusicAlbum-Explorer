using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MusicAlbum_Explorer.Models
{
    public class Song : INotifyPropertyChanged
    {
        public Song()
        {
            Id = Guid.NewGuid().ToString();
        }

        // Unique id to identify the song when storing favorites
        public string Id { get; set; }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                if (_duration == value) return;
                _duration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DurationFormatted));
            }
        }

        // YouTube URL for the song (can be null/empty)
        private string _youTubeUrl;
        public string YouTubeUrl
        {
            get => _youTubeUrl;
            set
            {
                if (_youTubeUrl == value) return;
                _youTubeUrl = value;
                OnPropertyChanged();
            }
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (_isFavorite == value) return;
                _isFavorite = value;
                OnPropertyChanged();
            }
        }

        // Human readable duration like "3:45"
        public string DurationFormatted => Duration == TimeSpan.Zero ? string.Empty : $"{(int)Duration.TotalMinutes}:{Duration.Seconds:D2}";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}