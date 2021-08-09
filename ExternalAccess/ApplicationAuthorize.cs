using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
//using System.Web.Http.Filters;
using System.Web.Http;
using System.Threading;
using System.Net.Http;

//using System.Web.Mvc;

namespace CFSWebService.ExternalAccess
{

    public class ApplicationAuthorizeAttribute : AuthorizeAttribute
    {

      
        public override void OnAuthorization(HttpActionContext actionContext)
        {
          
            var identity = Thread.CurrentPrincipal.Identity;

            if (identity == null && HttpContext.Current != null)
            {
                identity = HttpContext.Current.User.Identity;
            }

            if (identity != null && !identity.IsAuthenticated)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }

           
        }

    }   

}