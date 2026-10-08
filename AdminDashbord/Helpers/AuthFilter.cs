using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdminDashbord.Helpers
{
    /// <summary>
    /// Ye filter check karta hai ke user logged in hai ya nahi.
    /// Agar logged in nahi hai to login page pe redirect kar deta hai.
    /// </summary>
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var adminId = session.GetString("AdminId");

            if (string.IsNullOrEmpty(adminId))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
