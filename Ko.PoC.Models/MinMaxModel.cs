using System.ComponentModel.DataAnnotations;

namespace Ko.PoC.Models
{
    public class MinMaxModel
    {
        [Display(Name = "Enter less than 10 value")]
        public int LessThan10 { get; set; }

        [Display(Name = "Enter greater than 10 value")]
        public int GreaterThan10 { get; set; }

        [Display(Name = "Enter number between 10 and 100")]
        public int InRange10To100 { get; set; }
    }
}