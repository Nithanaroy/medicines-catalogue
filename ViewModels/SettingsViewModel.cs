using System;
using System.Collections.Generic;
using System.ComponentModel;
using MedicinesCatalogue.Data;
using MedicinesCatalogue.Lib;

namespace MedicinesCatalogue.ViewModels
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
            Settings = App.Settings;
        }

        private Settings settings;
        public Settings Settings
        {
            get { return settings; }
            set
            {
                if (value != settings)
                {
                    settings = value;
                }
            }
        }

        /// <summary>
        /// Saves all settings to IsolatedStorage
        /// </summary>
        public void Save()
        {
            ApplicationHelper.SetValueForSetting(ApplicationHelper.AvailableSettings.HasPassword, this.Settings.HasPassword);
            ApplicationHelper.SetValueForSetting(ApplicationHelper.AvailableSettings.Password, this.Settings.Password);
        }
    }
}
