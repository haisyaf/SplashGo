using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SplashGoJunpro.Converters
{
    /// <summary>
    /// Multi-value converter untuk placeholder visibility
    /// Hides placeholder jika textbox memiliki focus ATAU memiliki text
    /// </summary>
    public class PlaceholderVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] = text string (Email atau Password)
            // values[1] = IsFocused boolean

            if (values.Length != 2)
            return Visibility.Visible;

            string text = values[0] as string;
            bool isFocused = values[1] is bool && (bool)values[1];

            // Hide placeholder jika:
            // 1. TextBox sedang focus ATAU
            // 2. TextBox memiliki text
            if (isFocused || !string.IsNullOrEmpty(text))
            return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
