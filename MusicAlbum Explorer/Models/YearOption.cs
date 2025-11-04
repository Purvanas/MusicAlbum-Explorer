namespace MusicAlbum_Explorer.Models
{
    public class YearOption
    {
        public int? Year { get; set; }
        public string Display { get; set; }

        public override string ToString() => Display;
    }
}