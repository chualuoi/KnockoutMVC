using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.ModelBinder
{
    public class CustomModelBindingContext : ModelBindingContext
    {
        public CustomModelBindingContext()
        {
            ComputedReadOnlyProperties = new List<string>();
            InapplicableProperties = new List<string>();
            IncludedProperties = new List<string>();
            LockedReadOnlyProperties = new List<string>();
            PropertyOriginalValues = new Dictionary<string, object>();
            OriginalModelPopulated = false;
            OriginalModelCompleted = false;
        }

        #region Properties

        public List<string> ComputedReadOnlyProperties { get; set; }

        public Dictionary<string, object> PropertyOriginalValues { get; set; }

        public List<string> InapplicableProperties { get; set; }
        public List<string> IncludedProperties { get; set; }
        public List<string> LockedReadOnlyProperties { get; set; }

        private object originalModel;
        public object OriginalModel
        {
            get { return originalModel; }
            set
            {
                if (!OriginalModelPopulated && value != null)
                {
                    OriginalModelPopulated = true;
                    originalModel = value;
                }
            }
        }

        public bool OriginalModelPopulated { get; private set; }

        public bool OriginalModelCompleted { get; set; }

        public bool RebindingIndicator { get; set; }

        #endregion

        #region Filters

        /// <summary>
        /// Checks if a property should be validated 
        /// </summary>
        /// <remarks>
        /// Normally, all properties will be validated (IncludedProperties will be empty).
        /// However in some case, we only want to bind/validate some particular properties.
        /// This predicator will return true if
        /// -> includedProperties is empty: normal case
        /// --OR--
        /// -> includedProperties has one item
        /// ----> contains the given property
        /// ------OR-----
        /// ----> is applicable and validatable.
        /// </remarks>
        public Predicate<string> ValidatablePropertyFilter
        {
            get
            {
                return propertyToBeValidated =>
                       IsApplicable(propertyToBeValidated)
                        && (
                        IncludedProperties.Count == 0
                            || IsValidatable(propertyToBeValidated)
                        );
            }
        }

        /// <summary>
        /// Checks if a property should be bound
        /// </summary>
        /// <remarks>
        /// Normally, a property will not be bound by ModelBinder if it has no public method Set or it has attribute ReadOnly(true).
        /// But for some case, we 
        /// </remarks>
        public Predicate<string> FrameworkPropertyFilter
        {
            get
            {
                return propertyToBeBound =>
                       IsApplicable(propertyToBeBound)
                       && PropertyFilter(propertyToBeBound)
                       && ValidatablePropertyFilter(propertyToBeBound)
                       && CanWrite(propertyToBeBound)
                    ;
            }
        }

        /// <summary>
        /// Checks if a propety is applicable
        /// </summary>
        /// <remarks>
        /// A property is applicable if it is not in InapplicableProperties
        /// </remarks>
        private Predicate<string> IsApplicable
        {
            get { return onCheckedProperty => !InapplicableProperties.Contains(onCheckedProperty); }
        }

        /// <summary>
        /// Checks if a propety is validatable
        /// </summary>
        /// <remarks>
        /// A property is validatable if it is in ValidatableProperties
        /// </remarks>
        private Predicate<string> IsValidatable
        {
            get
            {
                return propertyToBeValidated =>
                           IncludedProperties.Any(
                               validatableProperty =>
                                    string.Equals(propertyToBeValidated, validatableProperty)
                                    || validatableProperty.StartsWith(propertyToBeValidated + ".")
                               );
            }
        }

        /// <summary>
        /// Checks if a propety can be updated
        /// </summary>
        /// <remarks>
        /// A property can be updated if it is not nor in LockedReadOnlyPropertyProperties neither ComputedReadOnlyProperties
        /// </remarks>
        private Predicate<string> CanWrite
        {
            get
            {
                return onCheckedProperty =>
                       !LockedReadOnlyProperties.Contains(onCheckedProperty)
                       && !ComputedReadOnlyProperties.Contains(onCheckedProperty);
            }
        }

        #endregion
    }

    public static class ModelBindingContextExtensions
    {
        public static bool IsRoot(this ModelBindingContext context)
        {
            return context != null && string.IsNullOrWhiteSpace(context.ModelName);
        }
    }
}
