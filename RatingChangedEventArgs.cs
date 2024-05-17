namespace SongDB;

public class RatingChangedEventArgs : EventArgs
{
    public int NewRating { get; }

    public RatingChangedEventArgs(int newRating, object tag)
    {
        NewRating = newRating;
    }
}