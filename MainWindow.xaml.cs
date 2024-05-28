using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Enumeration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using TagLib;

namespace SongDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer autoSaveTimer;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MusicViewModel();

            ObservableCollection<string> AvailableGenres = new ObservableCollection<string> { "Rap", "(No Genre)", "Hip-Hop", "Trap" };
            SaveGenres(AvailableGenres);

            InitializeAutoSaveTimer();
        }

        private void SaveGenres(ObservableCollection<string> AvailableGenres)
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

        private void Click_Izhod(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ListView_Item_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            if (listView.SelectedItem is MusicTrack musicTrack)
            {
                if (musicTrack != null)
                {
                    //StatusNotifTextBlock.Text = musicTrack.Title;
                    string messageBoxText = musicTrack.Title;
                    string caption = "Ime muzike :)";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;

                    MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                }
            }
            
        }
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.DataContext = new SettingsViewModel();
            settingsWindow.ShowDialog(); // Open as modal
        }
        public ObservableCollection<MusicTrack> LoadMusicData(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<MusicTrack>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (ObservableCollection<MusicTrack>)serializer.Deserialize(reader);
            }
        }

        private void Uvozi_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML datoteke (*.xml)|*.xml";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                var tracks = LoadMusicData(filePath);
                ((MusicViewModel)DataContext).MusicTracks.Clear();  // Izbriši trenutne skladbe

                foreach (var track in tracks)
                {
                    track.InitializeAfterDeserialization();  // Inicializiraj vsako skladbo
                    ((MusicViewModel)DataContext).MusicTracks.Add(track);  // Dodaj skladbo v obstoječo zbirko
                }
            }
        }

        public void SaveMusicData(string filePath, ObservableCollection<MusicTrack> musicTracks)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<MusicTrack>));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, musicTracks);
            }
        }

        private void Izvozi_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML datoteke (*.xml)|*.xml";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                var musicTracks = ((MusicViewModel)DataContext).MusicTracks;
                foreach (var track in musicTracks)
                    track.PrepareForSerialization();
                
                SaveMusicData(filePath, musicTracks);
            }
        }
        private void RatingControl_RatingChanged(object sender, RatingChangedEventArgs e)
        {
            if(e.Track is MusicTrack track && e.NewRating is int newRating)
            {
                track.Rating = newRating;
            }
        }

        private void InitializeAutoSaveTimer()
        {
            autoSaveTimer = new DispatcherTimer();
            autoSaveTimer.Tick += AutoSaveTimer_Tick;
            UpdateAutoSaveSettings();
        }

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            SaveMusicData("autosave.xml", ((MusicViewModel)DataContext).MusicTracks);
        }

        private void UpdateAutoSaveSettings()
        {
            if (Properties.Settings.Default.IsAutoSaveEnabled)
            {
                autoSaveTimer.Interval = TimeSpan.FromMinutes(Properties.Settings.Default.AutoSaveInterval);
                autoSaveTimer.Start();
            }
            else
            {
                autoSaveTimer.Stop();
            }
        }

        private void MoveOffScreenButton_Click(object sender, RoutedEventArgs e)
        {
            Storyboard moveButtonStoryboard = (Storyboard)this.FindResource("MoveButtonOffScreenAnimation");
            moveButtonStoryboard.Begin();
        }
    }
}