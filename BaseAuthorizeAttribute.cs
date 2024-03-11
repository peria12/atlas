// <copyright file="BaseAuthorizeAttribute.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>Chennakesava</author>
// </copyright>
// <created Date>05/23/2016  </created Date>
// <summary>Custom authorization implementation </summary>

namespace GlobalNetApps.AtlasFlightSchedule.Filters
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Configuration;

    /// <summary>
    /// Custom Authorization implemented using Authorize Attribute
    /// </summary>
    public class BaseAuthorizeAttribute : AuthorizeAttribute
    {
        public BaseAuthorizeAttribute()
        {
            this.Roles = ConfigurationManager.AppSettings.Get("AccessLevel");
           
        }
        /// <summary>
        /// Pass group  or role names into the class constructor
        /// </summary>
        /// <param name="RolesList">Role names</param>
        public BaseAuthorizeAttribute(string RolesList)
        { 
            if (RolesList=="Flight")
            {
                this.Roles = ConfigurationManager.AppSettings.Get("FSAccessLevel");
            }
            else

            { this.Roles = ConfigurationManager.AppSettings.Get("AccessLevel"); }
        }

        /// <summary>
        /// This method will call while authorizing the controller or action level events
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            //Read cookies name from request object
            var authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                //Decrypt the cookies detaisl for to find out the user data
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket != null && !authTicket.Expired)
                {
                    var usergroups = authTicket.UserData.Split(',');
                    HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket), usergroups);
                    //foreach (var group in Roles.Split(','))
                    //{
                    //    //Valid user roles 
                    //    if (HttpContext.Current.User.IsInRole(group))
                    //    {
                    //        base.OnAuthorization(filterContext);
                    //        return;
                    //    }
                    //}
                    //Return to error page if user is not authorized
                    base.OnAuthorization(filterContext);
                    return;
                    //filterContext.Result = new RedirectToRouteResult(
                    //    new RouteValueDictionary(new { Controller = "Error", Action = "AccessDenied" }));
                }
                else
                {
                    //Return to login page if user session expired.
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { Controller = "Error", Action = "Dashboard" }));
                }
            }
            else
            {
                //Return to login page if user cookie expired.
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { Controller = "Error", Action = "Dashboard" }));
            }
        }
    }


}