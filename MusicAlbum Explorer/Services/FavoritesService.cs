using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MusicAlbum_Explorer.Models;
using Microsoft.Maui.Storage;

namespace MusicAlbum_Explorer.Services
{
    public static class FavoritesService
    {
        const string PrefKey = "favorites_v1";
        static List<FavoriteSongInfo> _favorites;

        static FavoritesService()
        {
            Load();
        }

        static void Load()
        {
            try
            {
                var json = Preferences.Get(PrefKey, string.Empty);
                if (string.IsNullOrEmpty(json))
                {
                    _favorites = new List<FavoriteSongInfo>();
                }
                else
                {
                    _favorites = JsonSerializer.Deserialize<List<FavoriteSongInfo>>(json) ?? new List<FavoriteSongInfo>();
                }
            }
            catch
            {
                _favorites = new List<FavoriteSongInfo>();
            }
        }

        static void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(_favorites);
                Preferences.Set(PrefKey, json);
            }
            catch
            {
                // ignore
            }
        }

        public static IReadOnlyList<FavoriteSongInfo> GetAll() => _favorites.AsReadOnly();

        public static bool IsFavoriteId(string id) => !string.IsNullOrEmpty(id) && _favorites.Any(f => f.Id == id);

        public static void AddFavorite(Song song, string albumTitle = null, string artistName = null, string cover = null)
        {
            if (song == null) return;
            if (IsFavoriteId(song.Id)) return;

            var info = new FavoriteSongInfo
            {
                Id = song.Id,
                Title = song.Title,
                Album = albumTitle,
                Artist = artistName,
                YouTubeUrl = song.YouTubeUrl,
                Duration = song.Duration == System.TimeSpan.Zero ? string.Empty : $"{(int)song.Duration.TotalMinutes}:{song.Duration.Seconds:D2}",
                Cover = cover
            };

            _favorites.Add(info);
            Save();
            OnChanged?.Invoke();
        }

        public static void RemoveFavoriteById(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            var existing = _favorites.FirstOrDefault(f => f.Id == id);
            if (existing == null) return;
            _favorites.Remove(existing);
            Save();
            OnChanged?.Invoke();
        }

        public static void ToggleFavorite(Song song, string albumTitle = null, string artistName = null, string cover = null)
        {
            if (song == null) return;
            if (IsFavoriteId(song.Id))
                RemoveFavoriteById(song.Id);
            else
                AddFavorite(song, albumTitle, artistName, cover);
        }

        public static event Action OnChanged;
    }
}
