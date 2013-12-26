using System;
using System.Collections.Generic;
using System.ComponentModel;
using MedicinesCatalogue.Lib;

namespace MedicinesCatalogue.Data
{
    public class Settings
    {
        /// <summary>
        /// Sets the properties to the default settings values
        /// </summary>
        public Settings()
        {
            Dictionary<ApplicationHelper.AvailableSettings, object> defaultSettings = ApplicationHelper.GetDefaultSettings();
            HasPassword = (Boolean)defaultSettings[ApplicationHelper.AvailableSettings.HasPassword];
            Password = defaultSettings[ApplicationHelper.AvailableSettings.Password].ToString();
        }

        /// <summary>
        /// Indicates whether user has a password set for Medicines Catalogue to open it
        /// </summary>
        private Boolean hasPassword;
        public Boolean HasPassword
        {
            get { return hasPassword; }
            set
            {
                if (value != hasPassword)
                {
                    hasPassword = value;
                }
            }
        }

        /// <summary>
        /// Stores the actual password string
        /// </summary>
        private String password;
        public String Password
        {
            get { return password; }
            set
            {
                if (value != password)
                {
                    value = value.Trim();
                    if (HasPassword && !Validations.Blank(value, "Password"))
                        throw new ArgumentException(Validations.Message);
                    password = value;
                }
            }
        }
    }
}
