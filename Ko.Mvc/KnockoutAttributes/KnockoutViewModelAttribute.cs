using System;

namespace Ko.Mvc.KnockoutAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class KnockoutViewModelAttribute : Attribute
    {
        /// <summary>
        /// Indicates the classes marked this attribute is the root view model for the page
        /// </summary>
        public bool RootIndicator { get; set; }

        /// <summary>
        /// If this property is not null, nor empty, nor whitespace, 
        /// it will be set as the name of the generated file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets whether current view model is knockout view model.
        /// By default, it is true and you don't have to specify it in the attribute declaration.
        /// However, this is used to specify that specific class is not a knockout view model event it matches the knockout view model naming convention
        /// </summary>
        public bool IsKnockoutViewModel { get; set; }

        public KnockoutViewModelAttribute()
        {
            FileName = string.Empty;
            RootIndicator = false;
            IsKnockoutViewModel = true;
        }
    }
}