using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime;
using System.Web.Mvc;
using KoLib.Mvc.ValidationInfrastructure.Attributes.Resources;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class MinAttribute : ValidationAttribute, IClientValidatable
    {
        #region Ctors

        public MinAttribute(int minimum)
            : this()
        {
            Minimum = minimum;
            OperandType = typeof (int);
        }

        public MinAttribute(double minimum)
            : this()
        {
            Minimum = minimum;
            OperandType = typeof (double);
        }

        public MinAttribute(decimal minimum)
            : this()
        {
            Minimum = minimum;
            OperandType = typeof (decimal);
        }

        public MinAttribute(Type type, string minimum)
            : this()
        {
            OperandType = type;
            Minimum = minimum;
        }

        private MinAttribute()
            : base((() => Constants.DefaultTooSmallErrorTemplate))
        {
        }

        #endregion

        #region Properties & Fields

        /// <summary>
        /// Template for custom error message on client
        /// </summary>
        protected string etemplate;

        public object Minimum { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; protected set; }

        public Type OperandType { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get; protected set; }

        private Func<object, object> Conversion { get; set; }

        #endregion

        #region Overrides of ValidationAttribute

        public override bool IsValid(object value)
        {
            SetupConversion();
            if (value == null)
                return true;
            var str = value as string;
            if (str != null && string.IsNullOrEmpty(str))
                return true;
            object obj;
            try
            {
                obj = Conversion(value);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
            var comparable1 = (IComparable) Minimum;

            var result = comparable1.CompareTo(obj) <= 0;

            return result;
        }

        public override string FormatErrorMessage(string name)
        {
            SetupConversion();

            etemplate = string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, "{0}");

            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, (object) name, Minimum);
        }

        #endregion

        #region Supports

        private void Initialize(IComparable minimum, Func<object, object> conversion)
        {
            Minimum = minimum;
            Conversion = conversion;
        }

        private void SetupConversion()
        {
            if (Conversion != null)
                return;
            var minimum = Minimum;
            if (minimum == null)
            {
                throw new InvalidOperationException(DataAnnotationResources.RangeAttribute_Must_Set_Min);
            }
            var type1 = minimum.GetType();
            if (type1 == typeof (int))
            {
                Initialize((int) minimum, (v => (object) Convert.ToInt32(v, CultureInfo.InvariantCulture)));
            }
            else if (type1 == typeof (double))
            {
                Initialize((double) minimum, (v => (object) Convert.ToDouble(v, CultureInfo.InvariantCulture)));
            }
            else if (type1 == typeof (decimal))
            {
                Initialize((decimal) minimum, (v => (object) Convert.ToDecimal(v, CultureInfo.InvariantCulture)));
            }
            else
            {
                var type = OperandType;
                if (type == null)
                {
                    throw new InvalidOperationException(DataAnnotationResources.RangeAttribute_Must_Set_Operand_Type);
                }
                var type2 = typeof (IComparable);
                if (!type2.IsAssignableFrom(type))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            DataAnnotationResources.RangeAttribute_ArbitraryTypeNotIComparable,
                            new object[]
                                {
                                    type.FullName,
                                    type2.FullName
                                }
                            )
                        );
                }
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                Initialize((IComparable) converter.ConvertFromString((string) minimum), (value =>
                    {
                        if (value == null || !(value.GetType() == type))
                            return converter.ConvertFrom(value);
                        return value;
                    }));
            }
        }

        #endregion

        #region Implementation of IClientValidatable

        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = GetRule(metadata, context);

            yield return rule;
        }

        protected virtual ModelClientValidationRule GetRule(ModelMetadata metadata, ControllerContext context)
        {
            var validationName = metadata.GetValidationName();

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(validationName),
                ValidationType = "fmin"
            };

            rule.ValidationParameters.Add("etemplate", etemplate);
            rule.ValidationParameters.Add("min", Minimum);

            return rule;
        }
        #endregion
    }
}