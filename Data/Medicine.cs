using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;
using MedicinesCatalogue.Lib;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using MedicinesCatalogue.ViewModels;
using Microsoft.Phone.Scheduler;

namespace MedicinesCatalogue.Data
{
    [Table]
    public class Medicine : NotifyPropertyChangeDataError
    {
        private int id;
        [Column(IsPrimaryKey = true, CanBeNull = false, IsDbGenerated = true,
            DbType = "INT NOT NULL IDENTITY", AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    NotifyPropertyChanging("Id");
                    id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        private string name;
        [Column(CanBeNull = false, DbType = "nvarchar(50)")]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    string key = "Name";
                    if (!PopulateErrorListAdapter(key, Validations.Validate(key,
                        new Validation(Validation.ValidationType.CheckEmpty, value),
                        new Validation(Validation.ValidationType.CheckMaxLength, value, "50"))))
                    {
                        NotifyPropertyChanging(key);
                        name = value;
                        NotifyPropertyChanged(key);
                    }
                }
            }
        }

        private DateTime mfgDate = DateTime.Now; // Default manufactured date is now
        [Column]
        public DateTime MfgDate
        {
            get { return mfgDate; }
            set
            {
                if (mfgDate != value)
                {
                    string key = "MfgDate";
                    // TODO : Make "expDate == new DateTime()" condition better. Intention is to skip validations on initial binding
                    //if (expDate == new DateTime() || !PopulateErrorListAdapter(key, Validations.Validate(key, new Validation(Validation.ValidationType.CheckMaxDate, value, expDate))))
                    //{
                    NotifyPropertyChanging(key);
                    mfgDate = value;
                    NotifyPropertyChanged(key);
                    //}
                }
            }
        }

        private DateTime expDate = DateTime.Now.AddYears(3); // Default expiry date is 3 years from now
        [Column(CanBeNull = false)]
        public DateTime ExpDate
        {
            get { return expDate; }
            set
            {
                if (expDate != value)
                {
                    string key = "ExpDate";
                    // TODO : Make "expDate == new DateTime()" condition better. Intention is to skip validations on initial binding
                    //if (expDate == new DateTime() || !PopulateErrorListAdapter(key, Validations.Validate(key,
                    //     new Validation(Validation.ValidationType.CheckEmpty, value))))
                    //{
                    // TODO : Make "mfgDate == new DateTime()" condition better. Intention is to skip validations on initial binding
                    //if (mfgDate == new DateTime() || !PopulateErrorListAdapter(key, Validations.Validate(key, new Validation(Validation.ValidationType.CheckMinDate, value, mfgDate))))
                    //{
                    NotifyPropertyChanging(key);
                    expDate = value;
                    NotifyPropertyChanged(key);
                    //}
                    //}
                }
            }
        }

        /// <summary>
        /// Added in v1
        /// Indicates whether user is currently using the medicine or not
        /// If he is using the medicine, he will be alerted to take the medicine according to dosage field
        /// </summary>
        private bool isUsing = false; // By default user is not using the medicine
        [Column(DbType = "BIT NOT NULL DEFAULT 0")]
        public bool IsUsing
        {
            get { return isUsing; }
            set
            {
                if (value != isUsing)
                {
                    NotifyPropertyChanging("IsUsing");
                    isUsing = value;
                    NotifyPropertyChanged("IsUsing");
                }
            }
        }

        private string cureFor;
        [Column(DbType = "nvarchar(100) NULL")]
        public string CureFor
        {
            get { return cureFor; }
            set
            {
                if (cureFor != value)
                {
                    string key = "CureFor";
                    if (ApplicationHelper.ConvertToNullIfEmpty(ref value) || !PopulateErrorListAdapter(key, Validations.Validate(key,
                        new Validation(Validation.ValidationType.CheckMaxLength, value, "100"))))
                    {
                        NotifyPropertyChanging(key);
                        cureFor = value;
                        NotifyPropertyChanged(key);
                    }
                }
            }
        }

        private string dosage;
        [Column(DbType = "nvarchar(200) NULL")]
        public string Dosage
        {
            get { return dosage; }
            set
            {
                if (dosage != value)
                {
                    string key = "Dosage";
                    if (ApplicationHelper.ConvertToNullIfEmpty(ref value) || !PopulateErrorListAdapter(key, Validations.Validate(key,
                        new Validation(Validation.ValidationType.CheckMaxLength, value, "200"))))
                    {
                        NotifyPropertyChanging(key);
                        dosage = value;
                        NotifyPropertyChanged(key);
                    }
                }
            }
        }

