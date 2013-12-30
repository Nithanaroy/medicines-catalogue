using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AppAgentAdapter.Data;
using MedicinesCatalogue.Lib;
using System.Collections.Generic;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Data.Linq;
using MedicinesCatalogue.ViewModels;
using System.Runtime.InteropServices;
using Microsoft.Phone.Scheduler;

namespace MedicinesCatalogue
{
    public partial class App : Application
    {
        private static MainViewModel viewModel = null;
        public static Settings Settings = AppAgentAdapter.App.Settings;

        public static string DbConnection { get; set; }
        /// <summary>
        /// Used by UpdateDB() running migrations and updating the DDL if database is not in the latest version
        /// Increments in DB version will be by 1 always
        /// </summary>
        private int currentDBVersion = 1;

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                    viewModel = new MainViewModel();

                return viewModel;
            }
        }

        ///// <summary>
        ///// Model that has all the settings of application
        ///// </summary>
        //public static Settings Settings
        //{
        //    get
        //    {
        //        if (settings == null)
        //            settings = new Settings();
        //        return settings;
        //    }
        //    set
        //    {
        //        settings = value;
        //    }
        //}

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            DbConnection = Resources["IsoDbConnection"] as string;
            CreateDB();
            UpdateDB();
            GetSettingsFromIsolatedStorage();
            StartAgent();
        }

        private void StartAgent()
        {
            StopAgentIfStarted();

            PeriodicTask task = new PeriodicTask(ApplicationHelper.myBackgroundAgent);
            task.Description = "Medicines Catalogue, Custom Agent for Live Tiles & Lock Screen Notifications";
            ScheduledActionService.Add(task);
#if DEBUG
            // If we're debugging, attempt to start the task immediately
            ScheduledActionService.LaunchForTest(ApplicationHelper.myBackgroundAgent, new TimeSpan(0, 0, 1));
#endif
        }

        private void StopAgentIfStarted()
        {
            if (ScheduledActionService.Find(ApplicationHelper.myBackgroundAgent) != null)
            {
                ScheduledActionService.Remove(ApplicationHelper.myBackgroundAgent);
            }
        }

        /// <summary>
        /// :: Important ::
        /// Aim: Update DB to latest version using currentDBVersion variable.
        /// 
        /// Working: 
        /// Just like migrations in Rails.
        /// Each case in the switch has changes in that specific DB version
        /// default case implies latest version and so no changes
        /// At the end of each case we add goto to next case to fall through until we reach the latest version
        /// 
        /// Notes:
        /// Target version will always be 'one' version higher than the current one. Increments in DB versions is always by 1
        /// All the changes to schema from the targetVersion will be run
        /// </summary>
        private void UpdateDB()
        {
            using (var db = new MedicineContext(App.DbConnection))
            {
                var adapter = db.CreateDatabaseSchemaUpdater();
                switch (GetNextDBVersion(adapter.DatabaseSchemaVersion))
                {
                    case 0:
                        // Version 0. Initial DB
                        goto case 1;
                    case 1:
                        // Version 1 changes
                        // New Columns
                        adapter.AddColumn<Medicine>("IsUsing");
                        adapter.AddColumn<Medicine>("Quantity");
                        adapter.AddColumn<Medicine>("QuantityType");
                        adapter.AddColumn<Medicine>("ReminderIds");
                        adapter.AddColumn<Medicine>("DosageType");
                        // New Tables
                        adapter.AddTable<Group>();
                        adapter.AddTable<Medicine_Group>();
                        goto default;
                    default:
                        // DB upto date
                        break;
                }
                adapter.DatabaseSchemaVersion = currentDBVersion;
                adapter.Execute();
            }
        }

        /// <summary>
        /// Has the logic which returns the next version for upgrade, given the current version
        /// </summary>
        /// <param name="presentVersion">Present version number of the DB</param>
        /// <returns>Next version to start the upgrade / migrations from</returns>
        private int GetNextDBVersion(int presentVersion)
        {
            return presentVersion + 1;
        }

        /// <summary>
        /// Creates the Medicines Catalogue DB if it doesn't exist
        /// Adds a sample medicine
        /// </summary>
        private void CreateDB()
        {
            using (var db = new MedicineContext(DbConnection))
            {
                if (!db.DatabaseExists())
                {
                    db.CreateDatabase();
                    InsertSampleData(db);
                    db.SubmitChanges();
                    var adapter = db.CreateDatabaseSchemaUpdater();
                    adapter.DatabaseSchemaVersion = currentDBVersion;
                    adapter.Execute();
                }
            }
        }

        /// <summary>
        /// Inserts sample data into the database
        /// </summary>
        /// <param name="db">Database context to use</param>
        private void InsertSampleData(MedicineContext db)
        {
            // Sample Medicines
            Medicine m1 = new Medicine()
            {
                Name = "saridon (sample)",
                MfgDate = DateTime.Now,
                ExpDate = DateTime.Now.AddMonths(3),
                CureFor = "Headache",
                Dosage = "4",
                DosageType = "cups",
                AgeGroup = "18+",
                Price = 20.50F,
                BoughtAddress = "Near Canara bank, Kormangala, Bangalore. Ph: 080-23399239",
                Quantity = 2,
                QuantityType = "pills",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            db.Medicines.InsertOnSubmit(m1);

            Medicine m2 = new Medicine()
            {
                Name = "nice (sample)",
                MfgDate = DateTime.Now,
                ExpDate = DateTime.Now.AddMonths(3),
                IsUsing = true,
                CureFor = "Fever",
                Dosage = "3",
                DosageType = "spoons",
                AgeGroup = "20+",
                Price = 25.50F,
                BoughtAddress = "Indiranagar, Bangalore. Ph: 080-23399239",
                Quantity = 4,
                QuantityType = "pills",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            db.Medicines.InsertOnSubmit(m2);

            db.SubmitChanges();

            // Sample Groups
            Group g1 = new Group()
            {
                Name = "Is Using (sample)",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            db.Groups.InsertOnSubmit(g1);

            Group g2 = new Group()
            {
                Name = "Not Using (sample)",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            db.Groups.InsertOnSubmit(g2);

            db.SubmitChanges();

            // Sample Associations
            db.MedicinesGroups.InsertOnSubmit(new Medicine_Group()
            {
                Medicine = m1,
                Group = g1
            });

            db.MedicinesGroups.InsertOnSubmit(new Medicine_Group()
            {
                Medicine = m2,
                Group = g2
            });

            db.SubmitChanges();
        }

        /// <summary>
        /// Get application settings from IsolatedStorage and populate the static "settings" variable
        /// </summary>
        private void GetSettingsFromIsolatedStorage()
        {
            Settings.Password = AppAgentAdapter.Lib.ApplicationHelper.GetValueForSetting(AppAgentAdapter.Lib.ApplicationHelper.AvailableSettings.Password).ToString();
            Settings.HasPassword = (bool)AppAgentAdapter.Lib.ApplicationHelper.GetValueForSetting(AppAgentAdapter.Lib.ApplicationHelper.AvailableSettings.HasPassword);
        }

        #region PasswordVerification
        public static void VerifyUserAuthenticity(PhoneApplicationPage requestedPage)
        {
            if (SessionVariables.IsLoggedIn)
                return;
            var input = new PasswordInputPrompt
            {
                Title = "Password Required!",
                Message = "\r\nThe page you are trying to access requires a password",
                MessageTextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                IsAppBarVisible = true,
                IsSubmitOnEnterKey = true,
                HorizontalAlignment = HorizontalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center

            };
            input.Completed += delegate(object sender, PopUpEventArgs<string, PopUpResult> e)
            {
                if (e.PopUpResult != PopUpResult.Ok)
                {
                    // TODO: A better way to handle Back button close of pop up
                    if (requestedPage.NavigationService.CanGoBack)
                        requestedPage.NavigationService.GoBack();
                    else
                        // requestedPage.NavigationService.Navigate(new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.MainPage), UriKind.Relative));
                        throw new UnauthorizedAccessException("User cannot access this page. Quitting the app as exit was triggered");
                    return;
                }
                if (e.Result.Equals(Settings.Password))
                    SessionVariables.IsLoggedIn = true;
                else
                {
                    SessionVariables.IsLoggedIn = false;
                    MessageBox.Show("Incorrect Password");
                    VerifyUserAuthenticity(requestedPage);
                }
            };
            input.InputScope = new InputScope { Names = { new InputScopeName { NameValue = InputScopeNameValue.NumericPassword } } };
            input.Show();
        }
        #endregion

        /// <summary>
        /// Shows a dialog to create a new group
        /// </summary>
        /// <returns>Name of the new group if created else an empty string</returns>
        public static void CreateNewGroupDialog(GroupViewModel groupViewModel = null, bool showInfoMsg = false)
        {
            string groupName = String.Empty;
            var input = new InputPrompt
            {
                Title = "Add Group",
                Message = "\r\nChoose a name for the new group",
                MessageTextWrapping = TextWrapping.Wrap,
                IsAppBarVisible = true,
                IsCancelVisible = true,
                IsSubmitOnEnterKey = true,
                IsOverlayApplied = true
            };
            input.Completed += delegate(object sender, PopUpEventArgs<string, PopUpResult> e)
            {
                if (e.PopUpResult == PopUpResult.Ok)
                {
                    groupName = e.Result;
                    if (groupViewModel == null)
                        groupViewModel = new GroupViewModel();
                    groupViewModel.Group.Name = groupName;
                    if (groupViewModel.Create())
                    {
                        groupViewModel.AllGroups.Add(groupViewModel.Group);
                        if (showInfoMsg)
                            MessageBox.Show("Group is created but won't be shown here!\r\nAdd some medicines to the group to see it here.");
                    }
                }
            };
            input.Show();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            #region Attempt to set Background for all pages
            //ImageBrush brush = new ImageBrush
            //{
            //    ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Images/vintageBeach.png", UriKind.Relative)),
            //    Stretch = Stretch.UniformToFill
            //};
            //this.RootFrame.Background = brush; 
            #endregion
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            // Ensure that application state is restored appropriately
            App.ViewModel.LoadAllMedicines();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
            SessionVariables.IsLoggedIn = false;
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
            else
            {
                // Ignore UnauthorizedAccessException
                if (Marshal.GetHRForException(e.ExceptionObject) == -2147024891)
                    return;

                string message = String.Format("\"Your message if any\"\r\n\r\n::Do not modify from here::\r\n\r\nMessage from exception object:\r\n{0}\r\n\r\n\r\nStacktrace:\r\n{1}",
                                            e.ExceptionObject.Message, e.ExceptionObject.StackTrace);
                ApplicationHelper.SendMail(ApplicationHelper.EmailFor.Error, message);
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Assign the quick card URI mapper class to the application frame.
            RootFrame.UriMapper = new QuickCardUriMapper();
            
            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        /// <summary>
        /// Gets the app wide must use Date format
        /// </summary>
        /// <param name="dateTime">DateTime instance to convert</param>
        /// <returns>string of formatted date Eg: 14-Nov-2013</returns>
        internal static string GetFormattedDate(DateTime dateTime)
        {
            return dateTime.ToString("dd-MMM-y");
        }

        /// <summary>
        /// Gets the app wide formatted time
        /// </summary>
        /// <param name="dateTime">DateTime instance to convert</param>
        /// <returns>string of formatted time. Eg: 09:00 AM</returns>
        internal static object GetFormattedTime(DateTime dateTime)
        {
            return dateTime.ToString("h:mm tt");
        }
    }
}