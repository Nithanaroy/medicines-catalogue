using System;
using System.Windows;
using Microsoft.Phone.Controls;
using MedicinesCatalogue.Lib;
using Microsoft.Phone.Tasks;

namespace MedicinesCatalogue
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            versionTextBlock.Text = "Version " + String.Format("{0:N1}", ApplicationHelper.currentApplicationVersion); 
            detailsTextBlock.Text = "\r\nTracking which medicine is for what and its expiry are some use cases. This also alerts when to take a medicine and how much!";
            currentReleaseTextBlock.Text = "New Features added:\r\n→ Group medicines\r\n→ Alerts and reminders\r\n→ UI Enhancements";
            nextReleaseTextBlock.Text = "Features in next version:\r\n→ Multiple images per medicine\r\n→ Camera enhancements\r\n→ Live tiles";
            copyrightsTextBlock.Text = "© 2013 Nitin Pasumarthy";
        }

        private void HomeMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.MainPage), UriKind.Relative));
        }

        private void RateAndReviewButton_Click(object sender, RoutedEventArgs e)
        {
            new MarketplaceReviewTask().Show();
        }

        private void FeedBackMenuButton_Click(object sender, EventArgs e)
        {
            ApplicationHelper.SendMail(ApplicationHelper.EmailFor.FeedBack);
        }
    }
}