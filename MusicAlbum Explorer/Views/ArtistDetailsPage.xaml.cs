using Microsoft.Maui.Controls;
using MusicAlbum_Explorer.Models;
using System.Collections.Generic;
using System.Windows.Input;

namespace MusicAlbum_Explorer.Views
{
    public partial class ArtistDetailsPage : ContentPage, IQueryAttributable
    {
        public ICommand AlbumTappedCommand { get; }

        public ArtistDetailsPage()
        {
            InitializeComponent();

            AlbumTappedCommand = new Command<Album>(async (album) =>
            {
                if (album == null) return;
                await Shell.Current.GoToAsync(nameof(AlbumSongsPage), true, new Dictionary<string, object>
                {
                    { "album", album }
                });
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // no-op
        }

        // Receive the artist object via Shell navigation dictionary
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query != null && query.TryGetValue("artist", out var obj) && obj is Artist artist)
            {
                BindingContext = artist;
            }
        }
    }
}