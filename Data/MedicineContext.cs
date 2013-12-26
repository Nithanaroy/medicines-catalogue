using System;
using System.Data.Linq;

namespace MedicinesCatalogue.Data
{
    public class MedicineContext : DataContext
    {
        public MedicineContext(string connectionString) : base(connectionString) { }

        public Table<Medicine> Medicines;
        public Table<Group> Groups;
        public Table<Medicine_Group> MedicinesGroups;
    }
}
