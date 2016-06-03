using Ko.Mvc.KnockoutAttributes;

namespace Ko.PoC.Models
{
    [KnockoutViewModel]
    public class Person
    {
        public string Name { get; set; }
        public string Occupation { get; set; }
    }
}