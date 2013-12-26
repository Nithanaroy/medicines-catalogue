using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MedicinesCatalogue.Lib
{
    /// <summary>
    /// Common implementations of INotifyPropertyChanging, INotifyPropertyChanged, INotifyDataErrorInfo
    /// Inherited in ViewModels and Models
    /// </summary>
    public class NotifyPropertyChangeDataError : INotifyPropertyChanging, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanging(string p)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, new PropertyChangingEventArgs(p));
        }
        internal void NotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        /// <summary>
        /// Adapter Design Patter: Input => Errors from Validation Class viz List<string>
        /// Output => Add each error to ErrorList
        /// </summary>
        /// <param name="fieldName">Key for ErrorList</param>
        /// <param name="errorsFromValidationClass">List of errors passed from Validations Class</param>
        /// <returns>Whether any errors are present or not</returns>
        internal bool PopulateErrorListAdapter(string fieldName, List<string> errorsFromValidationClass)
        {
            bool hasErrors = false;
            errorList.Remove(fieldName);
            if (errorsFromValidationClass.Count > 0)
            {
                errorList.Add(fieldName, errorsFromValidationClass);
                if (ErrorsChanged != null)
                    ErrorsChanged(this, new DataErrorsChangedEventArgs(fieldName));
                hasErrors = true;
            }
            return hasErrors;
        }

        /// <summary>
        /// Has the list of errors for each field
        /// </summary>
        internal Dictionary<string, List<string>> errorList = new Dictionary<string, List<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (!errorList.ContainsKey(propertyName))
                return string.Empty;
            return errorList[propertyName];
        }
        public bool HasErrors
        {
            get { return errorList.Count > 0; }
        }
    }
}
