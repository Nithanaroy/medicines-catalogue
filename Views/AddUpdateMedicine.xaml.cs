using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using AppAgentAdapter.Data;
using MedicinesCatalogue.Lib;
using MedicinesCatalogue.ViewModels;

// For camera
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using System.IO.IsolatedStorage;
using System.Windows.Data;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using System.Windows.Shapes;


namespace MedicinesCatalogue.Views
{
    /// <summary>
    /// Add or update a medicine
    /// </summary>
    public partial class AddUpdateMedicine : PhoneApplicationPage
    {
        MedicineViewModel viewmodel;
        GroupViewModel groupViewModel;

        /// <summary>
        /// Used for changing the title of the page.
        /// This page is used for both updating and adding medicines
        /// </summary>
        private string addMedicineTitle = "add", editMedicineTitle = "update";

        /// <summary>
        /// Temporary image filename saved in IsloatedStorage
        /// </summary>
        string tempJPEG = "tempJPG.jpg";

        /// <summary>
        /// Indicates whether the user took a picture
        /// Currently used in: While adding a new medicine
        /// </summary>
        bool tookPicture = false;

        /// <summary>
        /// Flag which indicates there are any validation errors
        /// </summary>
        bool hasErrors = false;

        /// <summary>
        /// List of error strings
        /// </summary>
        List<string> errors = new List<string>();

        public AddUpdateMedicine()
        {
            InitializeComponent();
            BindGroups();
            SwitchToBasicView();
            ChangeInputScopeIfNotPresent();
        }

        /// <summary>
        /// Sets the Delete Image source to Light Theme image
        /// </summary>
        private void SetThemeSpecificImages()
        {
            var DeleteReminderButton = RemindersListBox.FindName("DeleteReminderButton") as Button;
            Visibility darkBackgroundVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];

            if (darkBackgroundVisibility == Visibility.Visible)
                DeleteReminderButton.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/delete.png", UriKind.Relative)) };
            else
                DeleteReminderButton.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/delete_light.png", UriKind.Relative)) };
        }

        /// <summary>
        /// Changes the input scope to Search if TextBox doesn't have any
        /// </summary>
        private void ChangeInputScopeIfNotPresent()
        {
            var allTextBoxes = new List<TextBox>();
            GetAllControlsOfType<TextBox>(MedicineStackPanel, allTextBoxes);
            var SearchInputScope = new System.Windows.Input.InputScope()
                    {
                        Names = { new InputScopeName() 
                        { 
                            NameValue = InputScopeNameValue.Search } 
                        }
                    };
            foreach (var textbox in allTextBoxes)
            {
                if (textbox.InputScope == null)
                    textbox.InputScope = SearchInputScope;
            }
        }

        /// <summary>
        /// Fills the GroupsListPicker with all the available groups
        /// Selected groups in case of updating medicine are bound in OnNavigatedTo method
        /// </summary>
        private void BindGroups()
        {
            groupViewModel = new GroupViewModel();
            Binding binding = new Binding()
            {
                Source = groupViewModel,
                Path = new PropertyPath("AllGroups")
            };
            GroupsListPicker.SetBinding(ListPicker.ItemsSourceProperty, binding);
        }

        /// <summary>
        /// 1. Authenticates the user
        /// 2. Sets the viewmodel
        /// 3. Bind the GroupsListPicker selected items property
        /// 4. Sets the image on page with correct source
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            App.VerifyUserAuthenticity(this);

            if (viewmodel == null)
            {
                base.OnNavigatedTo(e);
                string _medicineId = "";
                if (NavigationContext.QueryString.TryGetValue("medicineId", out _medicineId))
                {
                    int medicineId = int.Parse(_medicineId);
                    viewmodel = new MedicineViewModel(medicineId);
                    PageTitle.Text = editMedicineTitle;
                }
                else
                {
                    viewmodel = new MedicineViewModel();
                    PageTitle.Text = addMedicineTitle;
                }

                DataContext = viewmodel.Medicine;
            }

