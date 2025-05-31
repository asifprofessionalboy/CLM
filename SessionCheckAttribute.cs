using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CLMSAPP
{
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = (Controller)context.Controller;

            if (controller.HttpContext.Session.GetString("Username") == null)
            {
                context.Result = new RedirectToActionResult("Login", "User", null);
            }

            controller.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            controller.Response.Headers["Pragma"] = "no-cache";
            controller.Response.Headers["Expires"] = "0";

            base.OnActionExecuting(context);
        }
    }
}
