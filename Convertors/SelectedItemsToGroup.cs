using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using MedicinesCatalogue.Data;

namespace MedicinesCatalogue.Convertors
{
    public class SelectedItemsToGroup : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        /// <summary>
        /// Converts object type to ObservableCollection<Group> type
        /// This explicit conversion is required only if user changed the selection
        /// If user didn't change any option, casting fails, hence the Try-Catch
        /// </summary>
        /// <returns>Observable collection of MedicinesCatalogue.Data.Group type</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var objectGroups = (ObservableCollection<object>)value;
                ObservableCollection<Group> groups = new ObservableCollection<Group>();
                foreach (var group in objectGroups)
                    groups.Add((Group)group);
                return groups;
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}
