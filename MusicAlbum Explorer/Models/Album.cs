using System.Collections.Generic;

namespace MusicAlbum_Explorer.Models
{
    public class Album
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public string Cover { get; set; }

        // Parent artist name (populated when albums are created)
        public string ArtistName { get; set; }

        public List<Song> Songs { get; set; } = new List<Song>();
    }
}
