using System.Collections.Generic;

namespace MusicAlbum_Explorer.Models
{
    public class Album
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public string Cover { get; set; }
        public List<Song> Songs { get; set; } = new List<Song>();
    }
}
