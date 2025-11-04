using System.Collections.Generic;

namespace MusicAlbum_Explorer.Models
{
    public class Artist
    {
        public string Name { get; set; }
        public string Firstname { get; set; }
        public string Stagename { get; set; }
        public string Photo { get; set; }
        public string Biographie { get; set; }
        public List<MusicType> MusicType { get; set; } = new List<MusicType>();
        public List<Album> Albums { get; set; } = new List<Album>();
    }
}