using System;

namespace MedicinesCatalogue.Lib
{
    class SessionVariables
    {
        /// <summary>
        /// Used to track whether user is logged in
        /// If logged out password dialog is prompted
        /// </summary>
        private static Boolean isLoggedIn = App.Settings.HasPassword ? false : true;
        public static Boolean IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                if (!App.Settings.HasPassword)
                    isLoggedIn = true;
                else
                    isLoggedIn = value;
            }
        }
    }
}
