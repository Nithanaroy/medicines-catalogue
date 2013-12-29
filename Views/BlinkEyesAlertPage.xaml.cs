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

            BlinkEyesBrowser.Loaded += new RoutedEventHandler(BlinkEyesBrowser_Loaded);
        }

        void BlinkEyesBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            SaveFilesToIsoStore();
            //SaveHTMLToIsolatedStore();

            //Save3();   
            //BlinkEyesBrowser.Navigate(new Uri("ImageHTML.html", UriKind.Relative));
            //BlinkEyesBrowser.Navigate(new Uri("http://www.yahoo.com", UriKind.Absolute));
            BlinkEyesBrowser.NavigateToString("<html><head><meta name='viewport' content='width=480, user-scalable=yes' /></head><body>HTML Text</body></html>");
        }

        private void Save3()
        {
            StreamResourceInfo strm = Application.GetResourceStream(new Uri("/MedicinesCatalogue;component/Files/ImageHTML.html", UriKind.Relative));
            StreamReader reader = new StreamReader(strm.Stream);
            string data = reader.ReadToEnd();
        }

        private void SaveHTMLToIsolatedStore()
        {
            byte[] myByteArray = null;
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            using (var stream = new IsolatedStorageFileStream("filename.txt",
                 FileMode.Create, FileAccess.Write, store))
            {
                stream.Write(myByteArray, 0, myByteArray.Length);
            }
        }

        private void SaveFilesToIsoStore()
        {
            //These files must match what is included in the application package,
            //or BinaryStream.Dispose below will throw an exception.

            Dictionary<string, string> files = new Dictionary<string, string>(){
                {"imageHTML.html", "../Files/ImageHTML.html"}
            };
            string f = "ImageHTML.html", f1 = "../Files/ImageHTML.html";

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (false == isoStore.FileExists(f))
                {
                    //foreach (string f in files)
                    //{

                    StreamResourceInfo sr = Application.GetResourceStream(new Uri("/MedicinesCatalogue;component/Files/ImageHTML.html", UriKind.Relative));
                    using (BinaryReader br = new BinaryReader(sr.Stream))
                    {
                        byte[] data = br.ReadBytes((int)sr.Stream.Length);
                        SaveToIsoStore(f, data);
                    }
                    //}
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