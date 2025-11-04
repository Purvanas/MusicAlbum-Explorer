using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using MusicAlbum_Explorer.Models;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Linq;

namespace MusicAlbum_Explorer.ViewModels
{
    public class ArtistListViewModel : BindableObject
    {
        public ObservableCollection<Artist> Artists { get; } = new ObservableCollection<Artist>();
        public ObservableCollection<Artist> FilteredArtists { get; } = new ObservableCollection<Artist>();

        public ObservableCollection<MusicType> AvailableMusicTypes { get; } = new ObservableCollection<MusicType>();
        public ObservableCollection<YearOption> AvailableYears { get; } = new ObservableCollection<YearOption>();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSearchNotEmpty));
                ApplyFilters();
            }
        }

        public bool IsSearchNotEmpty => !string.IsNullOrWhiteSpace(SearchText);

        private MusicType _selectedMusicType;
        public MusicType SelectedMusicType
        {
            get => _selectedMusicType;
            set
            {
                if (_selectedMusicType == value) return;
                _selectedMusicType = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private YearOption _selectedYearOption;
        public YearOption SelectedYearOption
        {
            get => _selectedYearOption;
            set
            {
                if (_selectedYearOption == value) return;
                _selectedYearOption = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private Artist _selectedArtist;
        public Artist SelectedArtist
        {
            get => _selectedArtist;
            set
            {
                if (_selectedArtist == value) return;
                _selectedArtist = value;
                OnPropertyChanged();
            }
        }

        public ICommand ArtistTappedCommand { get; }
        public ICommand AlbumTappedCommand { get; }

        // New clear/reset commands
        public ICommand ClearSearchCommand { get; }
        public ICommand ClearMusicTypeCommand { get; }
        public ICommand ClearYearCommand { get; }

        public ArtistListViewModel()
        {
            // Sample music types
            var pop = new MusicType { Name = "Pop" };
            var electro = new MusicType { Name = "Electro" };
            var folk = new MusicType { Name = "Folk" };

            // Add a null/placeholder option for music types
            AvailableMusicTypes.Add(new MusicType { Name = "Tous" });
            AvailableMusicTypes.Add(pop);
            AvailableMusicTypes.Add(electro);
            AvailableMusicTypes.Add(folk);

            // Sample data
            Artists.Add(new Artist {
                Stagename = "Céline Dion",
                Firstname = "Céline",
                Name = "Dion",
                Photo = "celinedion.jpg",
                Biographie = "Chanteuse canadienne connue pour sa voix puissante.",
                MusicType = new List<MusicType> { pop },
                Albums = new List<Album>
                {
                    new Album
                    {
                        Title = "Let's Talk About Love",
                        Year = 1997,
                        Cover = "celine_album1.jpg",
                        Songs = new List<Song>
                        {
                            new Song { Title = "My Heart Will Go On", Duration = new TimeSpan(0,4,40), YouTubeUrl = "https://www.youtube.com/watch?v=FHG2oizTlpY" }
                        }
                    }
                }
            });

            Artists.Add(new Artist {
                Stagename = "Stromae",
                Firstname = "Paul",
                Name = "Van Haver",
                Photo = "stromae.jpg",
                Biographie = "Auteur-compositeur belge mélangeant pop et électro.",
                MusicType = new List<MusicType> { pop, electro },
                Albums = new List<Album>
                {
                    new Album
                    {
                        Title = "Racine Carrée",
                        Year = 2013,
                        Cover = "stromae_album1.jpg",
                        Songs = new List<Song>
                        {
                            new Song { Title = "Alors on danse", Duration = new TimeSpan(0,3,27), YouTubeUrl = "https://www.youtube.com/watch?v=VHoT4N43jK8" }
                        }
                    }
                }
            });

            Artists.Add(new Artist {
                Stagename = "Ed Sheeran",
                Firstname = "Edward",
                Name = "Christopher Sheeran",
                Photo = "edsheeran.jpg",
                Biographie = "Chanteur britannique pop/folk.",
                MusicType = new List<MusicType> { pop, folk },
                Albums = new List<Album>
                {
                    new Album
                    {
                        Title = "÷ (Divide)",
                        Year = 2017,
                        Cover = "edsheeran_album1.jpg",
                        Songs = new List<Song>
                        {
                            new Song { Title = "Shape of You", Duration = new TimeSpan(0,3,53), YouTubeUrl = "https://www.youtube.com/watch?v=JGwWNGJdvx8" }
                        }
                    }
                }
            });

            // Populate year filter options with a "Tous" placeholder first
            var years = Artists.SelectMany(a => a.Albums).Select(al => al.Year).Distinct().OrderBy(y => y);
            AvailableYears.Add(new YearOption { Year = null, Display = "Tous" });
            foreach (var y in years) AvailableYears.Add(new YearOption { Year = y, Display = y.ToString() });

            // default selection: Tous
            SelectedYearOption = AvailableYears.FirstOrDefault();
            SelectedMusicType = AvailableMusicTypes.FirstOrDefault();

            ApplyFilters();

            ArtistTappedCommand = new Command<Artist>(async (artist) =>
            {
                if (artist == null) return;
                // Pass the full object to the details page
                await Shell.Current.GoToAsync(nameof(Views.ArtistDetailsPage), true, new Dictionary<string, object>
                {
                    { "artist", artist }
                });
            });

            AlbumTappedCommand = new Command<Album>(async (album) =>
            {
                if (album == null) return;
                await Shell.Current.GoToAsync(nameof(Views.AlbumSongsPage), true, new Dictionary<string, object>
                {
                    { "album", album }
                });
            });

            // Clear/reset commands
            ClearSearchCommand = new Command(() => { SearchText = string.Empty; });
            ClearMusicTypeCommand = new Command(() => { SelectedMusicType = AvailableMusicTypes.FirstOrDefault(); });
            ClearYearCommand = new Command(() => { SelectedYearOption = AvailableYears.FirstOrDefault(); });
        }

        private void ApplyFilters()
        {
            var query = Artists.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var txt = SearchText.Trim().ToLowerInvariant();
                query = query.Where(a => (a.Stagename ?? string.Empty).ToLowerInvariant().Contains(txt) || (a.Name ?? string.Empty).ToLowerInvariant().Contains(txt) || (a.Firstname ?? string.Empty).ToLowerInvariant().Contains(txt));
            }

            if (SelectedMusicType != null && SelectedMusicType.Name != "Tous")
            {
                query = query.Where(a => a.MusicType != null && a.MusicType.Any(mt => mt.Name == SelectedMusicType.Name));
            }

            if (SelectedYearOption != null && SelectedYearOption.Year.HasValue)
            {
                query = query.Where(a => a.Albums != null && a.Albums.Any(al => al.Year == SelectedYearOption.Year.Value));
            }

            FilteredArtists.Clear();
            foreach (var a in query) FilteredArtists.Add(a);
        }
    }
}