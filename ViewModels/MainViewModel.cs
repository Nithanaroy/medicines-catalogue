using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MedicinesCatalogue.Lib;
using AppAgentAdapter.Data;
using MedicinesCatalogue.ViewModels;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;

namespace MedicinesCatalogue
{
    public class MainViewModel : NotifyPropertyChangeDataError
    {
        /// <summary>
        /// Connection to DB
        /// </summary>
        public MedicineContext db { get; private set; }

        /// <summary>
        /// Is true if medicines are fetched at least once from DB
        /// </summary>
        private bool AreAllMedicinesLoaded;

        /// <summary>
        /// Flag for checking whether groups are loaded
        /// </summary>
        private bool AreAllGroupsLoaded;

        public MainViewModel()
        {
            db = AppAgentAdapter.App.db;
        }

        /// <summary>
        /// Instantiates new DB connection. Used when we dispose the old connection
        /// </summary>
        /// <returns>New connection to DB</returns>
        public MedicineContext GetNewConnection()
        {
            db = AppAgentAdapter.App.db;
            return db;
        }

        /// <summary>
        /// List of all medicines in DB
        /// </summary>
        private ObservableCollection<Medicine> medicines;
        public ObservableCollection<Medicine> Medicines
        {
            get { return medicines; }
            set
            {
                if (medicines != value)
                {
                    NotifyPropertyChanging("Medicines");
                    medicines = value;
                    NotifyPropertyChanged("Medicines");
                }
            }
        }

        /// <summary>
        /// For groups screen
        /// </summary>
        private IQueryable<PublicGrouping<string, Medicine>> medicineGroups;
        public IQueryable<PublicGrouping<string, Medicine>> MedicineGroups
        {
            get { return medicineGroups; }
            private set
            {
                if (value != medicineGroups)
                {
                    NotifyPropertyChanging("MedicineGroups");
                    medicineGroups = value;
                    NotifyPropertyChanged("MedicineGroups");
                }
            }
        }

        /// <summary>
        /// Dispose the DatabaseContext
        /// </summary>
        ~MainViewModel()
        {
            db.Dispose();
            db = null;
        }

        /// <summary>
        /// Fetches all medicines from DB and refreshes the In Memory Object -> Medicines
        /// Refreshing is required when we need to reload data from DB as they were changed in other places
        /// </summary>
        /// <param name="loadGroups">Loads groups also</param>
        public void LoadAllMedicines(bool loadGroups = false)
        {
            if (!AreAllMedicinesLoaded)
                Medicines = new ObservableCollection<Medicine>(
                db.Medicines.OrderByDescending(m => m.Id));

            this.AreAllMedicinesLoaded = true;

            if (loadGroups)
                LoadAllMedicinesByGroups();
        }

        /// <summary>
        /// Fetches all medicines groups and refreshes in mem object -> MedicineGroups if changed
        /// </summary>
        /// <param name="loadForce">Fetches groups even though they are retieved from DB. Used for updating UI</param>
        public void LoadAllMedicinesByGroups(bool loadForce = false)
        {
            if (loadForce || ApplicationHelper.HasDbChanged(db) || !AreAllGroupsLoaded)
                MedicineGroups = from m in db.Medicines
                                 join mg in db.MedicinesGroups on m.Id equals mg.MedicineId
                                 join g in db.Groups on mg.GroupId equals g.Id
                                 group m by g.Name into groupName
                                 select new PublicGrouping<string, Medicine>(groupName);

            AreAllGroupsLoaded = true;
        }


        public static int getNUmberOfMedicines()
        {
            return new Random().Next(10);
        }
    }
}