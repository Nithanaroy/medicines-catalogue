using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MedicinesCatalogue.Lib;
using MedicinesCatalogue.ViewModels;
using Microsoft.Phone.Controls;
using AppAgentAdapter.ViewModels;

namespace MedicinesCatalogue.Views
{
    public partial class settings : PhoneApplicationPage
    {
        SettingsViewModel viewModel;
        private bool hasErrors = false;

        private string enteredPasscode = String.Empty;
        private string passwordChar = ApplicationHelper.passwordChar;

        public settings()
        {
            InitializeComponent();
            viewModel = new SettingsViewModel();
        }

        /// <summary>
        /// User should be authenticated to access this page
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.VerifyUserAuthenticity(this);
        }

        /// <summary>
        /// Initialized variables that are dependant on DataBinding
        /// </summary>
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            enteredPasscode = ActualPasscode.Text;
        }

        /// <summary>
        /// Saves the settings
        /// </summary>
        private void SaveMenuButton_Click(object sender, EventArgs e)
        {
            hasErrors = false;
            PasswordProtectionToggleSwitch.GetBindingExpression(ToggleSwitch.IsCheckedProperty).UpdateSource();
            if ((bool)PasswordProtectionToggleSwitch.IsChecked)
                ActualPasscode.GetBindingExpression(TextBlock.TextProperty).UpdateSource();
            if (!hasErrors)
            {
                viewModel.Save();
                MessageBox.Show("Settings saved!");
            }
            this.Focus();
        }

        /// <summary>
        /// Shows the errors in a message box if any
        /// </summary>
        private void ControlsPanel_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                hasErrors = true;
                MessageBox.Show(e.Error.ErrorContent.ToString());
            }
        }

        #region PasswordTextBox
        // Source:- 

        private void PasswordTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            //modify new passcode according to entered key
            enteredPasscode = GetNewPasscode(enteredPasscode, e.PlatformKeyCode);

            ActualPasscode.Text = enteredPasscode;

            //replace text by *
            PasswordTextBox.Text = Regex.Replace(enteredPasscode, @".", passwordChar);

            //take cursor to end of string
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

        #region Navigation
        /// <summary>
        /// Navigates to Home Page
        /// </summary>
        private void HomeMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.MainPage), UriKind.Relative));
        }

        /// <summary>
        /// Navigates to About Page
        /// </summary>
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.AboutPage), UriKind.Relative));
        }
        #endregion
    }
}