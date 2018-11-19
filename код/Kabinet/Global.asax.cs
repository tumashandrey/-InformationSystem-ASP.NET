using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using Kabinet.Models;

namespace Kabinet
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                JavaScriptSerializer serializer = new JavaScriptSerializer();

                var serializeModel = serializer.Deserialize<UserModel>(authTicket.UserData);

                var newUser = new CustomPrincipal(authTicket.Name)
                {
                    ID = serializeModel.ID,
                    Email = serializeModel.Email,
                    UserName = serializeModel.UserName,
                    IsAdmin = serializeModel.IsAdmin,
                };

                HttpContext.Current.User = newUser;
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