        /// <summary>
        /// Added in v1
        /// Indicates the type of dosage for medicine, whether pellets, syrup or paste
        /// Dosage Types:
        /// 1) Pellets => pills
        /// 2) Syrup => ml
        /// 3) Paste => mg
        /// </summary>
        private String dosageType;
        [Column(DbType = "nvarchar(10) NULL")]
        public String DosageType
        {
            get { return dosageType; }
            set
            {
                if (value != dosageType)
                {
                    NotifyPropertyChanging("DosageType");
                    dosageType = value;
                    NotifyPropertyChanged("DosageType");
                }
            }
        }

        private string ageGroup;
        [Column(DbType = "nvarchar(50) NULL")]
        public string AgeGroup
        {
            get { return ageGroup; }
            set
            {
                if (ageGroup != value)
                {
                    string key = "AgeGroup";
                    if (ApplicationHelper.ConvertToNullIfEmpty(ref value) || !PopulateErrorListAdapter(key, Validations.Validate(key,
                        new Validation(Validation.ValidationType.CheckMaxLength, value, "50"))))
                    {
                        NotifyPropertyChanging(key);
                        ageGroup = value;
                        NotifyPropertyChanged(key);
                    }
                }
            }
        }

        // TODO: Try to do something better here rather than an object type. Have to support NULL values and Invalid entries like 'Unacceptable Value' as well
        //private object price;
        //[Column(DbType = "float NULL")]
        //public object Price
        //{
        //    get { return price; }
        //    set
        //    {
        //        if (price != value)
        //        {
        //            string key = "Price", _price = null;
        //            if (value != null)
        //            {
        //                _price = value.ToString();
        //                ApplicationHelper.ConvertToNullIfEmpty(ref _price);
        //            }
        //            if (_price != null)
        //            {
        //                try
        //                {
        //                    Double.Parse(_price);
        //                }
        //                catch (FormatException)
        //                {
        //                    throw new ArgumentException(String.Format(ApplicationHelper.errorInvalidArgumentMessage, key));
        //                }
        //                catch (Exception)
        //                {
        //                    throw new Exception(ApplicationHelper.errorGenericMessage);
        //                }
        //            }

        //            if (Validations.IsNull(_price) || !PopulateErrorListAdapter(key, Validations.Validate(key,
        //                new Validation(Validation.ValidationType.CheckPositive, _price))))
        //            {
        //                NotifyPropertyChanging(key);
        //                price = _price;
        //                NotifyPropertyChanged(key);
        //            }
        //        }
        //    }
        //}

        private double? price;
        [Column(DbType = "float NULL")]
        public double? Price
        {
            get { return price; }
            set
            {
                if (price != value)
                {
                    string key = "Price";
                    if (Validations.IsNull(value) || !PopulateErrorListAdapter(key, Validations.Validate(key,
                        new Validation(Validation.ValidationType.CheckPositive, value))))
                    {
                        NotifyPropertyChanging(key);
                        price = value;
                        NotifyPropertyChanged(key);
                    }
                }
            }
        }

        /// <summary>
        /// Added in v1
        /// Indicates the quantity remaining
        /// If the user is using the medicine, this quantity is updated using dosage field
        /// </summary>
        private double? quantity;
        [Column(DbType = "float NULL")]
        public double? Quantity
        {
            get { return quantity; }
            set
            {
                if (value != quantity)
                {
                    string key = "Quantity";
                    if (Validations.IsNull(value) || !PopulateErrorListAdapter(key, Validations.Validate(key,
                        new Validation(Validation.ValidationType.CheckPositive, value))))
                    {
                        NotifyPropertyChanging(key);
                        quantity = value;
                        NotifyPropertyChanged(key);
                    }
                }
            }
        }

        /// <summary>
        /// Added in v1
        /// Indicates the type of medicine, whether pellets, syrup or paste
        /// Quantity Types:
        /// 1) Pellets => pills
        /// 2) Syrup => ml
        /// 3) Paste => mg
        /// </summary>
        private String quantityType;
        [Column(DbType = "nvarchar(10) NULL")]
        public String QuantityType
        {
            get { return quantityType; }
            set
            {
                if (value != quantityType)
                {
                    ApplicationHelper.ConvertToNullIfEmpty(ref value);
                    NotifyPropertyChanging("QuantityType");
                    quantityType = value;
                    NotifyPropertyChanged("QuantityType");
                }
            }
        }

        /// <summary>
        /// Added in v1
        /// Squashed string of all reminder names (Ids) for this medicine
        /// All these reminders are separated by ';'
        /// Currently user can add upto (369 - 9) / 36 = 10 reminders per medicine. 9 separators => ;
        /// 36 is the size of GUID used as Reminder name
        /// </summary>
        private String reminderIds;
        [Column(DbType = "nvarchar(369) NULL", CanBeNull = true)]
        public String ReminderIds
        {
            get { return reminderIds; }
            set
            {
                if (value != reminderIds)
                {
                    reminderIds = value;
                }
            }
        }

