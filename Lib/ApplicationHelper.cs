using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;
using MedicinesCatalogue.Data;
using System.Data.Linq;

namespace MedicinesCatalogue.Lib
{
    class ApplicationHelper
    {
        /// <summary>
        /// Used for error messages with invalid arguments. Has to specify a value for {0} placeholder
        /// </summary>
        public static string errorInvalidArgumentMessage = "Invalid {0} specified";

        /// <summary>
        /// Generic error message
        /// </summary>
        public static string errorGenericMessage = "Something went wrong. Please report to us in About page. Thanks.";

        //public static Size MaxImageDimensions = new Size(768, 1280);

        /// <summary>
        /// Relative URL for default image for a medicine
        /// </summary>
        public static string defaultImageURL = "/Images/Default.jpg";

        /// <summary>
        /// A BITMAP version of the default image. Used for Binding with Image Source in XAML
        /// </summary>
        public static BitmapImage defaultImage = new BitmapImage(new Uri(defaultImageURL, UriKind.Relative));

        /// <summary>
        /// Have to manually update this value on every release
        /// </summary>
        public static float currentApplicationVersion = 2.0F;

        /// <summary>
        /// The password masking character to be used everywhere
        /// </summary>
        public static readonly string passwordChar = "●";

        /// <summary>
        /// Converts a list of strings into a single string with multiple lines
        /// Currently Used: For displaying error messages
        /// </summary>
        /// <param name="source">List of strings to be converted</param>
        /// <returns>String with each list item on a separate line</returns>
        public static string GetStringFromListItems(List<string> source)
        {
            StringBuilder resultString = new StringBuilder();
            foreach (string item in source)
                resultString.AppendLine(item);
            return resultString.ToString();
        }

        /// <summary>
        /// Converts an empty string to NULL
        /// </summary>
        /// <param name="text">Source string to convert</param>
        /// <returns>true if converted</returns>
        public static bool ConvertToNullIfEmpty(ref string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                text = null;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Pages in the app we can be navigate to
        /// </summary>
        public enum URLs
        {
            MainPage,
            DetailsPage,
            AboutPage,
            AddPage,
            UpdatePage,
            SettingsPage,
            AddReminderPage
        };
        /// <summary>
        /// A single place to obtain URL for any page in the app
        /// Currently Used: In all navigation services of the page
        /// </summary>
        /// <param name="urlFor">Enumerated value of page for which URL is required</param>
        /// <returns>Relative URL string</returns>
        public static string GetUrlFor(URLs urlFor)
        {
            string URL = "";
            switch (urlFor)
            {
                case URLs.MainPage: URL = "/Views/MainPage.xaml";
                    break;
                case URLs.DetailsPage: URL = "/Views/DetailsPage.xaml?medicineId=";
                    break;
                case URLs.AboutPage: URL = "/Views/About.xaml";
                    break;
                case URLs.AddPage: URL = "/Views/AddUpdateMedicine.xaml";
                    break;
                case URLs.UpdatePage: URL = "/Views/AddUpdateMedicine.xaml?medicineId=";
                    break;
                case URLs.SettingsPage: URL = "/Views/Settings.xaml";
                    break;
                case URLs.AddReminderPage: URL = "/Views/AddReminders.xaml?medicineId=";
                    break;
                default: goto case URLs.MainPage;
            }
            return URL;
        }


        /// <summary>
        /// Occassions when an email is sent
        /// </summary>
        public enum EmailFor
        {
            FeedBack,
            Error
        }
        /// <summary>
        /// Email sender. Uses Microsoft email Task
        /// Currently Used: Sending Feedback and error mails
        /// </summary>
        /// <param name="emailFor">Enumerated reason for sending the mail</param>
        /// <param name="message">Message if any to include in the mail</param>
        public static void SendMail(EmailFor emailFor, string message = "")
        {
            Microsoft.Phone.Tasks.EmailComposeTask emailTask = new Microsoft.Phone.Tasks.EmailComposeTask();
            switch (emailFor)
            {
                case EmailFor.FeedBack:
                    emailTask.Subject = "Feedback :: My Medicine Catalogue v" + currentApplicationVersion;
                    break;
                case EmailFor.Error:
                    emailTask.Subject = "Error :: My Medicine Catalogue v" + currentApplicationVersion;
                    break;
                default: goto case EmailFor.FeedBack;
            }
            emailTask.To = "nithanaroy@hotmail.com";
            emailTask.Body = message;
            emailTask.Show();
        }

        /// <summary>
        /// Genarates a unique name for JPG image which is saved in IsloatedStorage
        /// </summary>
        /// <param name="medicineId">Medicine ID for which filename is required</param>
        /// <returns>Unique filename for this medicine ID</returns>
        public static string GetImageFilenameInIsloatedStorage(int medicineId)
        {
            return medicineId + ".jpg";
        }

        /// <summary>
        /// Retrieves the BITMAP image for a file given from IsolatedStorage
        /// </summary>
        /// <param name="filename">Name of file as saved in IsloatedStorage</param>
        /// <returns>BITMAP_IMAGE if found else null</returns>
        public static BitmapImage GetImageFromIsolatedStorage(string filename)
        {
            using (var myStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                BitmapImage bm = null;
                try
                {
                    IsolatedStorageFileStream myFileStream = myStore.OpenFile(filename, FileMode.Open, FileAccess.Read);
                    bm = new BitmapImage();
                    bm.SetSource(myFileStream);
                    myFileStream.Close();
                }
                catch (IsolatedStorageException)
                {
                    // File not found. Happens when user has a default picture. Silently ignore
                }
                return bm;
            }
        }

        /// <summary>
        /// Deletes the image if it exists and then
        /// Saves new JPEG to IsolatedStorage
        /// </summary>
        /// <param name="filename">Name of the file to save the JPEG as</param>
        /// <param name="bitmap">The bitmap to save</param>
        public static void SaveImageInIsolatedStorage(string filename, WriteableBitmap bitmap)
        {
            DeleteImageInIsolatedStorage(filename);
            using (var myStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                IsolatedStorageFileStream myFileStream = myStore.CreateFile(filename);
                Extensions.SaveJpeg(bitmap, myFileStream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 85);
                myFileStream.Close();
            }
        }

        /// <summary>
        /// Rename a file in IsloatedStorage. Ignores if file specified is not found
        /// </summary>
        /// <param name="sourceFilename">Existing filename to be modified</param>
        /// <param name="destinationFilename">New name for the file</param>
        public static void RenameImageInIsolatedStorage(string sourceFilename, string destinationFilename)
        {
            using (var myStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myStore.FileExists(sourceFilename))
                {
                    if (myStore.FileExists(destinationFilename))
                        myStore.DeleteFile(destinationFilename);
                    myStore.MoveFile(sourceFilename, destinationFilename);
                }
            }
        }

