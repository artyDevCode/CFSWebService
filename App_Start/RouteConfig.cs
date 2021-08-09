using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CFSWebService
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "DefaultChart",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Chart", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "DefaultManage",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Manage", action = "Index", id = UrlParameter.Optional }
           );
        }
    }
}
