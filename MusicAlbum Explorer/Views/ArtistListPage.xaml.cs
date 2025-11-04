using Microsoft.Maui.Controls;
using MusicAlbum_Explorer.ViewModels;
using System;

namespace MusicAlbum_Explorer.Views
{
    public partial class ArtistListPage : ContentPage
    {
        public ArtistListPage()
        {
            InitializeComponent();
        }

        private async void OnFavoritesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(FavoritesPage));
        }
    }
}