using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using MedicinesCatalogue.Lib;
using MedicinesCatalogue.ViewModels;

namespace MedicinesCatalogue
{
    /// <summary>
    /// Shows the details of a medicine
    /// </summary>
    public partial class DetailsPage : PhoneApplicationPage
    {
        MedicineViewModel viewmodel;

        public DetailsPage()
        {
            InitializeComponent();
            SwitchToBasicView();
        }

        /// <summary>
        /// 1) Authenticates the user
        /// 2) Sets the viewmodel. Navigates to MainPage if medicine not found
        /// 3) Sets the image of the medicine if any
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.VerifyUserAuthenticity(this);
            base.OnNavigatedTo(e);

            string medicineId = "";
            if (NavigationContext.QueryString.TryGetValue("medicineId", out medicineId))
            {
                int id = int.Parse(medicineId);
                viewmodel = new MedicineViewModel(id);
            }
            else
            {
                NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.MainPage), UriKind.Relative));
                return;
            }

            // Check if medicine is found
            if (viewmodel.Medicine == null)
            {
                NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.MainPage), UriKind.Relative));
                return;
            }
            else
                DataContext = viewmodel.Medicine;

            AlarmsListBox.ItemsSource = viewmodel.Medicine.Reminders;
            if (viewmodel.Medicine.Reminders.Count == 0)
                AlarmsEmpty.Visibility = Visibility.Visible;
            else
                AlarmsEmpty.Visibility = Visibility.Collapsed;

            App.TriggerBlinkingEyes(this);
        }

        /// <summary>
        /// Deletes the medicine with confirmation
        /// Navigates to Home page after successful delete
        /// </summary>
        private void DeleteMenuButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult actionToTake = MessageBox.Show("Are you sure?", "Delete " + ListTitle.Text, MessageBoxButton.OKCancel);
            if (actionToTake == MessageBoxResult.OK)
            {
                // Delete image of medicine if any
                ApplicationHelper.DeleteImageInIsolatedStorage(ApplicationHelper.GetImageFilenameInIsloatedStorage(viewmodel.Medicine.Id));
                viewmodel.Delete();
                NavigationService.Navigate(new Uri(String.Format("{0}?flashMessage={1}, deleted successfully",
                    ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.MainPage), ListTitle.Text), UriKind.Relative));

                // Delete all reminders attached to this medicine
                new ReminderFactory(viewmodel.Medicine).UnscheduleAllReminders();
            }
        }

        #region ImagePreview
        /// <summary>
        /// Shows the hidden Image Preview Grid to show the image full size
        /// </summary>
        private void MedicineImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (viewmodel.Medicine.PictureURI != null)
                ImagePicker.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Hides the Image Preview grid
        /// </summary>
        private void CloseImagePreview_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ImagePicker.Visibility = System.Windows.Visibility.Collapsed;
        }
        #endregion

        #region Navigation
        /// <summary>
        /// Navigates to AddUpdatePage for updating the medicine
        /// </summary>
        private void EditMenuButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.UpdatePage) + viewmodel.Medicine.Id, UriKind.Relative));
            return;
        }

        /// <summary>
        /// Navigates to home page
        /// </summary>
        private void HomeMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.MainPage), UriKind.Relative));
        }

        /// <summary>
        /// Navigates to Settings page
        /// </summary>
        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.SettingsPage), UriKind.Relative));
        }

        /// <summary>
        /// Navigates to about page
        /// </summary>
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.AboutPage), UriKind.Relative));
        }

        /// <summary>
        /// Override hardware back key of phone press
        /// Forecibly navigates to Home Page
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.MainPage), UriKind.Relative));
        }
        #endregion

        #region Basic_Advanced_Toggle
        /// <summary>
        /// Hides all the advanced controls
        /// </summary>
        private void SwitchToBasicView()
        {
            IsUsing.Visibility = Visibility.Collapsed;

            MfgDateHeading.Visibility = Visibility.Collapsed;
            MfgDate.Visibility = Visibility.Collapsed;

            AgeGroupHeading.Visibility = Visibility.Collapsed;
            AgeGroup.Visibility = Visibility.Collapsed;

            GroupsHeading.Visibility = Visibility.Collapsed;
            Groups.Visibility = Visibility.Collapsed;

            PriceHeading.Visibility = Visibility.Collapsed;
            Price.Visibility = Visibility.Collapsed;

            QuantityHeading.Visibility = Visibility.Collapsed;
            Quantity.Visibility = Visibility.Collapsed;

            AddressHeading.Visibility = Visibility.Collapsed;
            Address.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Shows all controls
        /// </summary>
        private void SwitchToAdvancedView()
        {
            IsUsing.Visibility = Visibility.Visible;

            MfgDateHeading.Visibility = Visibility.Visible;
            MfgDate.Visibility = Visibility.Visible;

            AgeGroupHeading.Visibility = Visibility.Visible;
            AgeGroup.Visibility = Visibility.Visible;

            GroupsHeading.Visibility = Visibility.Visible;
            Groups.Visibility = Visibility.Visible;

            PriceHeading.Visibility = Visibility.Visible;
            Price.Visibility = Visibility.Visible;

            QuantityHeading.Visibility = Visibility.Visible;
            Quantity.Visibility = Visibility.Visible;

            AddressHeading.Visibility = Visibility.Visible;
            Address.Visibility = Visibility.Visible;
        }

        private void ViewSwitch_Checked(object sender, RoutedEventArgs e)
        {
            ViewTypeHeadingTextBlock.Text = "Advanced View";
            SwitchToAdvancedView();
        }

        private void ViewSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewTypeHeadingTextBlock.Text = "Basic View";
            SwitchToBasicView();
        }
        #endregion
    }
}