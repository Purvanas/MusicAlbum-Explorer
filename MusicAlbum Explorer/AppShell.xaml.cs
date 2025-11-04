using Microsoft.Maui.Controls;
using MusicAlbum_Explorer.Views;

namespace MusicAlbum_Explorer
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ArtistDetailsPage), typeof(ArtistDetailsPage));
            Routing.RegisterRoute(nameof(AlbumSongsPage), typeof(AlbumSongsPage));
            Routing.RegisterRoute(nameof(YouTubePlayerPage), typeof(YouTubePlayerPage));
        }
    }
}
