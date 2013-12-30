using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

// Reference the data model.
using AppAgentAdapter.Data;
using System;

namespace MedicinesCatalogue.ViewModels
{
    public class QuickCardTargetPageViewModel : INotifyPropertyChanged
    {
        MedicineContext db;

        // Observable collection for the deep link URI parameters.
        private ObservableCollection<Medicine> _QuickCardUriParameters;
        public ObservableCollection<Medicine> QuickCardUriParameters
        {
            get { return _QuickCardUriParameters; }
            set
            {
                if (_QuickCardUriParameters != value)
                {
                    _QuickCardUriParameters = value;
                    NotifyPropertyChanged("QuickCardUriParameters");
                }
            }
        }

        // Class constructor.
        public QuickCardTargetPageViewModel()
        {
            // Create observable collection object.
            QuickCardUriParameters = new ObservableCollection<Medicine>();
        }

        // Load parameters from quick page; extract from the NavigationContext.QueryString
        public void LoadUriParameters(IDictionary<string, string> QueryString)
        {
            // Clear parameters in the ViewModel.
            QuickCardUriParameters.Clear();
            db = App.ViewModel.db;

            // Loop through the quick card parameters in the deep link URI.
            foreach (string strKey in QueryString.Keys)
            {
                // Set default value for parameter if no value is present.
                string strKeyValue = "<no value present in URI>";

                // Try to extract parameter value from URI.
                QueryString.TryGetValue(strKey, out strKeyValue);

                // Add parameter object to ViewModel collection.
                QuickCardUriParameters.Add(db.Medicines.SingleOrDefault(m => m.Name.ToLower().Contains(strKeyValue.ToLower())));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}