        private ObservableCollection<Reminder> reminders;
        public ObservableCollection<Reminder> Reminders
        {
            get
            {
                // From DB, existing reminders
                //NotifyPropertyChanging("Reminders");
                if (reminders == null)
                {
                    reminders = new ObservableCollection<Reminder>();
                    if (ReminderIds != null)
                    {
                        var existingReminderNames = ReminderIds.Split(';');
                        var _reminders = ScheduledActionService.GetActions<Reminder>().Where(r => existingReminderNames.Contains(r.Name));
                        foreach (var _reminder in _reminders)
                            reminders.Add(_reminder);
                    }
                }
                //NotifyPropertyChanged("Reminders");
                return reminders;
            }
        }

        private string boughtAddress;
        [Column(DbType = "nvarchar(250) NULL")]
        public string BoughtAddress
        {
            get { return boughtAddress; }
            set
            {
                if (boughtAddress != value)
                {
                    string key = "BoughtAddress";
                    if (ApplicationHelper.ConvertToNullIfEmpty(ref value) || !PopulateErrorListAdapter(key, Validations.Validate(key,
                        new Validation(Validation.ValidationType.CheckMaxLength, value, "250"))))
                    {
                        NotifyPropertyChanging(key);
                        boughtAddress = value;
                        NotifyPropertyChanged(key);
                    }
                }
            }
        }

        private string pictureURI;
        [Column(DbType = "nvarchar(25) NULL")]
        public string PictureURI
        {
            get { return pictureURI; }
            set
            {
                if (value != pictureURI)
                {
                    NotifyPropertyChanging("PictureURI");
                    pictureURI = value;
                    NotifyPropertyChanged("PictureURI");
                    NotifyPropertyChanged("MedicineImage");
                }
            }
        }

        /// <summary>
        /// Used for DataBinding the image field. Populated using the PictureURI field
        /// </summary>
        public BitmapImage MedicineImage
        {
            get
            {
                if (PictureURI == null)
                    return ApplicationHelper.defaultImage; ;
                BitmapImage medicineImage = ApplicationHelper.GetImageFromIsolatedStorage(PictureURI);
                // No Image for this medicine
                if (medicineImage == null)
                    return ApplicationHelper.defaultImage; ;
                return medicineImage;
            }
        }

        /// <summary>
        /// Added in v1
        /// List of medicine_group mappings for the medicine
        /// This is the one side of many-one relationship
        /// A medicine can have many medicine-group mappings => belong to many groups
        /// </summary>
        private EntitySet<Medicine_Group> groupIds = new EntitySet<Medicine_Group>();
        [Association(Storage = "groupIds", ThisKey = "Id", OtherKey = "MedicineId", DeleteRule = "CASCADE")]
        public EntitySet<Medicine_Group> GroupsIds
        {
            get { return groupIds; }
            set
            {
                NotifyPropertyChanging("GroupsIds");
                groupIds.Assign(value);
                NotifyPropertyChanged("GroupsIds");
                NotifyPropertyChanged("Groups");
            }
        }

        /// <summary>
        /// Used for data binding
        /// Populated using the medicine-group mappings
        /// </summary>
        private ObservableCollection<Group> groups;
        public ObservableCollection<Group> Groups
        {
            get
            {
                groups = new ObservableCollection<Group>();
                foreach (var groupId in this.GroupsIds)
                    groups.Add(groupId.Group);
                return groups;
            }
            set
            {
                MedicinesGroupsViewModel.UpdateGroupsOfMedicine(value, this);
            }
        }

        private DateTime createdAt = DateTime.Now;
        [Column(CanBeNull = false)]
        public DateTime CreatedAt
        {
            get { return createdAt; }
            set
            {
                if (createdAt != value)
                {
                    NotifyPropertyChanging("CreatedAt");
                    createdAt = value;
                    NotifyPropertyChanged("CreatedAt");
                }
            }
        }

        private DateTime updatedAt = DateTime.Now;
        [Column(CanBeNull = false)]
        public DateTime UpdatedAt
        {
            get { return updatedAt; }
            set
            {
                if (updatedAt != value)
                {
                    NotifyPropertyChanging("UpdatedAt");
                    updatedAt = value;
                    NotifyPropertyChanged("UpdatedAt");
                }
            }
        }

        /// <summary>
        /// Used for search on Home Page
        /// </summary>
        /// <returns>A jammed string of all details of a medicine</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Name.Trim());
            sb.Append(this.MfgDate.ToShortDateString());
            sb.Append(this.ExpDate.ToShortDateString());
            sb.Append(this.Dosage);
            sb.Append(this.CureFor);
            sb.Append(this.AgeGroup);
            sb.Append(this.Price);
            sb.Append(this.BoughtAddress);
            sb.Append(this.Quantity);
            sb.Append(this.QuantityType);
            sb.Append(this.IsUsing ? "using" : "not using");
            return sb.ToString().ToLower();
        }
    }
}
