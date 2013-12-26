using System;
using System.Collections.Generic;
using MedicinesCatalogue.ViewModels;
using Microsoft.Phone.Scheduler;
using System.Linq;
using System.Collections.ObjectModel;
using MedicinesCatalogue.Data;

namespace MedicinesCatalogue.Lib
{
    /// <summary>
    /// Takes care of adding and removing remiders
    /// </summary>
    public class ReminderFactory : NotifyPropertyChangeDataError
    {
        private Medicine medicine;
        public ReminderFactory(Medicine _medicine)
        {
            medicine = _medicine;
        }

        /// <summary>
        /// Creates a new Reminder instance and adds to current medicine's Reminders list
        /// </summary>
        /// <param name="beginTime">Begin time for Reminder</param>
        /// <param name="endTime">End time for Reminder</param>
        /// <param name="interval">Frequency of Reminder</param>
        public void AddReminderToList(DateTime beginTime, DateTime endTime, RecurrenceInterval interval)
        {
            String name = System.Guid.NewGuid().ToString();

            Reminder reminder = new Reminder(name);
            reminder.Title = "Its time to take medicine";
            reminder.BeginTime = beginTime;
            reminder.ExpirationTime = endTime;
            reminder.RecurrenceType = interval;

            medicine.Reminders.Add(reminder);
            //NotifyPropertyChanged("Reminders");
        }

        /// <summary>
        /// Called to update NavigationURI and Content of all Reminders of this Medicine
        /// This is created separately as it requires medicineId for NavigationURI (new medicines first have to be saved to get their IDs)
        /// </summary>
        /// <param name="medicineId">MedicineID used for navigating to Details Page</param>
        public void UpdateIndependantReminderAttributes(int medicineId)
        {
            foreach (var reminder in medicine.Reminders)
            {
                reminder.NavigationUri = new Uri(ApplicationHelper.GetUrlFor(ApplicationHelper.URLs.DetailsPage) + medicineId, UriKind.Relative);
                reminder.Content = GetReminderContent(medicineId);
            }
        }

        /// <summary>
        /// Gets the content of the reminder which shows Dosage (if present and valid), Dosage Type (if present and valid) and Medicine Name
        /// </summary>
        /// <param name="medicineId">MedicineID for which content is requried</param>
        /// <returns>Medicine Content string</returns>
        private string GetReminderContent(int medicineId)
        {
            var medicine = new MedicineViewModel(medicineId).Medicine;
            string dosageString = medicine.Dosage;
            double dosageDouble;
            if (String.IsNullOrWhiteSpace(dosageString) || !double.TryParse(dosageString, out dosageDouble))
                return String.Format("Take {0}\r\nAdd Dosage to get that info also here!\r\n[Click here to know more details]",
                    medicine.Name);
            else
                return String.Format("Take {0} {1} of {2}\r\n\r\n[Click here to know more details]",
                    dosageDouble, medicine.DosageType, medicine.Name);
        }

        /// <summary>
        /// Adds all the reminders to ReminderService of Windows Phone
        /// On every medicine update, all reminders are removed and rescheduled
        /// So, there is a chance that an existing reminder is expired. When we try to reschedule such reminder, an error is thrown
        /// We remove such reminders, by not adding them to guids list which will be ultimately saved
        /// </summary>
        /// <returns>Array of "all" reminder names added for the medicine</returns>
        public String[] ScheduleAllReminders()
        {
            var guids = new List<string>();
            foreach (var reminder in medicine.Reminders)
            {
                try
                {
                    ScheduledActionService.Add(reminder);
                    guids.Add(reminder.Name);
                }
                catch (Exception) { }

            }
            return guids.ToArray();
        }

        /// <summary>
        /// Removes all reminders registered under this medicine from ReminderService
        /// Sometimes there can be inconsistencies, we ignore such instances by catching those errors
        /// </summary>
        public void UnscheduleAllReminders()
        {
            if (medicine.ReminderIds == null)
                return;
            foreach (var reminderName in medicine.ReminderIds.Split(';'))
            {
                try
                {
                    ScheduledActionService.Remove(reminderName);
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Removes a reminder from in-memory list of Reminders
        /// </summary>
        /// <param name="name"></param>
        public void RemoveReminder(string name)
        {
            medicine.Reminders.Remove(medicine.Reminders.Single(r => r.Name == name));
        }

        /// <summary>
        /// Analogous to ToString() of any class
        /// </summary>
        /// <param name="reminderName">Unique name of the reminder</param>
        /// <returns>Shows the FromDate, ToDate, Frequency and Time of the Reminder</returns>
        public String GetReminderString(string reminderName)
        {
            string description = String.Empty;
            Reminder reminder = medicine.Reminders.Single(r => r.Name == reminderName);
            if (reminder != null)
                description = String.Format("From {0} to {1} at {2}. Repitition: {3}",
                        App.GetFormattedDate(reminder.BeginTime),
                        App.GetFormattedDate(reminder.ExpirationTime),
                        App.GetFormattedTime(reminder.ExpirationTime),
                        reminder.RecurrenceType);
            return description;
        }
    }
}
