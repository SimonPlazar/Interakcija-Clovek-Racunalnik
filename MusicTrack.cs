using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.VisualBasic.CompilerServices;

namespace SongDB;

public class MusicTrack : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public MusicTrack()
    {
        Artist = "(No Artist)";
        Title = "(No Title)";
        Album = "(No Album)";
        Genre = "(No Genre)";
        Year = 404;
        //pathImage = "resources/cover2.png";
    }

    public MusicTrack(string artist, string title, string album, string genre, int year)
    {
        Artist = artist;
        Title = title;
        Album = album;
        Genre = genre;
        Year = year;
        //pathImage = "resources/cover1.jpg";
    }

    private string artist;
    public string Artist
    {
        get { return artist; }
        set
        {
            artist = value;
            OnPropertyChanged("Artist");
        }
    }
    
    private string title;
    public string Title
    {
        get { return title; }
        set
        {
            title = value;
            OnPropertyChanged("Title");
        }
    }

    private string album;
    public string Album
    {
        get { return album; }
        set
        {
            if (value == "" || value == null) album = "(No Album)";
            else album = value;
            OnPropertyChanged("Album");
        }
    }

    private string genre;
    public string Genre
    {
        get { return genre; }
        set
        {
            genre = value;
            OnPropertyChanged("Genre");
        }
    }

    private int year;
    public int Year
    {
        get { return year; }
        set
        {
            if (value < 1900 || value > DateTime.Now.Year + 1)
            {
                year = 404;
            }
            else
            {
                year = value;
            }
            OnPropertyChanged("Year");
        }
    }

    private string? path_image;
    public string pathImage
    {
        get { return path_image; }
        set
        {
            if (File.Exists(value)) 
            { 
                path_image = value; 
                OnPropertyChanged("pathImage");
            }
        }
    }

    private BitmapImage? bitmap_image;

    [XmlIgnore]
    public BitmapImage BitmapImage
    {
        get
        {
            if (bitmap_image != null)
                return bitmap_image;
            if (File.Exists(path_image))
                return new BitmapImage(new Uri(path_image, UriKind.Relative));
            return new BitmapImage(new Uri("resources/cover1.jpg", UriKind.Relative));
        }
        set
        {
            bitmap_image = value;
            OnPropertyChanged("bitmapImage");
        }
    }

    private string format;
    public string Format
    {
        get { return format; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                format = "Unknown";
            else
                format = value;
            OnPropertyChanged("bitmapImage");
        }
    }

    private string? path_music;
    public string? PathMusic
    {
        get { return path_music; }
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !File.Exists(value))
                path_music = "Unknown";
            else
                path_music = value;
            OnPropertyChanged("PathMusic");
        }
    }

    private int? length;
    public int? Length
    {
        get { return length; }
        set
        {
            if (value <= 0)
                length = 0;
            else
                length = value;
            OnPropertyChanged("Length");
        }
    }

    private int? bitrate;
    public int? Bitrate
    {
        get { return bitrate; }
        set
        {
            if (value <= 0)
                bitrate = 0;
            else
                bitrate = value;
            OnPropertyChanged("Bitrate");
        }
    }

    private int? count_rating;
    private float? rating;
    public float? Rating
    {
        get { return rating; }
        set
        {
            count_rating ??= 0;
            rating ??= 0;

            if (value == 0)
                rating = 0;
            if (value < 0 || value > 5)
                ;
            else
            {
                count_rating++;
                rating += value / count_rating;
            }
        }
    }

    private bool? isFavorite;
    public bool? IsFavorite
    {
        get => isFavorite;
        set
        {
            if (isFavorite != value)
            {
                isFavorite = value;
                OnPropertyChanged(nameof(IsFavorite));
            }
        }
    }
    public string ConvertBitmapImageToBase64(BitmapImage bitmapImage)
    {
        if (bitmapImage == null) return null;

        PngBitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
        using (MemoryStream ms = new MemoryStream())
        {
            encoder.Save(ms);
            byte[] imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }
    }

    public BitmapImage ConvertBase64ToBitmapImage(string base64String)
    {
        if (string.IsNullOrEmpty(base64String)) return null;

        byte[] imageBytes = Convert.FromBase64String(base64String);
        using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
        {
            ms.Write(imageBytes, 0, imageBytes.Length);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = ms;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze(); // Pomembno za uporabo slike v več niti
            return image;
        }
    }

    public string? ImageBase64 { get; set; }
    // Kličite to metodo pred serializacijo
    public void PrepareForSerialization()
    {
        if (BitmapImage != null)
        {
            ImageBase64 = ConvertBitmapImageToBase64(BitmapImage);
        }
    }

    // Kličite to metodo po deserializaciji
    public void InitializeAfterDeserialization()
    {
        if (!string.IsNullOrEmpty(ImageBase64))
        {
            BitmapImage = ConvertBase64ToBitmapImage(ImageBase64);
        }
    }


    public override string ToString()
    {
        return $"{Artist} - {Title}";
    }

    public MusicTrack Clone()
    {
        MusicTrack newTrack = new MusicTrack();
        newTrack.UpdateForm(this);

        return newTrack;
    }

    public void UpdateForm(MusicTrack track)
    {
        Artist = track.Artist;
        Title = track.Title;
        Album = track.Album;
        Genre = track.Genre;
        Year = track.Year;

        if (track.pathImage != null)
            pathImage = track.pathImage;
        if (track.bitmap_image != null)
            BitmapImage = track.BitmapImage;
        if (track.format != null)
            Format = track.Format;
        if (track.path_music != null)
            PathMusic = track.PathMusic;
        if (track.length != null)
            Length = track.Length;
        if (track.bitrate != null)
            Bitrate = track.Bitrate;
        if (track.rating != null)
            Rating = track.Rating;
        if (track.isFavorite != null)
            IsFavorite = track.IsFavorite;
    }

    private void OnPropertyChanged(String propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}