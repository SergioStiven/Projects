using Xpinn.SportsGo.Util;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web.Http.Dispatcher;
using Xpinn.SportsGo.Util.Portable;
using System.Web.Http.Cors;

namespace Xpinn.SportsGo.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Se registra el handler que encripta y desencripta los requests y response, de manera global si lo pones aca
            //GlobalConfiguration.Configuration.MessageHandlers.Add(new EncryptedServerHandler(new SecureMessagesHelper()));
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings = JsonSerializerOptionsConfiguration.ReturnJsonSerializerSettings();

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            var cors = new EnableCorsAttribute(@"*", "*", "*");
            config.EnableCors(cors);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: null,
                handler: new EncryptedServerHandler(new SecureMessagesHelper(), new HttpControllerDispatcher(GlobalConfiguration.Configuration)) // De manera por route si lo pones aca
            );
        }
    }
}
