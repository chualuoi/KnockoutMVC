using Ko.Mvc.KnockoutAttributes;
using Ko.PoC.Models;

namespace Ko.PoC.ViewModels
{
    public class DemoViewModel
    {
        public RequiredModel RequiredModel { get; set; }
        public RegexModel RegexModel { get; set; }
        public MinMaxModel MinMaxModel { get; set; }
    }
}