using System.Web.Mvc;
using Ko.PoC.Models;

namespace Ko.PoC.Web.Controllers
{
    public class DemoController : Controller
    {
        public ActionResult Required()
        {
            return View();
        }        
    }
}
