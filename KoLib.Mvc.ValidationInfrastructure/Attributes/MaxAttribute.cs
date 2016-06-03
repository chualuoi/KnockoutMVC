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
    public class MaxAttribute : ValidationAttribute, IClientValidatable
    {
        #region Ctors

        public MaxAttribute(int maximum)
            : this()
        {
            Maximum = maximum;
            OperandType = typeof (int);
        }

        public MaxAttribute(double maximum)
            : this()
        {
            Maximum = maximum;
            OperandType = typeof (double);
        }

        public MaxAttribute(decimal maximum)
            : this()
        {
            Maximum = maximum;
            OperandType = typeof (decimal);
        }

        public MaxAttribute(Type type, string maximum)
            : this()
        {
            OperandType = type;
            Maximum = maximum;
        }

        private MaxAttribute()
            : base((() => Constants.DefaultTooLargeErrorTemplate))
        {
        }

        #endregion

        #region Properties & Fields

        /// <summary>
        /// Template for custom error message on client
        /// </summary>
        protected string etemplate;

        public object Maximum { [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
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
            var comparable1 = (IComparable) Maximum;

            var result = comparable1.CompareTo(obj) >= 0;

            return result;
        }

        public override string FormatErrorMessage(string name)
        {
            SetupConversion();

            etemplate = string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, "{0}");

            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, (object) name, Maximum);
        }

        #endregion

        #region Implementation of IClientValidatable

        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = GetRule(metadata, context);

            yield return rule;
        }

        #endregion

        #region Supports

        protected virtual void Initialize(IComparable maximum, Func<object, object> conversion)
        {
            Maximum = maximum;
            Conversion = conversion;
        }

        private void SetupConversion()
        {
            if (Conversion != null)
                return;
            var maximum = Maximum;
            if (maximum == null)
            {
                throw new InvalidOperationException(DataAnnotationResources.RangeAttribute_Must_Set_Max);
            }
            var type1 = maximum.GetType();
            if (type1 == typeof(int))
            {
                Initialize((int)maximum, (v => (object)Convert.ToInt32(v, CultureInfo.InvariantCulture)));
            }
            else if (type1 == typeof(double))
            {
                Initialize((double)maximum, (v => (object)Convert.ToDouble(v, CultureInfo.InvariantCulture)));
            }
            else if (type1 == typeof(decimal))
            {
                Initialize((decimal)maximum, (v => (object)Convert.ToDecimal(v, CultureInfo.InvariantCulture)));
            }
            else
            {
                var type = OperandType;
                if (type == null)
                {
                    throw new InvalidOperationException(DataAnnotationResources.RangeAttribute_Must_Set_Operand_Type);
                }
                var type2 = typeof(IComparable);
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
                Initialize((IComparable)converter.ConvertFromString((string)maximum), (value =>
                {
                    if (value == null || !(value.GetType() == type))
                        return converter.ConvertFrom(value);
                    return value;
                }));
            }
        }

        protected virtual ModelClientValidationRule GetRule(ModelMetadata metadata, ControllerContext context)
        {
            var validationName = metadata.GetValidationName();

            var rule = new ModelClientValidationRule
                {
                    ErrorMessage = FormatErrorMessage(validationName),
                    ValidationType = "fmax"
                };

            rule.ValidationParameters.Add("etemplate", etemplate);
            rule.ValidationParameters.Add("max", Maximum);
            return rule;
        }

        #endregion

    }
}