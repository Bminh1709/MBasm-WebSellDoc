using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MBasmProject.Filter.Helper
{
    public class BlockDirectAccessAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            // Check if the user is accessing the action directly
            if (filterContext.Controller.ControllerContext.IsChildAction)
            {
                // Allow access if the action is being invoked as a child action
                return;
            }

            // Redirect or return an error result
            filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            // Alternatively, you can redirect the user to an error page or a different action
            // filterContext.Result = new RedirectResult("/Error/AccessDenied");
        }
    }
}