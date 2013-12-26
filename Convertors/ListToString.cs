using System;
using System.Collections.Generic;
using System.Windows.Data;
using MedicinesCatalogue.Data;

namespace MedicinesCatalogue.Convertors
{
    public class ListToString : IValueConverter
    {
        /// <summary>
        /// Used in DetailsPage.xaml to aggregate all the groups
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var groups = value as IList<Group>;
            if (groups.Count == 0)
                return null;
            return String.Join(", ", groups);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
