using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MedicinesCatalogue.Lib
{
    /// <summary>
    /// Available quntity types
    /// </summary>
    public class QuantityTypes
    {
        /// <summary>
        /// All the available types of quantities. 
        /// Note: The empty string is for none of the above. Very imp. Same is shown in the UI ::TODO::
        /// </summary>
        private string[] availableTypes = { "", "pills", "ml", "mg", "strips", "cups", "spoons" };
        public string[] AvailableTypes
        {
            get { return availableTypes; }
        }


        /// <summary>
        /// Available medicine types
        /// </summary>
        public enum TypeFor
        {
            Pills,
            Syrup,
            Tube,
            Strips,
            Cups,
            Spoons
        }

        /// <summary>
        /// Gets display string for a medicine type
        /// </summary>
        /// <param name="medicineType">The type of medicine</param>
        /// <returns>Display string for the medicine type</returns>
        public static string GetDisplayStringFor(TypeFor medicineType)
        {
            switch (medicineType)
            {
                case TypeFor.Pills:
                    return "pills";
                case TypeFor.Syrup:
                    return "ml";
                case TypeFor.Tube:
                    return "mg";
                case TypeFor.Strips:
                    return "strips";
                case TypeFor.Cups:
                    return "cups";
                case TypeFor.Spoons:
                    return "spoons";
                default:
                    return "pills";
            }
        }
    }
}
