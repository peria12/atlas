// <copyright file="ErrorController.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>Chennakesava</author>
// </copyright>
// <created Date> 05/23/2016  </created Date>
// <summary> </summary>

namespace GlobalNetApps.AtlasFlightSchedule.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;

    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return Redirect(FormsAuthentication.LoginUrl);
        }
    }
}