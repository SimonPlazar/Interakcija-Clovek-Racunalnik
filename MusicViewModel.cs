using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Input;

namespace SongDB;

public class MusicViewModel : INotifyPropertyChanged
{
    public ObservableCollection<MusicTrack> MusicTracks { get; set; }
    public MusicTrack NewMusicTrack { get; set; }
    public ObservableCollection<string> AvailableGenres { get; set; }
    private EditWindow editMusicWindow;

    public ICommand RatingChangedCommand { get; private set; }
    public ICommand ToggleFavoriteCommand { get; private set; }
    public ICommand AddMusicTrack { get; private set; }
    public ICommand EditMusicTrack { get; private set; }
    public ICommand DeleteTrack { get; private set; }
    public ICommand SaveCommand { get; private set; }


    public MusicViewModel()
    {
        MusicTracks = new ObservableCollection<MusicTrack>();
        NewMusicTrack = new MusicTrack();
        AvailableGenres = LoadGenres();

        AddMusicTrack = new Command(AddMusicTrackCommand);
        EditMusicTrack = new Command(
            execute: (param) => EditMusicTrackCommand(this),
            canExecute: (param) => !(SelectedTrack == null));
        DeleteTrack = new Command(
            execute: (param) => DeleteTrackCommand(this),
            canExecute: (param) => !(SelectedTrack == null));
        SaveCommand = new Command((param) => SaveChanges());
        ToggleFavoriteCommand = new Command(ToggleFavorite);
        RatingChangedCommand = new Command(param =>
        {
            if (param is object[] parameters && parameters.Length == 2)
            {
                ChangeRating(parameters[0], (int)parameters[1]);
            }
        });

        MusicTracks.Add(new MusicTrack("Artist1", "Title1", "Album1", "Rap", 2024));
        MusicTracks.Add(new MusicTrack("Artist2", "Title2", "Album2", "Rap", 2024));
        MusicTracks.Add(new MusicTrack("Artist3", "Title3", "Album3", "Rap", 2024));
    }

    private void ToggleFavorite(object parameter)
    {
        if (parameter is MusicTrack track)
        {
            track.IsFavorite = !track.IsFavorite;
        }
    }

    private void ChangeRating(object parameter, int rating)
    {
        if (parameter is MusicTrack track)
        {
            track.Rating = rating;
        }
    }

    private MusicTrack _selectedTrack;
    public MusicTrack SelectedTrack
    {
        get => _selectedTrack;
        set
        {
            if (_selectedTrack != value)
            {
                _selectedTrack = value;
                OnPropertyChanged(nameof(SelectedTrack));

                if (editMusicWindow != null && editMusicWindow.IsVisible)
                {
                    if (SelectedTrack != null)
                        EditableTrack = SelectedTrack.Clone();
                    else
                        EditableTrack = null;
                    editMusicWindow.DataContext = this; // Posodobite DataContext v oknu za urejanje
                }
            }
        }
    }

    private MusicTrack _editableTrack;
    public MusicTrack EditableTrack
    {
        get => _editableTrack;
        set
        {
            _editableTrack = value;
            OnPropertyChanged(nameof(EditableTrack));
        }
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

    private void AddMusicTrackCommand(object? obj)
    {
        AvailableGenres = LoadGenres();
        AddWindow addWindow = new AddWindow();
        addWindow.DataContext = new MusicViewModel();

        if (addWindow.ShowDialog() == true)
        {
            MusicTrack newTrack = ((MusicViewModel)addWindow.DataContext).NewMusicTrack;
            if (newTrack != null)
                MusicTracks.Add(newTrack);
        }
    }

    private void EditMusicTrackCommand(object? obj)
    {
        AvailableGenres = LoadGenres();
        if (editMusicWindow == null || !editMusicWindow.IsVisible)
        {
            EditableTrack = SelectedTrack.Clone();
            editMusicWindow = new EditWindow();
            editMusicWindow.DataContext = this;
            editMusicWindow.Owner = Application.Current.MainWindow; // Da bo okno vedno nad glavnim oknom
            editMusicWindow.Show();
            editMusicWindow.Closed += (sender, args) => editMusicWindow = null;
        }
        else
        {
            EditableTrack = SelectedTrack.Clone();
            editMusicWindow.Focus(); // Če je okno že odprto ga samo oživi
        }
    }

    private void SaveChanges()
    {
        if (SelectedTrack != null && EditableTrack != null)
        {
            SelectedTrack.UpdateForm(EditableTrack);
            SelectedTrack = null;
            EditableTrack = null;
            if (editMusicWindow != null)
                editMusicWindow.Close();
        }
    }

    private void DeleteTrackCommand(object? obj)
    {
        if (SelectedTrack != null)
        {
            MusicTracks.Remove(SelectedTrack);
            SelectedTrack = null;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}