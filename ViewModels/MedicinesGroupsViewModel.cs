using System;
using System.Collections.Generic;
using MedicinesCatalogue.Data;

namespace MedicinesCatalogue.ViewModels
{
    public class MedicinesGroupsViewModel
    {
        private static MedicineContext db = App.ViewModel.db;

        /// <summary>
        /// Updates the mapping between medicines and groups
        /// Removes the existing mappings and then adds these new ones
        /// Maps the passed groups for a given medicine
        /// </summary>
        /// <param name="Groups">Groups to be mapped against a medicine</param>
        /// <param name="medicine">Medicine to be mapped</param>
        public static void UpdateGroupsOfMedicine(IList<Group> groups, Medicine medicine)
        {
            db.MedicinesGroups.DeleteAllOnSubmit(medicine.GroupsIds);

            List<Medicine_Group> medicine_groups = new List<Medicine_Group>(groups.Count);
            foreach (var group in groups)
                medicine_groups.Add(new Medicine_Group() { GroupId = group.Id, MedicineId = medicine.Id });
            medicine.GroupsIds.Clear();
            medicine.GroupsIds.AddRange(medicine_groups);
        }
    }
}
