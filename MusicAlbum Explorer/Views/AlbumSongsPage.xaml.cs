using Microsoft.Maui.Controls;
using MusicAlbum_Explorer.Models;
using System;
using Microsoft.Maui.ApplicationModel;
using System.Collections.Generic;
using MusicAlbum_Explorer.Services;

namespace MusicAlbum_Explorer.Views
{
    public partial class AlbumSongsPage : ContentPage, IQueryAttributable
    {
        public AlbumSongsPage()
        {
            InitializeComponent();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query != null && query.TryGetValue("album", out var obj) && obj is Album album)
            {
                BindingContext = album;

                // initialize favorite state for songs based on persisted favorites
                if (album.Songs != null)
                {
                    foreach (var s in album.Songs)
                    {
                        s.IsFavorite = FavoritesService.IsFavoriteId(s.Id);
                    }
                }
            }
        }

        private async void OnSongClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Song song && !string.IsNullOrWhiteSpace(song.YouTubeUrl))
            {
                try
                {
                    // Navigate to an internal YouTube player page instead of launching external browser
                    await Shell.Current.GoToAsync(nameof(YouTubePlayerPage), true, new Dictionary<string, object>
                    {
                        { "url", song.YouTubeUrl }
                    });
                }
                catch (Exception)
                {
                    await DisplayAlert("Erreur", "Impossible d'ouvrir la vidéo.", "OK");
                }
            }
        }

        private void OnFavoriteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Song song)
            {
                // toggle in-memory flag so UI updates
                song.IsFavorite = !song.IsFavorite;

                // album and artist info are available via BindingContext
                if (BindingContext is Album album)
                {
                    if (song.IsFavorite)
                    {
                        FavoritesService.AddFavorite(song, album.Title, album.ArtistName, album.Cover);
                    }
                    else
                    {
                        FavoritesService.RemoveFavoriteById(song.Id);
                    }
                }
                else
                {
                    // Fallback: toggle persisted set by id only
                    if (song.IsFavorite)
                        FavoritesService.AddFavorite(song);
                    else
                        FavoritesService.RemoveFavoriteById(song.Id);
                }
            }
        }
    }
}
