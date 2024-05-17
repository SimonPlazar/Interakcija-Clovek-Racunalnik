using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SongDB
{
    public partial class RatingControl : UserControl
    {
        public event EventHandler<RatingChangedEventArgs> RatingChanged;
        public ICommand StarClickedCommand { get; private set; }

        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register("Rating", typeof(int), typeof(RatingControl),
                new PropertyMetadata(1, OnRatingChanged));

        public int Rating
        {
            get { return (int)GetValue(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }

        public RatingControl()
        {
            InitializeComponent();
            this.DataContext = this;

            //Rating = 0;
            Rating = (int)GetValue(RatingProperty);

            StarClickedCommand = new Command(param => StarClicked(param));

            UpdateStars();
        }

        private void StarClicked(object param)
        {
            int rating = int.Parse(param.ToString());
            Rating = rating;
            RatingChanged?.Invoke(this, new RatingChangedEventArgs(rating, this.Tag));
        }

        private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RatingControl;
            control?.UpdateStars();
        }

        private void UpdateStars()
        {
            Star1Color = Rating >= 1 ? Brushes.Gold : Brushes.Gray;
            Star2Color = Rating >= 2 ? Brushes.Gold : Brushes.Gray;
            Star3Color = Rating >= 3 ? Brushes.Gold : Brushes.Gray;
            Star4Color = Rating >= 4 ? Brushes.Gold : Brushes.Gray;
            Star5Color = Rating >= 5 ? Brushes.Gold : Brushes.Gray;
        }

        public Brush Star1Color
        {
            get { return (Brush)GetValue(Star1ColorProperty); }
            set { SetValue(Star1ColorProperty, value); }
        }

        public static readonly DependencyProperty Star1ColorProperty =
            DependencyProperty.Register("Star1Color", typeof(Brush), typeof(RatingControl), new PropertyMetadata(Brushes.Gray));

        public Brush Star2Color
        {
            get { return (Brush)GetValue(Star2ColorProperty); }
            set { SetValue(Star2ColorProperty, value); }
        }

        public static readonly DependencyProperty Star2ColorProperty =
            DependencyProperty.Register("Star2Color", typeof(Brush), typeof(RatingControl), new PropertyMetadata(Brushes.Gray));

        public Brush Star3Color
        {
            get { return (Brush)GetValue(Star3ColorProperty); }
            set { SetValue(Star3ColorProperty, value); }
        }

        public static readonly DependencyProperty Star3ColorProperty =
            DependencyProperty.Register("Star3Color", typeof(Brush), typeof(RatingControl), new PropertyMetadata(Brushes.Gray));

        public Brush Star4Color
        {
            get { return (Brush)GetValue(Star4ColorProperty); }
            set { SetValue(Star4ColorProperty, value); }
        }

        public static readonly DependencyProperty Star4ColorProperty =
            DependencyProperty.Register("Star4Color", typeof(Brush), typeof(RatingControl), new PropertyMetadata(Brushes.Gray));

        public Brush Star5Color
        {
            get { return (Brush)GetValue(Star5ColorProperty); }
            set { SetValue(Star5ColorProperty, value); }
        }

        public static readonly DependencyProperty Star5ColorProperty =
            DependencyProperty.Register("Star5Color", typeof(Brush), typeof(RatingControl), new PropertyMetadata(Brushes.Gray));
    }
}
