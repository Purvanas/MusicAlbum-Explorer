using System;

namespace MusicAlbum_Explorer.Models
{
    public class Song
    {
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }

        // YouTube URL for the song (can be null/empty)
        public string YouTubeUrl { get; set; }

        // Human readable duration like "3:45"
        public string DurationFormatted => Duration == TimeSpan.Zero ? string.Empty : $"{(int)Duration.TotalMinutes}:{Duration.Seconds:D2}";
    }
}