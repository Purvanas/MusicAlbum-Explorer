using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using MusicAlbum_Explorer.Services;
using System;
using System.Linq;

namespace MusicAlbum_Explorer.Views
{
    public partial class FavoritesPage : ContentPage
    {
        public FavoritesPage()
        {
            InitializeComponent();
            FavoritesService.OnChanged += FavoritesService_OnChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshList();
        }

        private void FavoritesService_OnChanged()
        {
            // Ensure UI update on main thread
            MainThread.BeginInvokeOnMainThread(() => RefreshList());
        }

        private void RefreshList()
        {
            var items = FavoritesService.GetAll().ToList();
            FavoritesCollection.ItemsSource = items;
        }

        private void OnRemoveClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is string id)
            {
                FavoritesService.RemoveFavoriteById(id);
            }
        }
    }
}
