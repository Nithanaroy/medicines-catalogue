using System;
using System.Collections.Generic;

namespace MedicinesCatalogue.Lib
{
    class Validations
    {
        /// <summary>
        /// Error message for current validation.
        /// This is set whenever any validation method is called
        /// </summary>
        private static string message;
        public static string Message { get { return message; } }

        /// <summary>
        /// Checks whether passed string is empty. White spaces are also considered empty
        /// </summary>
        /// <param name="toCheck">String to be validated</param>
        /// <param name="fieldName">Field name to be mentioned in the error message for user readability</param>
        /// <returns>True if validation passes</returns>
        public static bool Blank(string toCheck, string fieldName = "Value")
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(toCheck))
            {
                message = fieldName + " can't be blank";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if passed string has letters less than some specified limit
        /// </summary>
        /// <param name="toCheck">String to be validated</param>
        /// <param name="maxLength">Maximum number of characters allowed</param>
        /// <param name="fieldName">Field name to be mentioned in the error message for user readability</param>
        /// <returns>True if validation passes</returns>
        public static bool MaxLength(string toCheck, int maxLength, string fieldName = "Value")
        {
            if (toCheck != null && toCheck.Length > maxLength)
            {
                message = String.Format("{0} has more than {1} letter(s)", fieldName, maxLength);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if specified number is positive
        /// </summary>
        /// <param name="toCheck">Number to check</param>
        /// <param name="fieldName">Field name to be mentioned in the error message for user readability</param>
        /// <returns>True if validation passes</returns>
        public static bool Positive(double toCheck, string fieldName = "Value")
        {
            if (toCheck <= 0)
            {
                message = fieldName + " should be positive";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if given date is less than or equal to certain date
        /// </summary>
        /// <param name="toCheck">Date to check</param>
        /// <param name="maxValue">Maximum permissible Date</param>
        /// <param name="fieldName">Field name to be mentioned in the error message for user readability</param>
        /// <returns>True if validation passes</returns>
        public static bool MaxDate(DateTime toCheck, DateTime maxValue, string fieldName = "Value")
        {
            if (toCheck > maxValue)
            {
                message = String.Format("{0} is more than allowed {1} value", fieldName, maxValue.ToString("MMMM dd, yyyy"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if given date is greater than or equal to a specified date
        /// </summary>
        /// <param name="toCheck">Date to check</param>
        /// <param name="minValue">Minimum requiqred Date</param>
        /// <param name="fieldName">Field name to be mentioned in the error message for user readability</param>
        /// <returns>True if validation passes</returns>
        public static bool MinDate(DateTime toCheck, DateTime minValue, string fieldName = "Value")
        {
            if (toCheck < minValue)
            {
                message = String.Format("{0} is more than allowed {1} value", fieldName, minValue.ToString("MMMM dd, yyyy"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if passed object is null
        /// </summary>
        /// <param name="toCheck">Object to check</param>
        /// <param name="fieldName">Field name to be mentioned in the error message for user readability</param>
        /// <returns>True if object is NULL</returns>
        public static bool IsNull(object toCheck, string fieldName = "Value")
        {
            if (toCheck == null)
                return true;
            return false;
        }

        /// <summary>
        /// Checks if specified number is positive
        /// </summary>
        /// <param name="toCheck">Number to check</param>
        /// <param name="fieldName">Field name to be mentioned in the error message for user readability</param>
        /// <returns>True if validation passes</returns>
        public static bool NonNegative(double toCheck, string fieldName = "Value")
        {
            if (toCheck < 0)
            {
                message = fieldName + " can't be negative";
                return false;
            }
            return true;
        }

        /// <summary>
        /// For multiple validations use this method
        /// </summary>
        /// <param name="fieldName">To get error message with name of the field</param>
        /// <param name="validations">Array of validations to perform</param>
        /// <returns>List of error messsages</returns>
        public static List<String> Validate(string fieldName, params Validation[] validations)
        {
            List<String> errorMessages = new List<String>();
            foreach (Validation validationArgs in validations)
            {
                switch (validationArgs.TypeOfValidation)
                {
                    case Validation.ValidationType.CheckEmpty:
                        if (!Blank(validationArgs.ValueToValidate.ToString(), fieldName))
                            errorMessages.Add(message);
                        break;
                    case Validation.ValidationType.CheckMaxLength:
                        if (!MaxLength(validationArgs.ValueToValidate.ToString(), Convert.ToInt32(validationArgs.ArgumentsForValidation[0]), fieldName))
                            errorMessages.Add(message);
                        break;
                    case Validation.ValidationType.CheckPositive:
                        if (!Positive(Convert.ToDouble(validationArgs.ValueToValidate), fieldName))
                            errorMessages.Add(message);
                        break;
                    case Validation.ValidationType.CheckMaxDate:
                        if (!MaxDate(Convert.ToDateTime(validationArgs.ValueToValidate), Convert.ToDateTime(validationArgs.ArgumentsForValidation[0]), fieldName))
                            errorMessages.Add(message);
                        break;
                    case Validation.ValidationType.CheckMinDate:
                        if (!MinDate(Convert.ToDateTime(validationArgs.ValueToValidate), Convert.ToDateTime(validationArgs.ArgumentsForValidation[0]), fieldName))
                            errorMessages.Add(message);
                        break;
                    case Validation.ValidationType.CheckNonNegative:
                        if (!NonNegative(Convert.ToDouble(validationArgs.ValueToValidate), fieldName))
                            errorMessages.Add(message);
                        break;
                    default:
                        break;
                }
            }
            return errorMessages;
        }
    }

    /// <summary>
    /// Structure of a validation
    /// </summary>
    class Validation
    {
        /// <summary>
        /// Available validations
        /// </summary>
        public enum ValidationType
        {
            CheckEmpty,
            CheckMaxLength,
            CheckPositive,
            CheckMaxDate,
            CheckMinDate,
            CheckNonNegative
        };

        private ValidationType _validationType; // Type of validation
        private object _valueToValidate;  // Value to validate
        private object[] _argsForValidation; // Arguments required for validation. Eg: For MaxLength() maximum length allowed value

        public Validation(ValidationType validationType, object valueToValidate)
        {
            this._validationType = validationType;
            this._valueToValidate = valueToValidate;
            this._argsForValidation = null;
        }

        public Validation(ValidationType validationType, object valueToValidate, params object[] argumentsForValidation)
        {
            this._validationType = validationType;
            this._valueToValidate = valueToValidate;
            this._argsForValidation = argumentsForValidation;
        }

        public ValidationType TypeOfValidation { get { return _validationType; } }
        public object ValueToValidate { get { return _valueToValidate; } }
        public object[] ArgumentsForValidation { get { return _argsForValidation; } }
    }
}
