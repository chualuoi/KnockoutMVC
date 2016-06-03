using System.ComponentModel.DataAnnotations;

namespace Ko.PoC.Models
{
    public class RequiredModel
    {
        [Required]
        public string RequiredField { get; set; }
    }
}