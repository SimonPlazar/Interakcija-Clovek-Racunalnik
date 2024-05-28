using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SongDB
{
    internal class SettingsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> AvailableGenres { get; set; }
        public ICommand AddGenre { get; private set; }
        public ICommand DeleteGenre { get; private set; }
        public ICommand EditGenre { get; private set; }
        public ICommand SaveSettingsCommand { get; private set; }

        public SettingsViewModel()
        {
            //AvailableGenres = new ObservableCollection<string> { "Action", "Comedy", "Drama", "Fantasy", "Horror", "Romance", "SciFi", "Thriller" };
            AvailableGenres = LoadGenres();
            LoadSettings();
            AddGenre = new Command(
                execute: (param) => AddGenreCommand(this),
                canExecute: (param) => !string.IsNullOrEmpty(TextBoxInput));
            DeleteGenre = new Command(
                execute: (param) => DeleteGenreCommand(this),
                canExecute: (param) => !string.IsNullOrEmpty(SelectedGenre));
            EditGenre = new Command(
                execute: (param) => EditGenreCommand(this),
                canExecute: (param) => !string.IsNullOrEmpty(SelectedGenre) && !string.IsNullOrEmpty(TextBoxInput));
            
            SaveSettingsCommand = new Command(
                execute: (param) => SaveSettings(),
                canExecute: (param) => true
                );

        }
        public ObservableCollection<string> LoadGenres()
        {
            var genresCollection = Properties.Settings.Default["Genres"] as System.Collections.Specialized.StringCollection;
            //var genresCollection = Properties.Settings.Default.Genres;
            var genres = new ObservableCollection<string>();
            if (genresCollection != null)
            {
                foreach (var genre in genresCollection)
                {
                    genres.Add(genre);
                }
            }
            return genres;
        }

        public void AddGenreCommand(object? obj)
        {
            if (!AvailableGenres.Contains(TextBoxInput))
            {
                AvailableGenres.Add(TextBoxInput);
                TextBoxInput = "";
                SaveGenres();
            }
            
        }

        public void DeleteGenreCommand(object? obj)
        {
            if (SelectedGenre != null)
            {
                AvailableGenres.Remove(SelectedGenre);
                SelectedGenre = null;
                SaveGenres();
            }
        }

        private void EditGenreCommand(object? obj)
        {
            if (!AvailableGenres.Contains(TextBoxInput))
            {
                DeleteGenreCommand(obj);
                AddGenreCommand(obj);
            }
        }

        private void SaveGenres()
        {
            var genresCollection = Properties.Settings.Default["Genres"] as System.Collections.Specialized.StringCollection;

            // Če ni inicializiran
            if (genresCollection == null)
            {
                genresCollection = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default["Genres"] = genresCollection;
            }
            else
            {
                genresCollection.Clear();
            }

            // Dodajanje novih žanrov v seznam
            foreach (var genre in AvailableGenres)
            {
                genresCollection.Add(genre);
            }

            Properties.Settings.Default.Save();
        }


        private string _selectedGenre;
        public string SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                if (_selectedGenre != value)
                {
                    _selectedGenre = value;
                    OnPropertyChanged(nameof(SelectedGenre));
                }
            }
        }

        private string _textBoxInput;
        public string TextBoxInput
        {
            get => _textBoxInput;
            set
            {
                _textBoxInput = value;
                OnPropertyChanged(nameof(TextBoxInput));
            }
        }

        private bool isAutoSaveEnabled;
        public bool IsAutoSaveEnabled
        {
            get => isAutoSaveEnabled;
            set
            {
                isAutoSaveEnabled = value;
                OnPropertyChanged(nameof(IsAutoSaveEnabled));
            }
        }

        private int autoSaveInterval;
        public int AutoSaveInterval
        {
            get => autoSaveInterval;
            set
            {
                autoSaveInterval = value;
                OnPropertyChanged(nameof(AutoSaveInterval));
            }
        }

        public bool IsAutoSaveIntervalEnabled => IsAutoSaveEnabled;

        private void LoadSettings()
        {
            IsAutoSaveEnabled = Properties.Settings.Default.IsAutoSaveEnabled;
            AutoSaveInterval = Properties.Settings.Default.AutoSaveInterval;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.IsAutoSaveEnabled = IsAutoSaveEnabled;
            Properties.Settings.Default.AutoSaveInterval = AutoSaveInterval;
            Properties.Settings.Default.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
