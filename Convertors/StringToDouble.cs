using System;
using System.Windows.Data;

namespace MedicinesCatalogue.Convertors
{
    /// <summary>
    /// Used primarily for String from UI TextBox to a Nullable datatype in DB. Here for Price column
    /// </summary>
    public class StringToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // DB (double?) to UI (string)
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // UI (string) to DB (double?)
            if(string.IsNullOrWhiteSpace(value.ToString()))
                return null;
            else
                return System.Convert.ToDouble(value);
        }
    }
}
