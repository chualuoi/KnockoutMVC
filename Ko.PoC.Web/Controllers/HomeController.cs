using System.Web.Mvc;

namespace Ko.PoC.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Choose appropiate demo and see how it works";
            return View();
        }

       
    }
}