            RemindersListBox.ItemsSource = viewmodel.Medicine.Reminders;
            SetNoRemindersHelperText();
        }

        /// <summary>
        /// Sets the helper text's visibility which shows that no reminders are found whenever requried
        /// </summary>
        private void SetNoRemindersHelperText()
        {
            if (viewmodel.Medicine.Reminders.Count > 0)
                NoAlarmsFoundText.Visibility = Visibility.Collapsed;
            else
                NoAlarmsFoundText.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Any validation errors of the form are caught here
        /// 1) Populates the errors list thrown by Model
        /// 2) Colorize control's background to light yellow on error else white
        /// </summary>
        private void MedicineCanvas_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            Control c = e.OriginalSource as Control; // Can be a TextBox or DatePicker
            if (e.Action == ValidationErrorEventAction.Added)
            {
                hasErrors = true;
                errors.Add(e.Error.ErrorContent.ToString());
                c.Background = new SolidColorBrush(new Color() { A = 255, R = 250, G = 255, B = 189 });
            }
            else
                c.ClearValue(Control.BackgroundProperty);
        }

        /// <summary>
        /// Updates existing medicine or adds a new medicine. If viewmodel's Medicine's ID is 0, its a new medicine
        /// 1) Updates the corresponding column in DB as UpdateSourceTrigger is set to explicit in UI for each field
        /// 2) Shows the errors in a MessageBox if any
        /// 3) Adds the new medicine / updates existing
        /// 4) In case of new medicine, picture is saved after it has been inserted into DB. 
        ///    This is done so as to generate a unique file name for image to be saved in IsloatedStorage.
        ///    The name is generated from MedicineID
        /// 5) Update IsolatedStorage, i.e. remove image if deleted
        /// 6) Navigates back to Details page
        /// </summary>
        private void SaveMenuButton_Click(object sender, EventArgs e)
        {
            hasErrors = false;
            errors.Clear();

            UpdateBindingSource<TextBox>(TextBox.TextProperty);
            UpdateBindingSource<CheckBox>(CheckBox.IsCheckedProperty);
            UpdateBindingSource<DatePicker>(DatePicker.ValueProperty);

            // Update Groups only if medicine is already existing. Otherwise mapping fails.
            // So for new medicine, we first create the medicine and then update its groups mappings
            // TODO: Better way to hadle many to many
            if (viewmodel.Medicine.Id > 0)
                GroupsListPicker.GetBindingExpression(ListPicker.SelectedItemsProperty).UpdateSource();

            if (hasErrors)
                MessageBox.Show(ApplicationHelper.GetStringFromListItems(errors), "Correct these errors", MessageBoxButton.OK);
            else
            {
                if (viewmodel.Medicine.Id == 0)
                {
                    viewmodel.Create();
                    // Now map the groups
                    GroupsListPicker.GetBindingExpression(ListPicker.SelectedItemsProperty).UpdateSource();
                    viewmodel.Save();

                    App.ViewModel.Medicines.Insert(0, viewmodel.Medicine);
                    if (tookPicture)
                    {
                        string pictureFilename = ApplicationHelper.GetImageFilenameInIsloatedStorage(viewmodel.Medicine.Id);
                        ApplicationHelper.RenameImageInIsolatedStorage(tempJPEG, pictureFilename);
                        viewmodel.Medicine.PictureURI = pictureFilename;
                        viewmodel.Save(); // Update the picture file name
                    }
                }
                else
                {
                    if (tookPicture)
                    {
                        string pictureFilename = ApplicationHelper.GetImageFilenameInIsloatedStorage(viewmodel.Medicine.Id);
                        ApplicationHelper.RenameImageInIsolatedStorage(tempJPEG, pictureFilename);
                        viewmodel.Medicine.PictureURI = pictureFilename;
                    }
                    if (viewmodel.Medicine.PictureURI == null)
                        ApplicationHelper.DeleteImageInIsolatedStorage(ApplicationHelper.GetImageFilenameInIsloatedStorage(viewmodel.Medicine.Id));
                    viewmodel.Save();
                }

                // Now, update contents and register all the reminders of this medicine
                SaveAndAttachRemindersToMedicine();

                NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.DetailsPage) + viewmodel.Medicine.Id, UriKind.Relative));
            }
        }

        /// <summary>
        /// Unschedule all existing reminders from ReminderService
        /// Schedule all reminders using the in-memory list of Reminders
        /// This is done as in-memory Reminders list is only, updated according to user's actions
        /// </summary>
        private void SaveAndAttachRemindersToMedicine()
        {
            ReminderFactory factory = new ReminderFactory(viewmodel.Medicine);
            factory.UpdateIndependantReminderAttributes(viewmodel.Medicine.Id);
            factory.UnscheduleAllReminders();
            String[] reminderNames = factory.ScheduleAllReminders();
            if (reminderNames.Length == 0)
                return;
            viewmodel.Medicine.ReminderIds = String.Join(";", reminderNames);
            viewmodel.Save();
        }

        /// <summary>
        /// Code that actually updates the Model's properties
        /// This is required as UpdateSourceTrigger is set to Explicit in UI
        /// </summary>
        /// <typeparam name="T">TextBox or DatePicker</typeparam>
        /// <param name="dp">Text for a TextBox | Value for a DatePicker</param>
        private void UpdateBindingSource<T>(DependencyProperty dp)
            where T : FrameworkElement
        {
            List<T> allControls = new List<T>();
            GetAllControlsOfType<T>(MedicineStackPanel, allControls);
            foreach (var control in allControls)
            {
                control.GetBindingExpression(dp).UpdateSource();
            }
        }

        /// <summary>
        /// Gets all the controls no matter how deep they are in the Visual Tree.
        /// Recursively loops over all controls to get the required controls
        /// Used when manually updating the Source of a DataBound element
        /// </summary>
        /// <typeparam name="T">Type, generally Framework element</typeparam>
        /// <param name="source">A Panel only whose children are to be searched for recursively</param>
        /// <param name="result">As this is recursive, we pass the result List and add the Controls to it in every recursion</param>
        private void GetAllControlsOfType<T>(Panel source, List<T> result)
        {
            result.AddRange(source.Children.OfType<T>());
            foreach (Panel panel in source.Children.OfType<Panel>())
                GetAllControlsOfType<T>(panel, result);
        }

        /// <summary>
        /// 1) Sets the PictureURI field of viewmodel's Medicine to null, indicating no picture
        /// 2) Clicking the save button updates the medicine
        /// 3) The actual image from IsolatedStorage is removed on Save button press
        /// 4) Deletes the temporary image from IsolatedStorage
        /// </summary>
        private void DeleteImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBoxResult actionToTake = MessageBox.Show("You want to remove this picture?", "Are you sure", MessageBoxButton.OKCancel);
            if (actionToTake == MessageBoxResult.OK)
            {
                viewmodel.Medicine.PictureURI = null;
                ApplicationHelper.DeleteImageInIsolatedStorage(tempJPEG);
            }
        }

        /// <summary>
        /// Triggers the camera task and lets user take a picture
        /// </summary>
        private void CameraButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Handler raised when camera capture task is completed
        /// 1) Sets the image placeholder with taken picture
        /// 2) Saves the image to IsloatedStorage
        /// </summary>
        private void CaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                tookPicture = true;

                BitmapImage takenImage = new BitmapImage();
                takenImage.SetSource(e.ChosenPhoto);
                MedicineImage.Source = takenImage;

                e.ChosenPhoto.Seek(0, System.IO.SeekOrigin.Begin);
                WriteableBitmap wb = PictureDecoder.DecodeJpeg(e.ChosenPhoto);

                ApplicationHelper.SaveImageInIsolatedStorage(tempJPEG, wb);
            }
        }

        #region Navigation
        /// <summary>
        /// Cancels the add or update operation
        /// Navigates to Main Page if add operation or Details Page if update operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelMenuButton_Click(object sender, EventArgs e)
        {
            int medicineId = viewmodel.Medicine.Id;
            if (medicineId == 0)
                NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.MainPage), UriKind.Relative));
            else
                NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.DetailsPage) + medicineId, UriKind.Relative));
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
        #endregion

        /// <summary>
        /// Adding new group
        /// </summary>
        private void AddNewGroupButton_Click(object sender, RoutedEventArgs e)
        {
            groupViewModel.Group = new Group();
            App.CreateNewGroupDialog(groupViewModel);
        }

        /// <summary>
        /// Navigates to add reminder page
        /// </summary>
        private void NewReminderButton_Click(object sender, RoutedEventArgs e)
        {
            ReminderCanvas.Visibility = Visibility.Visible;
            ApplicationBar.Buttons.Clear();
            ApplicationBar.Buttons.Add(
                new ApplicationBarIconButton
                {
                    Text = "save",
                    IconUri = new Uri("/Images/save.png", UriKind.Relative)
                });
            ApplicationBar.Buttons.Add(
                new ApplicationBarIconButton
                {
                    Text = "cancel",
                    IconUri = new Uri("/Images/cancel.png", UriKind.Relative)
                });
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Click += SaveReminderMenuButton_Click;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Click += CancelReminderAddMenuButton_Click;
        }

        /// <summary>
        /// Creates a new Reminder instance with all details except navigtionURI, Content.
        /// These are filled when user actually saves the medicine
        /// </summary>
        private void SaveReminderMenuButton_Click(object sender, EventArgs e)
        {
            DateTime date = (DateTime)FromDatePicker.Value;
            DateTime time = (DateTime)TimePicker.Value;
            DateTime beginTime = date + time.TimeOfDay;

            if (beginTime < DateTime.Now)
            {
                MessageBox.Show("From Date or Time must be in the future", "OOPS!", MessageBoxButton.OK);
                return;
            }

            date = (DateTime)ToDatePicker.Value;
            time = (DateTime)TimePicker.Value;
            DateTime expirationTime = date + time.TimeOfDay;

            if (expirationTime < beginTime)
            {
                MessageBox.Show("To Date must be after the From Date", "OOPS!", MessageBoxButton.OK);
                return;
            }

            RecurrenceInterval recurrence = RecurrenceInterval.None;
            if (dailyRadioButton.IsChecked == true)
                recurrence = RecurrenceInterval.Daily;
            else if (weeklyRadioButton.IsChecked == true)
                recurrence = RecurrenceInterval.Weekly;
            else if (monthlyRadioButton.IsChecked == true)
                recurrence = RecurrenceInterval.Monthly;
            else if (yearlyRadioButton.IsChecked == true)
                recurrence = RecurrenceInterval.Yearly;

            new ReminderFactory(viewmodel.Medicine).AddReminderToList(beginTime, expirationTime, recurrence);

            ResetStageForAddUpdateMedicine();
        }

        /// <summary>
        /// Cancels the reminder add and closes the Canvas where all reminder controls are present
        /// </summary>
        void CancelReminderAddMenuButton_Click(object sender, EventArgs e)
        {
            ResetStageForAddUpdateMedicine();
        }

        /// <summary>
        /// Sets the application bar buttons to initial state from Reminder buttons
        /// </summary>
        private void ResetStageForAddUpdateMedicine()
        {
            ReminderCanvas.Visibility = Visibility.Collapsed;
            ApplicationBar.Buttons.Clear();
            ApplicationBar.Buttons.Add(
                new ApplicationBarIconButton
                {
                    Text = "photo",
                    IconUri = new Uri("/Images/camera.png", UriKind.Relative)
                });
            ApplicationBar.Buttons.Add(
                new ApplicationBarIconButton
                {
                    Text = "save",
                    IconUri = new Uri("/Images/save.png", UriKind.Relative)
                });
            ApplicationBar.Buttons.Add(
                new ApplicationBarIconButton
                {
                    Text = "cancel",
                    IconUri = new Uri("/Images/cancel.png", UriKind.Relative)
                });
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Click += CameraMenuButton_Click;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Click += SaveMenuButton_Click;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Click += CancelMenuButton_Click;
            SetNoRemindersHelperText();
        }

        /// <summary>
        /// Removes the reminder from Reminders list of this medicine with confirmation
        /// </summary>
        private void DeleteReminderButton_Click(object sender, EventArgs e)
        {
            var factory = new ReminderFactory(viewmodel.Medicine);
            var reminderName = (string)((Button)sender).Tag;
            if (MessageBox.Show(String.Format("Delete reminder,\r\n{0}", factory.GetReminderString(reminderName)),
                            "Are you sure?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                factory.RemoveReminder(reminderName);
            SetNoRemindersHelperText();
        }

        #region ListPicker_TextBox_Sync
        /// <summary>
        /// Generic method for mainting sync between a listpicker and a textbox
        /// Handles, the ListPicker value change event and accordingly modifies TextBox
        /// Used for Dosage and Quantity fields
        /// </summary>
        /// <param name="picker">Control 1, ListPicker</param>
        /// <param name="textbox">Control 2, TextBox</param>
        /// <param name="defaultPickerSelectedIndex">Index to select when TextBox value is not empty</param>
        private void SyncListPicker_TextBoxTextChanged(ListPicker picker, TextBox textbox, int defaultPickerSelectedIndex = 1)
        {
            if (textbox.Text.Trim().Length == 0)
                picker.SelectedIndex = 0;
            else if (picker.SelectedIndex == 0)
                picker.SelectedIndex = defaultPickerSelectedIndex;
        }

        /// <summary>
        /// Generic method for mainting sync between a listpicker and a textbox
        /// Handles, the TextBox value change event and accordingly modifies ListPicker
        /// Used for Dosage and Quantity fields
        /// </summary>
        /// <param name="textbox">Control 1, TextBox</param>
        /// <param name="picker">Control 2, ListPicker</param>
        /// <param name="defaultTextBoxText">Default TextBox value when ListPicker selected value is non empty</param>
        private void SyncTextBox_ListPickerSelectionChanged(TextBox textbox, ListPicker picker, String defaultTextBoxText = "")
        {
            if (picker.SelectedIndex == 0)
                textbox.Text = String.Empty;
            else if (textbox.Text.Trim().Length == 0)
                textbox.Text = defaultTextBoxText;
        }

        /// <summary>
        /// Sees that quantity and quantity type textboxes are in sync
        /// </summary>
        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SyncListPicker_TextBoxTextChanged(QuantityTypesListPicker, QuantityTextBox);
        }

        /// <summary>
        /// Sees that quantity and quantity type textboxes are in sync
        /// </summary>
        private void QuantityTypesListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string defaultQuantity = "0";
            if (viewmodel.Medicine.Quantity != null)
                defaultQuantity = viewmodel.Medicine.Quantity.ToString();
            SyncTextBox_ListPickerSelectionChanged(QuantityTextBox, QuantityTypesListPicker, defaultQuantity);
        }

        private void DosageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SyncListPicker_TextBoxTextChanged(DosageTypesListPicker, DosageTextBox);
        }

        private void DosageTypesListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string defaultDosage = "0";
            if (viewmodel.Medicine.Dosage != null)
                defaultDosage = viewmodel.Medicine.Dosage;
            SyncTextBox_ListPickerSelectionChanged(DosageTextBox, DosageTypesListPicker, defaultDosage);
        }
        #endregion

        /// <summary>
        /// Starts the camera when user taps the Image
        /// </summary>
        private void MedicineImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StartCamera();
        }

        /// <summary>
        /// Starts the camera when user clicks the Camera Menu button
        /// </summary>
        private void CameraMenuButton_Click(object sender, EventArgs e)
        {
            StartCamera();
        }

        /// <summary>
        /// Starts camera Task and attachs the completed event
        /// </summary>
        private void StartCamera()
        {
            CameraCaptureTask captureTask = new CameraCaptureTask();
            try
            {
                captureTask.Show();
            }
            catch (Exception)
            {
                // Ignore if user continuously presses the camera button
            }
            captureTask.Completed += CaptureTask_Completed;
        }

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
            GroupsListPicker.Visibility = Visibility.Collapsed;

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
            GroupsListPicker.Visibility = Visibility.Visible;

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

        /// <summary>
        /// Hides the keyboard when enter key is pressed
        /// </summary>
        private void PhoneApplicationPage_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                this.Focus();
        }
    }
}