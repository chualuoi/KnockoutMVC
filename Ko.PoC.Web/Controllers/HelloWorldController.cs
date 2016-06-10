using System.Web.Mvc;

namespace Ko.PoC.Web.Controllers
{
    public class HelloWorldController : BaseController
    {
        // GET: HelloWorld
        public ActionResult Index()
        {
            InitializeViewBag();
            return View();
        }
    }
}