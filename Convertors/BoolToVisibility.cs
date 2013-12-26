using System;
using System.Windows.Data;

namespace MedicinesCatalogue.Convertors
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (System.Windows.Visibility)value == System.Windows.Visibility.Visible ? true : false;
        }
    }
}
