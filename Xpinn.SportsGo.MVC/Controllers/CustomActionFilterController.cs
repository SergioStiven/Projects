using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public class CustomActionFilterController : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            // TODO: Add your action filter's tasks here
            
            var Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var Action = string.Concat(filterContext.ActionDescriptor.ActionName, " (Logged By: Custom Action Filter)");
            var IP = filterContext.HttpContext.Request.UserHostAddress;
            var DateTime = filterContext.HttpContext.Timestamp;
        }
    }
}