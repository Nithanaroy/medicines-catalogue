using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Resources;
using Microsoft.Phone.Controls;
using System.Windows;
using System;
using System.Collections.Generic;

namespace MedicinesCatalogue.Views
{
    public partial class BlinkEyesAlertPage : PhoneApplicationPage
    {
        public BlinkEyesAlertPage()
        {
            InitializeComponent();
            SaveFilesToIsoStore();
            //BlinkEyesBrowser.NavigateToString("<html><head><meta name='viewport' content='width=480, user-scalable=yes' /></head><body>HTML Text</body></html>");
            BlinkEyesBrowser.Navigate(new Uri("imageHTML.html", UriKind.Relative));
        }

        private void SaveFilesToIsoStore()
        {
            //These files must match what is included in the application package,
            //or BinaryStream.Dispose below will throw an exception.

            Dictionary<string, string> files = new Dictionary<string, string>(){
                {"imageHTML.html", "Files/ImageHTML.html"},
                {"blinkin_eyes.gif", "Images/blinkin_eyes.gif"}
            };

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                foreach (var f in files)
                {
                    if (false == isoStore.FileExists(f.Key))
                    {
                        StreamResourceInfo sr = Application.GetResourceStream(
                            new Uri("/MedicinesCatalogue;component/" + f.Value, UriKind.Relative));
                        if (sr == null)
                            sr = Application.GetResourceStream(new Uri(f.Value, UriKind.Relative));

                        using (BinaryReader br = new BinaryReader(sr.Stream))
                        {
                            byte[] data = br.ReadBytes((int)sr.Stream.Length);
                            SaveToIsoStore(f.Key, data);
                        }
                    }
                }
            }
        }

        private void SaveToIsoStore(string fileName, byte[] data)
        {
            string strBaseDir = string.Empty;
            string delimStr = "/";
            char[] delimiter = delimStr.ToCharArray();
            string[] dirsPath = fileName.Split(delimiter);

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //Re-create the directory structure.
                for (int i = 0; i < dirsPath.Length - 1; i++)
                {
                    strBaseDir = System.IO.Path.Combine(strBaseDir, dirsPath[i]);
                    isoStore.CreateDirectory(strBaseDir);
                }

                //Remove the existing file.
                if (isoStore.FileExists(fileName))
                {
                    isoStore.DeleteFile(fileName);
                }

                //Write the file.
                using (BinaryWriter bw = new BinaryWriter(isoStore.CreateFile(fileName)))
                {
                    bw.Write(data);
                    bw.Close();
                }
            }
        }
    }
}