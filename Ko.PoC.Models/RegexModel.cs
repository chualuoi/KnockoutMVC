using System.ComponentModel.DataAnnotations;

namespace Ko.PoC.Models
{
    public class RegexModel
    {
        [Display(Name = "This is character only property")]
        public string CharacterOnly { get; set; } 
    }
}