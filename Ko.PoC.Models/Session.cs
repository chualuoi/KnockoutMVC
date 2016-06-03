using Ko.Mvc.KnockoutAttributes;

namespace Ko.PoC.Models
{
    [KnockoutViewModel]
    public class Session
    {
        public string Name { get; set; }
        public Person Speaker { get; set; }
    }
}