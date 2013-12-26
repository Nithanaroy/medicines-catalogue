using System;
using System.Windows.Data;

namespace MedicinesCatalogue.Convertors
{
    public class StingToDateTime : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // backend to datepicker
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // datepicker to backend
            if (value == null)
                throw new ArgumentException("Date can't be blank");
            DateTime pickedDate = (DateTime)value;
            return pickedDate;
        }

        #endregion
    }
}
