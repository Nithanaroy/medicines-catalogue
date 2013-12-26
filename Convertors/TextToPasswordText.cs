using System;
using System.Text.RegularExpressions;
using System.Windows.Data;
using MedicinesCatalogue.Lib;

namespace MedicinesCatalogue.Convertors
{
    /// <summary>
    /// Used in SettingsPage for password as I was unable to use PasswordBox. It doesn't have Number as InputScope
    /// TODO: A better way to handle for PasswordBox
    /// </summary>
    public class TextToPasswordText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Regex.Replace(value.ToString(), @".", ApplicationHelper.passwordChar);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
