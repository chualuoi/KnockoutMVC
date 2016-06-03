using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace KoLib.Mvc.ValidationInfrastructure.Attributes
{
    public class FriendlyCaseIdAttribute : ValidationAttribute, IClientValidatable
    {
        private static readonly int[][] multiplication = new[]
                                                             {
                                                        new[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
                                                        new[]{1, 2, 3, 4, 0, 6, 7, 8, 9, 5},
                                                        new[]{2, 3, 4, 0, 1, 7, 8, 9, 5, 6},
                                                        new[]{3, 4, 0, 1, 2, 8, 9, 5, 6, 7},
                                                        new[]{4, 0, 1, 2, 3, 9, 5, 6, 7, 8},
                                                        new[]{5, 9, 8, 7, 6, 0, 4, 3, 2, 1},
                                                        new[]{6, 5, 9, 8, 7, 1, 0, 4, 3, 2},
                                                        new[]{7, 6, 5, 9, 8, 2, 1, 0, 4, 3},
                                                        new[]{8, 7, 6, 5, 9, 3, 2, 1, 0, 4},
                                                        new[]{9, 8, 7, 6, 5, 4, 3, 2, 1, 0}
                                                    };

        private static readonly int[][] permuatation = new int[10][];

        //private static int[] inverse = new[]{ 0, 4, 3, 2, 1, 5, 6, 7, 8, 9 };
        private const string Prefix = "M";
        private const int SequentialSeed = 55000100;

        static FriendlyCaseIdAttribute()
        {
            permuatation[0] = new[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            permuatation[1] = new[]{1, 5, 7, 6, 2, 8, 3, 0, 9, 4};            
            for (var outer = 2; outer < 8; outer++)
            {
                permuatation[outer] = new int[10];
            
                for (var inner = 0; inner < 10; inner++)
                {
                    permuatation[outer][inner] = permuatation[outer - 1][permuatation[1][inner]];
                }
            }            
        }

        #region Override ValidationAttribute

        public override bool IsValid(object value)
        {
            var result = false;
            if (value == null)
            {
                return true;
            }
            if (!(value is string))
            {
                return base.IsValid(value);
            }
            var stringValue = value.ToString();
            var friendlyIdPrefix = stringValue.Substring(0, 1);
            var friendlyIdWithoutPrefix = stringValue.Substring(1);

            if (friendlyIdPrefix == Prefix)
            {
                var friendlyIdSequentialId = stringValue.Substring(1, stringValue.Length - 2);
                int friendlyIdSequentialIdInt;

                if (int.TryParse(friendlyIdSequentialId, out friendlyIdSequentialIdInt) &&
                    friendlyIdSequentialIdInt >= SequentialSeed)
                {
                    var toCheck = (from char c in friendlyIdWithoutPrefix.Reverse() select int.Parse(c.ToString())).ToArray();
                    var checkDigit = 0;

                    for (var index = 0; index < toCheck.Length; index++)
                    {
                        checkDigit = multiplication[checkDigit][permuatation[index%8][toCheck[index]]];
                    }

                    result = (checkDigit == 0);
                }
            }
            return result;
        }

        public override string FormatErrorMessage(string name)
        {
            //Use default message if there is no message specified
            if (string.IsNullOrEmpty(ErrorMessage) && string.IsNullOrWhiteSpace(ErrorMessageResourceName))
            {
                ErrorMessage = Constants.DefaultInvalidFriendlyCaseIdErrorTemplate;
            }
            return base.FormatErrorMessage(name);
        }
        #endregion


        #region Implementation of IClientValidatable

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
                           {
                               ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                               ValidationType = "friendlycaseid"
                           };
            yield return rule;
        }

        #endregion
    }
}