using Microsoft.Maui.Controls;
using MusicAlbum_Explorer.Models;
using System;
using Microsoft.Maui.ApplicationModel;
using System.Collections.Generic;

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
    }
}
