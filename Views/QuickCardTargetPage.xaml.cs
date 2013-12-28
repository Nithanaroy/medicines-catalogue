using Microsoft.Phone.Controls;
using MedicinesCatalogue.ViewModels;
using System.Windows;

namespace MedicinesCatalogue.Views
{
    public partial class QuickCardTargetPage : PhoneApplicationPage
    {
        public QuickCardTargetPage()
        {
            InitializeComponent();

            // Create the ViewModel object.
            this.DataContext = new QuickCardTargetPageViewModel();

            // Create event handler for the page Loaded event.
            this.Loaded += new RoutedEventHandler(QuickCardTargetPage_Loaded);


        }

        // A property for the ViewModel.
        QuickCardTargetPageViewModel ViewModel
        {
            get { return (QuickCardTargetPageViewModel)DataContext; }
        }

        private void QuickCardTargetPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the quick card parameters from the deep link URI.
            ViewModel.LoadUriParameters(this.NavigationContext.QueryString);
        }
    }
}