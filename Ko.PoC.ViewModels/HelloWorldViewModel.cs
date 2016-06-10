using Ko.Mvc.KnockoutAttributes;

namespace Ko.PoC.ViewModels
{
    public class HelloWorldViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [KnockoutComputed]
        public string FullName { get { return FirstName + " " + LastName; } }
    }
}
