using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SplashGoJunpro.Converters
{
    /// <summary>
    /// Converter untuk mengubah boolean bookmark status menjadi warna
    /// True (bookmarked) = Biru, False (not bookmarked) = Abu-abu
    /// </summary>
    public class BoolToBookmarkColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isBookmarked && isBookmarked)
            {
                // Warna biru saat di-bookmark
                return new SolidColorBrush(Color.FromRgb(0, 102, 204)); // #0066CC
            }

            // Warna abu-abu saat tidak di-bookmark
            return new SolidColorBrush(Color.FromRgb(102, 102, 102)); // #666666
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}