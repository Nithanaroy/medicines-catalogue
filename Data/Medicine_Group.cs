using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using MedicinesCatalogue.Lib;

namespace MedicinesCatalogue.Data
{
    /// <summary>
    /// Join table for medicines and groups many-many relationship
    /// </summary>
    [Table]
    public class Medicine_Group : NotifyPropertyChangeDataError
    {
        [Column(IsPrimaryKey = true)]
        public int MedicineId { get; set; }

        [Column(IsPrimaryKey = true)]
        public int GroupId { get; set; }

        /// <summary>
        /// Many side of many-one relationship
        /// </summary>
        private EntityRef<Medicine> medicine;
        [Association(Storage = "medicine", ThisKey = "MedicineId", OtherKey = "Id", IsForeignKey = true)]
        public Medicine Medicine
        {
            get { return medicine.Entity; }
            set
            {
                NotifyPropertyChanging("Medicine");
                medicine.Entity = value;
                if (value != null)
                    MedicineId = value.Id;
                NotifyPropertyChanged("Medicine");
            }
        }

        /// <summary>
        /// Many side of many-one relationship
        /// </summary>
        private EntityRef<Group> group;
        [Association(Storage = "group", ThisKey = "GroupId", OtherKey = "Id", IsForeignKey = true)]
        public Group Group
        {
            get { return group.Entity; }
            set
            {
                NotifyPropertyChanging("Group");
                group.Entity = value;
                if (value != null)
                    GroupId = value.Id;
                NotifyPropertyChanged("Group");
            }
        }
    }
}
