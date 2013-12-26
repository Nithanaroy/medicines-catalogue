using System;
using System.Windows.Data;

namespace MedicinesCatalogue.Convertors
{
    /// <summary>
    /// Used in Details Page where we show whether user is using a medicine or not using the IsUsing property of Medicine
    /// </summary>
    public class BoolToCurrentlyUsingHelperText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? "You are currently using this medicine" : "You are not using this medicine currently";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
