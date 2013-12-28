using System;
using System.ComponentModel;
using AppAgentAdapter.Data;
using System.Linq;
using System.Data.Linq;
using System.Collections.ObjectModel;
using MedicinesCatalogue.Lib;

namespace MedicinesCatalogue.ViewModels
{
    public class MedicineViewModel : NotifyPropertyChangeDataError
    {
        /// <summary>
        /// Connection to DB
        /// </summary>
        MedicineContext db;

        /// <summary>
        /// Current medicine which is active
        /// Used in DetailsPage and AddUpdatePage where a single medicine is considered
        /// </summary>
        private Medicine medicine;
        public Medicine Medicine
        {
            get
            {
                return medicine;
            }
            set
            {
                if (medicine != value)
                {
                    NotifyPropertyChanging("Medicine");
                    medicine = value;
                    NotifyPropertyChanged("Medicine");
                }
            }
        }

        /// <summary>
        /// Gets the existing connection if any for DB
        /// Creates an empty Medicine instance
        /// Used for creating a new medicine as we do not have any ID for it yet
        /// </summary>
        public MedicineViewModel()
        {
            db = App.ViewModel.db;
            db.Refresh(RefreshMode.OverwriteCurrentValues, db.Medicines); // Clear any unsaved changes
            Medicine = new Medicine();
        }

        /// <summary>
        /// Gets the existing connection if any for DB
        /// Used for updating an existing medicine
        /// </summary>
        /// <param name="medicineId">Medicine to be retrieved from DB</param>
        public MedicineViewModel(int medicineId)
        {
            db = App.ViewModel.db;
            db.Refresh(RefreshMode.OverwriteCurrentValues, db.Medicines); // Clear any unsaved changes
            Medicine = db.Medicines.SingleOrDefault(m => m.Id == medicineId);
        }

        /// <summary>
        /// Update this Medicine and save to DB
        /// </summary>
        internal void Save()
        {
            medicine.UpdatedAt = DateTime.Now;
            db.SubmitChanges();
        }

        /// <summary>
        /// Create a new medicine with current instance
        /// Sets the created_at and updated_at columns also
        /// </summary>
        internal void Create()
        {
            db = App.ViewModel.db;
            db.Medicines.InsertOnSubmit(this.medicine);
            db.SubmitChanges();
        }

        /// <summary>
        /// Deletes the current medicine
        /// Also removes this medicine from Medicines list in MainViewModel
        /// </summary>
        internal void Delete()
        {
            db.Medicines.DeleteOnSubmit(medicine);
            db.SubmitChanges();

            App.ViewModel.Medicines.Remove(medicine);
        }
    }
}
