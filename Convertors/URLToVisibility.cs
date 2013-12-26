using System;
using System.Windows;
using System.Windows.Data;
using MedicinesCatalogue.Lib;

namespace MedicinesCatalogue.Convertors
{
    public class URLToVisibility : IValueConverter
    {
        /// <summary>
        /// Returns visible if Image URL is not default image
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Image URL to Delete Image button Visibility
            return  value.ToString().Equals(ApplicationHelper.defaultImageURL) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
