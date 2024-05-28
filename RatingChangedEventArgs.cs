namespace SongDB;

public class RatingChangedEventArgs : EventArgs
{
    public int NewRating { get; }
    public MusicTrack Track { get; }

    public RatingChangedEventArgs(MusicTrack track, int newRating)
    {
        NewRating = newRating;
        Track = track;
    }
}