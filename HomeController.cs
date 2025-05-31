using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CLMSAPP.Controllers
{
    [SessionCheck]
    public class HomeController : Controller
    {

        //for every page include this for session logged
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    if (HttpContext.Session.GetString("Username") == null)
        //    {
        //        context.Result = RedirectToAction("Login", "User");
        //    }

        //    // Disable caching
        //    Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        //    Response.Headers["Pragma"] = "no-cache";
        //    Response.Headers["Expires"] = "0";

        //    base.OnActionExecuting(context);
        //}

        public IActionResult Index()
        {
            return View();
        }
    }
}