        /// <summary>
        /// Deletes the file in IsolatedStorage if found. Ignores if file is not found
        /// </summary>
        /// <param name="filename">Name of the file to be deleted</param>
        public static void DeleteImageInIsolatedStorage(string filename)
        {
            using (var myStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myStore.FileExists(filename))
                    myStore.DeleteFile(filename);
            }
        }

        /// <summary>
        /// Maintains the list of default settings
        /// </summary>
        private static Dictionary<AvailableSettings, object> defaultSettings;

        /// <summary>
        /// List of all settings in the application
        /// </summary>
        public enum AvailableSettings
        {
            HasPassword,
            Password
        };

        /// <summary>
        /// Maintains the list of Default Settings of the Application
        /// </summary>
        /// <returns>A dictionary of Setting and Value</returns>
        public static Dictionary<AvailableSettings, object> GetDefaultSettings()
        {
            if (defaultSettings != null)
                return defaultSettings;

            defaultSettings = new Dictionary<AvailableSettings, object>();
            defaultSettings.Add(AvailableSettings.HasPassword, false);
            defaultSettings.Add(AvailableSettings.Password, "");
            return defaultSettings;
        }

        /// <summary>
        /// Only place from which any setting can be obtained. Retrieves from IsolatedStorage
        /// </summary>
        /// <param name="setting">Setting to retrieve</param>
        /// <returns>Value of the setting if found, else null</returns>
        public static object GetValueForSetting(AvailableSettings setting)
        {
            switch (setting)
            {
                case AvailableSettings.HasPassword:
                    return (new ApplicationSettingsHelper<Boolean>("HasPassword", (bool)defaultSettings[AvailableSettings.HasPassword])).Value;
                case AvailableSettings.Password:
                    return (new ApplicationSettingsHelper<String>("Password", defaultSettings[AvailableSettings.HasPassword].ToString())).Value;
            }
            return null;
        }

        /// <summary>
        /// Only place from which any setting can be set. Writes to IsolatedStorage
        /// </summary>
        /// <param name="setting">Setting to set</param>
        public static void SetValueForSetting(AvailableSettings setting, object value)
        {
            switch (setting)
            {
                case AvailableSettings.HasPassword:
                    (new ApplicationSettingsHelper<Boolean>("HasPassword", (bool)defaultSettings[AvailableSettings.HasPassword])).Value = (bool)value;
                    break;
                case AvailableSettings.Password:
                    (new ApplicationSettingsHelper<String>("Password", defaultSettings[AvailableSettings.HasPassword].ToString())).Value = value.ToString();
                    break;
            }
        }

        #region PivotTabs
        /// <summary>
        /// Tabs on the HomePage
        /// </summary>
        public enum TabsItems
        {
            All,
            Groups
        };

        private static Dictionary<TabsItems, string> tabs = new Dictionary<TabsItems, string>(2) 
        {
            { TabsItems.All, "all" },
            { TabsItems.Groups, "groups" }
        };

        public static Dictionary<TabsItems, string> Tabs { get { return tabs; } }
        #endregion

        /// <summary>
        /// Tells whether Database has changed indicating to trigger refresh all in memory objects
        /// </summary>
        /// <returns>true if database changed</returns>
        public static bool HasDbChanged(DataContext db)
        {
            ChangeSet changes = db.GetChangeSet();
            return (changes.Updates.Count > 0 || changes.Inserts.Count > 0 || changes.Deletes.Count > 0);
        }
    }
}
