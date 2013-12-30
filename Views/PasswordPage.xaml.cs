using System;
using Microsoft.Phone.Controls;
using System.Windows.Input;
using MedicinesCatalogue.Lib;
using System.Text.RegularExpressions;
using AppAgentAdapter.Data;

namespace MedicinesCatalogue.Views
{
    public partial class PasswordPage : PhoneApplicationPage
    {
        private string enteredPasscode = String.Empty;
        private string passwordChar = ApplicationHelper.passwordChar;

        public PasswordPage()
        {
            InitializeComponent();
        }

        #region PasswordTextBox
        private void PasswordTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            MessageTexBlock.Visibility = System.Windows.Visibility.Collapsed;

            enteredPasscode = GetNewPasscode(enteredPasscode, e.PlatformKeyCode);
            PasswordTextBox.Text = Regex.Replace(enteredPasscode, @".", passwordChar);

            if (PasswordTextBox.Text.Length == ApplicationHelper.passwordLength)
            {
                if (enteredPasscode == App.Settings.Password)
                {
                    SessionVariables.IsLoggedIn = true;
                    this.NavigationService.GoBack();
                }
                else
                {
                    MessageTexBlock.Visibility = System.Windows.Visibility.Visible;
                    PasswordTextBox.Text = String.Empty;
                    enteredPasscode = String.Empty;
                }
            }

            PasswordTextBox.SelectionStart = PasswordTextBox.Text.Length;
        }


        private string GetNewPasscode(string oldPasscode, int keyId)
        {
            string newPasscode = string.Empty;
            if (keyId == 8)
            {
                // backspace pressed
                if (oldPasscode.Length > 0)
                    newPasscode = oldPasscode.Substring(0, oldPasscode.Length - 1);
            }
            else if (keyId >= 48 && keyId <= 57) // Numbers pressed
                newPasscode = oldPasscode + (keyId - 48);
            else // Any other key incld. '.' is ignored
                newPasscode = oldPasscode;

            return newPasscode;
        }
        #endregion

        private void PhoneApplicationPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SetUpStage();
        }

        private void SetUpStage()
        {
            PasswordTextBox.Focus();
        }
    }
}