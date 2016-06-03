using System;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.ModelBinder
{
    public class DecimalModelBinder : DefaultModelBinder
    {
        #region Implementation of IModelBinder

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult != null)
            {
                decimal newValue;

                return Decimal.TryParse(valueProviderResult.AttemptedValue, out newValue) ? (object)newValue : null;   
            }
            return null;
        }

        #endregion
    }
}