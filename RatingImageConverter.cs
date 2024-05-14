using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SongDB
{
    public class RatingImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rating = (int)value;
            var index = System.Convert.ToInt32(parameter);

            return rating >= index ? "resources/icons/add_favorite.png" : "resources/icons/remove_favorite.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // V tej aplikaciji ne potrebujemo konverzije nazaj.
            throw new NotImplementedException();
        }
    }

}
