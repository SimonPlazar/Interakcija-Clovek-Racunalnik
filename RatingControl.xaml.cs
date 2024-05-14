using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SongDB
{
    /// <summary>
    /// Interaction logic for RatingControl.xaml
    /// </summary>
    public partial class RatingControl : UserControl
    {
        public static readonly DependencyProperty CurrentRatingProperty =
            DependencyProperty.Register("CurrentRating", typeof(int), typeof(RatingControl), new PropertyMetadata(0));

        public int CurrentRating
        {
            get { return (int)GetValue(CurrentRatingProperty); }
            set { SetValue(CurrentRatingProperty, value); }
        }

        public RatingControl()
        {
            InitializeComponent();
            RatingValues = Enumerable.Range(1, 5).ToList(); // Za 5 zvezdic
        }

        public List<int> RatingValues { get; }

        private void Rate_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var rating = (int)button.DataContext;
            CurrentRating = rating;
            RaiseRatingChangedEvent(rating);
        }

        // Dogodek
        public event EventHandler<RatingChangedEventArgs> RatingChanged;

        protected virtual void RaiseRatingChangedEvent(int newRating)
        {
            RatingChanged?.Invoke(this, new RatingChangedEventArgs(newRating));
        }
    }

    public class RatingChangedEventArgs : EventArgs
    {
        public int NewRating { get; }

        public RatingChangedEventArgs(int newRating)
        {
            NewRating = newRating;
        }
    }

}
