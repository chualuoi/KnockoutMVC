using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using KoLib.Mvc.ValidationInfrastructure.Attributes;
using KoLib.Mvc.ValidationInfrastructure.Helpers;

namespace KoLib.Mvc.ValidationInfrastructure.Contracts
{
    /// <summary>
    /// This will be base model for all other model.
    /// This class will do basic object validation, especially for collection
    /// </summary>
    public abstract class KoLibModelBase<TModel> : IValidatableObject
    {
        public virtual List<CollectionRule<TModel>> CollectionRules
        {
            get { return null; }
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CollectionRules == null)
            {
                yield return null;
            }
            //Iterate through all collection rules in this model
            foreach (var collectionRule in CollectionRules)
            {
                //Get the value of the collection
                var collectionValue = (IEnumerable)collectionRule.CollectionExpression.GetValue(this);
                if (collectionValue == null)
                {
                    continue;
                }
                
                //Invoke the collection rule for all item in the collection
                var itemIndex = 0;
                foreach (var item in collectionValue)
                {
                    collectionRule.HoldingValueExpression.SetValue(item, collectionRule.RuleExpression.IsTrue(this, itemIndex));
                    
                    //Check if the rule is valid
                    var isValid = (bool) collectionRule.HoldingValueExpression.GetValue(item);
                    if (!isValid)
                    {
                        //Get the error message for this rule
                        var ruleFor =
                            (RuleForAttribute)
                            collectionRule.HoldingValueExpression.PropertyInfo.GetCustomAttributes(
                                typeof (RuleForAttribute), false).FirstOrDefault();
                        if (ruleFor != null)
                        {
                            var collectionName =
                                ExpressionHelper.GetExpressionText(collectionRule.CollectionExpression.LambdaExpression);
                            var holdingValueName =
                                ExpressionHelper.GetExpressionText(
                                    collectionRule.HoldingValueExpression.LambdaExpression);
                            yield return
                                new ValidationResult(ruleFor.ErrorMessage,
                                                     new[]
                                                         {
                                                             string.Format("{0}[{1}].{2}", collectionName, itemIndex,
                                                                           holdingValueName)
                                                         });
                        }
                    }
                    itemIndex++;
                }                                
            }
        }
    }
}