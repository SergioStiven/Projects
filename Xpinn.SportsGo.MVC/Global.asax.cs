using System.Web.Mvc;
using System.Web.Routing;
using FreshMvvm;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util;
using System.Web;
using System;
using Xpinn.SportsGo.Util.Portable.Abstract;
using System.Net.Http;

namespace Xpinn.SportsGo.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Se registra las implementaciones para asi la capa Services pueda resolver la dependencia :D
            FreshIOC.Container.Register<ISecureableMessage, SecureMessagesHelper>().AsSingleton();
            FreshIOC.Container.Register<HttpMessageHandler, HttpClientHandler>().AsSingleton();

            FreshIOC.Container.Register<ILockeable, LockHelper>().AsMultiInstance();
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Response.Clear();
            Context.Server.ClearError();

            Response.RedirectToRoute("Default");
        }
    }
}