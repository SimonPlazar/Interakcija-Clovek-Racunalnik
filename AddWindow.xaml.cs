using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TagLib;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TagLib.Flac;

namespace SongDB
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private ObservableCollection<string> AvailableGenres;

        public AddWindow()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        public BitmapImage LoadImageFromBytes(IPicture picture)
        {
            if (picture == null)
                return null;

            using (var ms = new MemoryStream(picture.Data.Data))
            {
                var image = new BitmapImage();
                ms.Position = 0;  // Pomembno je, da nastavite položaj na začetek toka
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();  // Pomembno za uporabo slike v več nitih
                return image;
            }
        }

        private MusicTrack? ReadSongData(string filePath)
        {
            try
            {
                // Naloži datoteko z uporabo TagLib
                var file = TagLib.File.Create(filePath);

                // Pridobi metapodatke
                string title = file.Tag.Title;  // Naslov skladbe
                string album = file.Tag.Album;  // Album
                string artists = string.Join(", ", file.Tag.Performers);  // Izvajalci
                TimeSpan duration = file.Properties.Duration;  // Trajanje skladbe
                int time = duration.Seconds+duration.Minutes*60;
                string fileFormat = System.IO.Path.GetExtension(filePath);  // Format datoteke
                int year = (int)file.Tag.Year;  // Leto izdaje
                string genre = file.Tag.FirstGenre;  // Žanr
                int bitrate = file.Properties.AudioBitrate;  // Bitrate
                IPicture[] pictures = file.Tag.Pictures;

                // Zapri datoteko
                file.Dispose();

                // Ustvari nov objekt MusicTrack
                MusicTrack newTrack = new MusicTrack(artists, title, album, genre, year);
                newTrack.Format = fileFormat;
                newTrack.Length = time;
                newTrack.PathMusic = filePath;
                newTrack.Bitrate = bitrate;
                newTrack.pathImage = null;

                if (pictures.Length > 0)
                {
                    newTrack.BitmapImage = LoadImageFromBytes(pictures[0]);
                }

                return newTrack;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void ImportSong_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "FLAC datoteke (*.flac)|*.flac";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                MusicTrack? newMusic = ReadSongData(filePath);
                if (newMusic != null){
                    ((MusicViewModel)DataContext).NewMusicTrack = newMusic;
                    //((MusicViewModel)DataContext).MusicTracks.Add(newMusic);
                }

                DialogResult = true;
            }
        }

        private void ImportAlbum_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Slikovne datoteke (*.png;*.jpg)|*.png;*.jpg";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                //((MusicViewModel)DataContext).NewMusicTrack.pathImage = filePath;
                ((MusicViewModel)DataContext).NewMusicTrack.BitmapImage = new BitmapImage(new Uri(filePath));
            }
        }
    }
}
