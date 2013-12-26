using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Linq;
using System.Collections.Generic;
using MedicinesCatalogue.Lib;

namespace MedicinesCatalogue.Data
{
    [Table]
    public class Group : NotifyPropertyChangeDataError
    {
        /// <summary>
        /// Primary Key column. Didn't use Name as primary key as user can edit the name
        /// </summary>
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

        /// <summary>
        /// Group name should be unique
        /// System throws SQLCeException if this constraint is not met. 
        /// This is caught while saving the changes -> db.SubmitChanges()
        /// </summary>
        private String name;
        [Column(DbType = "nvarchar(30) NOT NULL UNIQUE", CanBeNull = false)]
        public String Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    string key = "Name";
                    if (!PopulateErrorListAdapter(key, Validations.Validate(key,
                        new Validation(Validation.ValidationType.CheckEmpty, value),
                        new Validation(Validation.ValidationType.CheckMaxLength, value, "30"))))
                    {
                        NotifyPropertyChanging("Name");
                        name = value;
                        NotifyPropertyChanged("Name");
                    }
                }
            }
        }

        /// <summary>
        /// The one side of many-one relationship
        /// A group has many medicine_group mappings => many medicines
        /// This mapping will be deleted if the group is deleted
        /// </summary>
        private EntitySet<Medicine_Group> medicineIds = new EntitySet<Medicine_Group>();
        [Association(Storage = "medicineIds", ThisKey = "Id", OtherKey = "GroupId", DeleteRule = "CASCADE")]
        public EntitySet<Medicine_Group> MedicineIds
        {
            get { return medicineIds; }
            set
            {
                NotifyPropertyChanging("MedicineIds");
                medicineIds.Assign(value);
                NotifyPropertyChanged("MedicineIds");
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
        /// Used during ListPicker binding in AddUpdatePage.xaml
        /// </summary>
        /// <returns>Name of the Group</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
