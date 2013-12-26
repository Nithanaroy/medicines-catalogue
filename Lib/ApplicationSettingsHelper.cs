using System.IO.IsolatedStorage;

namespace MedicinesCatalogue.Lib
{
    /// <summary>
    /// Saves or retrieves settings in IsolatedStorage
    /// </summary>
    /// <typeparam name="T">Type of setting</typeparam>
    public class ApplicationSettingsHelper<T>
    {
        string tag;
        T defaultValue;

        public ApplicationSettingsHelper(string tag, T defaultValue)
        {
            this.tag = tag;
            this.defaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                T valueFromIsloatedStorage;
                if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue(tag, out valueFromIsloatedStorage))
                {
                    IsolatedStorageSettings.ApplicationSettings[tag] = defaultValue;
                    return defaultValue;
                }
                return valueFromIsloatedStorage;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[tag] = value;
            }
        }
    }
}
