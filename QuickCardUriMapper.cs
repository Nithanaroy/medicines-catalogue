using System;
using System.Windows.Navigation;
using System.Net;

namespace MedicinesCatalogue
{

    // Sample URI: /SearchExtras?ProductName=Xbox+360&Category=Bing_Products_Electronics,Bing_Products_Office_Products

    public class QuickCardUriMapper : UriMapperBase
    {
        // Navigation destination. 
        //private static string TargetPageName = "QuickCardTargetPage.xaml";
        private static string TargetPageName = "MainPage.xaml";
        private string tempUri;

        public override Uri MapUri(Uri uri)
        {
            tempUri = uri.ToString();

            // Parse URI when launched by from a quick card.
            if (tempUri.Contains("/SearchExtras"))
            {
                // Decode all characters in the URI.
                tempUri = HttpUtility.UrlDecode(tempUri);

                // Create a new URI for product cards.
                if (tempUri.Contains("Bing_Products"))
                {
                    return GetProductCardUri(tempUri);
                }
            }

            // Immediately return the URI when it isn’t related to Search extensibility.
            return uri;
        }

        // Return a parsed Product Card URI.
        private Uri GetProductCardUri(string uri)
        {
            // Extract parameter values from URI.
            string ProductNameValue = GetURIParameterValue("ProductName=", uri);
            //string CategoryValue = GetURIParameterValue("Category=", uri);

            // Create new URI.
            string NewURI = String.Format("/Views/{0}?MedicineName={1}",
                                    TargetPageName, ProductNameValue);

            return new Uri(NewURI, UriKind.Relative);
        }

        // This method extracts the string values that correspond to URI parameters from a quick card.
        private string GetURIParameterValue(string parameteridentifier, string uri)
        {
            string tempValue = "";

            // If the parameter exists in the string, extract the corresponding parameter value.
            if (uri.Contains(parameteridentifier))
            {
                string subUri;

                // Extract the characters that contain and follow the parameter identifier.
                subUri = uri.Substring(uri.LastIndexOf(parameteridentifier));

                // Remove the parameter identifier from the substring.
                subUri = subUri.Replace(parameteridentifier, "");

                // Obtain the position of the next parameter in the substring.
                int nextParameterPosition = FindNextParameter(subUri);


                if (nextParameterPosition < int.MaxValue)
                {
                    // Remove the characters that contain and follow the next parameter.
                    tempValue = subUri.Substring(0, nextParameterPosition);
                }
                else
                {
                    // No more parameters follow in the string. 
                    tempValue = subUri;
                }

                // Encode the parameter values to help prevent issues in the URI.
                tempValue = HttpUtility.UrlEncode(tempValue);
            }

            return tempValue;
        }

        // Returns the string position of the next URI parameter, if applicable.
        private int FindNextParameter(string subUri)
        {
            int lowestPosition = int.MaxValue;
            int tempPosition;

            tempPosition = subUri.IndexOf("&ProductName");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&Category");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&PlaceName");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("?PlaceName");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&PlaceLatitude");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&PlaceLongitude");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&PlaceAddress");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&MovieName");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            return lowestPosition;
        }
    }
}
