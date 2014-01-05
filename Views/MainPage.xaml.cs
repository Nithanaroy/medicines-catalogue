using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using AppAgentAdapter.Data;
using MedicinesCatalogue.Lib;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using MedicinesCatalogue.ViewModels;

namespace MedicinesCatalogue
{
    /// <summary>
    /// Home page where all medicines are listed. Also provides the search facility
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// 1) Sets the DataContext of PhonePage as an instance of MedicineViewModel.
        ///    The whole app uses the same instance
        /// 2) Loads the medicines which populates the ListView
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// The app is launched by search extensibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.NavigationContext.QueryString.Keys.Count > 0)
                App.ViewModel.LoadUriParameters(this.NavigationContext.QueryString);
        }

        /// <summary>
        /// Shows notifications in MessageBox if any
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            App.VerifyUserAuthenticity(this);
            base.OnNavigatedTo(e);
            SetUpStage();
        }

        /// <summary>
        /// Sets the DataContext
        /// Loads all the medicines if the app is opened normally.
        /// Doesnt load all medicines if app laucnhed by Search Extensibility
        /// In that case the required medicines are loaded in Page Loaded event above
        /// </summary>
        private void SetUpStage()
        {
            ShowMessageIfAny();
            DataContext = App.ViewModel;

            // For some reason VisualStateGroups require specific DataContext to be set.
            // Not working with Page DataCotext
            MainListBox.DataContext = App.ViewModel;

            if (this.NavigationContext.QueryString.Keys.Count == 0)
                App.ViewModel.LoadAllMedicines();
            else
                ((PivotItem)RootPivot.Items[0]).Header = "search res";

            MedicineGroupsList.DataContext = App.ViewModel;

            App.TriggerBlinkingEyes(this);
        }

        /// <summary>
        /// Extracts the flashMessage of Query string and displays in a MessageBox if any
        /// </summary>
        private void ShowMessageIfAny()
        {
            string message;
            if (NavigationContext.QueryString.TryGetValue("flashMessage", out message))
                MessageBox.Show(message);
        }

        /// <summary>
        /// 1) Gets the medicine ID of the selected list item
        /// 2) Navigates to its details page
        /// </summary>
        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainListBox.SelectedIndex == -1)
                return;
            int medicineId = ((Medicine)MainListBox.SelectedItem).Id;
            NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.DetailsPage) + medicineId, UriKind.Relative));

            MainListBox.SelectedIndex = -1;
        }

        /// <summary>
        /// 1) Gets the medicine ID of the selected list item
        /// 2) Navigates to its details page
        /// </summary>
        private void MedicineGroupsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MedicineGroupsList.SelectedItem == null)
                return;
            int medicineId = ((Medicine)MedicineGroupsList.SelectedItem).Id;
            NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.DetailsPage) + medicineId, UriKind.Relative));
            MedicineGroupsList.SelectedItem = null;
        }

        /// <summary>
        /// Hides all list items which don't contain the text entered
        /// A list item's ToString() is Medicine's ToString() which is a merge of all details of a medicine
        /// </summary>
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchString = SearchTextBox.Text.Trim().ToLower();
            foreach (Medicine medicine in MainListBox.Items)
            {
                ListBoxItem item = (ListBoxItem)MainListBox.ItemContainerGenerator.ContainerFromItem(medicine);
                if (item == null)
                    continue;
                if (medicine.ToString().Contains(searchString))
                    item.Visibility = System.Windows.Visibility.Visible;
                else
                    item.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #region Navigtion
        /// <summary>
        /// Navigates to AddUpdate page
        /// </summary>
        private void AddMenuButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.AddPage), UriKind.Relative));
            return;
        }

        /// <summary>
        /// Navigates to settings page
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
        /// Exits the app
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                while (NavigationService.RemoveBackEntry() != null)
                {
                    NavigationService.RemoveBackEntry();
                }
            }
            base.OnBackKeyPress(e);
        }
        #endregion

        /// <summary>
        /// Loads groups data only if user comes to groups tab
        /// </summary>
        private void RootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTab = ((PivotItem)RootPivot.SelectedItem).Header.ToString();
            if (selectedTab == ApplicationHelper.Tabs[ApplicationHelper.TabsItems.Groups])
            {
                App.ViewModel.LoadAllMedicinesByGroups();
                SetApplicationBarForPivotItem(ApplicationHelper.TabsItems.Groups);
            }
            else if (selectedTab == ApplicationHelper.Tabs[ApplicationHelper.TabsItems.All])
                SetApplicationBarForPivotItem(ApplicationHelper.TabsItems.All);
        }

        /// <summary>
        /// If pivot item is all, Add button on application bar navigates to add new medicine page
        /// If pivot item is groups, Add button produces add new group dialog
        /// </summary>
        /// <param name="tab"></param>
        private void SetApplicationBarForPivotItem(ApplicationHelper.TabsItems tab)
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.Buttons.Add(
                new ApplicationBarIconButton
                {
                    Text = "add",
                    IconUri = new Uri("/Images/add.png", UriKind.Relative)
                });

            switch (tab)
            {
                case ApplicationHelper.TabsItems.All:
                    (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Click += AddMenuButton_Click;
                    break;
                case ApplicationHelper.TabsItems.Groups:
                    // Show add group Dialog
                    (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Click += delegate(object sender1, EventArgs e1)
                    {
                        App.CreateNewGroupDialog(null, true);
                    };
                    break;
            }
        }

        /// <summary>
        /// Deletes the group
        /// </summary>
        private void DeleteGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var groupName = ((Button)sender).Tag.ToString();
            var result = MessageBox.Show(String.Format("Delete, \"{0}\"", groupName), "Are you sure?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                GroupViewModel groupViewModel = new GroupViewModel(groupName);
                groupViewModel.Delete();
                App.ViewModel.LoadAllMedicinesByGroups(true);
                MessageBox.Show(String.Format("Deleted group, \"{0}\"", groupName));
            }
        }

        /// <summary>
        /// Shows the edit group dialog, where user can change the name
        /// </summary>
        private void EditGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var groupName = ((Button)sender).Tag.ToString();
            var input = new InputPrompt
            {
                Title = "Edit Group Name",
                Message = String.Format("\r\nChoose a new name for the group, \"{0}\"", groupName),
                MessageTextWrapping = TextWrapping.Wrap,
                IsAppBarVisible = true,
                IsCancelVisible = true,
                IsSubmitOnEnterKey = true,
                IsOverlayApplied = true
            };
            input.Completed += delegate(object sender1, PopUpEventArgs<string, PopUpResult> e1)
            {
                GroupViewModel groupViewModel;
                if (e1.PopUpResult == PopUpResult.Ok)
                {
                    groupViewModel = new GroupViewModel(groupName);
                    groupViewModel.Group.Name = e1.Result;
                    groupViewModel.Save();
                    App.ViewModel.LoadAllMedicinesByGroups(true);
                }
            };
            input.Show();
        }

        /// <summary>
        /// Hide the keyboard and show the search results
        /// </summary>
        private void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                this.Focus();
        }

        /// <summary>
        /// Toggles the display of Empty List Box text
        /// TODO: Still Groups LongListSelector not working
        /// </summary>
        private void RootPivot_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            if (MainListBox.Items.Count == 0)
                VisualStateManager.GoToState(MainListBox, "HasNoMedicines", true);
            else
                VisualStateManager.GoToState(MainListBox, "HasMedicines", true);

            if (MedicineGroupsList.GetItemsInView().Count == 0)
                VisualStateManager.GoToState(this, "HasNoGroups", true);
            else
                VisualStateManager.GoToState(this, "HasGroups", true);
        }
    }
}