using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AppAgentAdapter.Data;
using System.Linq;
using System.Collections.Generic;
using MedicinesCatalogue.Lib;
using System.Windows;
using System.Data.Linq;

namespace MedicinesCatalogue.ViewModels
{
    public class GroupViewModel : NotifyPropertyChangeDataError
    {
        MedicineContext db;

        private Group group;
        public Group Group
        {
            get { return group; }
            set
            {
                if (value != group)
                {
                    NotifyPropertyChanging("Group");
                    group = value;
                    NotifyPropertyChanged("Group");
                }
            }
        }

        private ObservableCollection<Group> groups;
        public ObservableCollection<Group> AllGroups
        {
            get { return groups; }
            set
            {
                if (value != groups)
                {
                    NotifyPropertyChanging("AllGroups");
                    groups = value;
                    NotifyPropertyChanged("AllGroups");
                }
            }
        }

        /// <summary>
        /// Sets the DB Context
        /// Loads all groups if asked for
        /// </summary>
        /// <param name="loadAllGroups">Flag to indicate whether to load groups</param>
        public GroupViewModel()
        {
            db = App.ViewModel.db;
            Group = new Group();
            LoadAllGroups();
        }

        public GroupViewModel(string groupName)
        {
            db = App.ViewModel.db;
            Group = db.Groups.Single(g => g.Name == groupName);
        }

        /// <summary>
        /// Creates a new group
        /// Group names should be UNIQUE. The SQL CE DB throws an execption if this constraint is not met
        /// Shows a Dialog Box assuming that whatever error is thrown at this stage is only because of UNIQUE constraint
        /// TODO: Find a way to detect the type of SQL CE error and handle accordingly
        /// </summary>
        /// <returns>True if successfully inserted, else false</returns>
        internal bool Create()
        {
            errorList.Clear();
            db.Groups.InsertOnSubmit(this.group);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception)
            {
                HandleDuplicateGroup();
                return false;
            }
            return true;
        }

        internal void LoadAllGroups()
        {
            AllGroups = new ObservableCollection<AppAgentAdapter.Data.Group>(db.Groups.OrderBy(g => g.Name));
        }

        internal void Save()
        {
            this.Group.UpdatedAt = DateTime.Now;
            try
            {
                db.SubmitChanges();
            }
            catch (Exception)
            {
                HandleDuplicateGroup();
            }
        }

        internal void Delete()
        {
            db.Groups.DeleteOnSubmit(this.group);
            db.SubmitChanges();
        }

        /// <summary>
        /// Shows the error message that the group name already exists
        /// Disposes the Connection and creates a new connection as there is no way to clear changes but not submitted
        /// </summary>
        private void HandleDuplicateGroup()
        {
            MessageBox.Show(String.Format("Group \"{0}\" already exists.\r\nUse the existing one or choose a different name", this.group.Name), "OOPS!", MessageBoxButton.OK);
            db.Dispose();
            db = App.ViewModel.GetNewConnection();
        }
    }
}
