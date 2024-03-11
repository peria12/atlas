// <copyright file="AircraftController.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>ChennakesavaRao</author>
// </copyright>
// <created Date> 05/10/2016  </created Date>
// <summary>To list out the aircrfts and export the data to excel </summary>

namespace GlobalNetApps.AtlasFlightSchedule.Controllers
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using Filters;
    using System.Web.Security;
    using System.Web;
    using DotNetOpenAuth.OAuth2;
    using System.Configuration;

   // [BaseAuthorize()]
    // [BaseAuthorize("Aircraft")]
    public class AircraftController : Controller
    {
        /// <summary>
        /// Create Log object instance and use where ever required to log info ,exceptions and etc..
        /// </summary>
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(AircraftController));
        private IAircraftService _aircraftServices;

        public const string AuthorizePath = "/Token";
        public const string TokenPath = "/Token";
        private static WebServerClient webServerClient;
        private static string accessToken, refreshToken;
        private static DateTime accessTokenExpiration;

        public AircraftController(IAircraftService aircraftServices)
        {
            _aircraftServices = aircraftServices;

        }

        //GET: List out the all Aircrats Reg No
        public ActionResult Index()
        {
            List<LOFViewModel> listLOF = new List<LOFViewModel>();
            //if (this.Session["accessToken"] == null)
            //{
            //    GetAccessToken();
            //}
           // string generatedToken = "";

            try
            {
                listLOF = _aircraftServices.GetAircraftRegNo(this);
                ViewBag.listLOF = listLOF;
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return View("Index");
        }

        // GET: Aircraft
        public ActionResult GetAircraft(string ID)
        {
            List<LOFViewModel> listLOF = new List<LOFViewModel>();
            //if (this.Session["accessToken"] == null)
            //{
            //    GetAccessToken();
            //}
            //string generatedToken = "";
            try
            {
                listLOF = _aircraftServices.GetAircrafts(this, ID);
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return View("GetAircraft", listLOF);
        }

        // GET: List out All Aircrafts
        public ActionResult GetAircrafts()
        {
            List<LOFViewModel> listLOF = new List<LOFViewModel>();
            //if (this.Session["accessToken"] == null)
            //{
            //    GetAccessToken();
           // }
            string generatedToken = "";
            try
            {
                listLOF = _aircraftServices.GetAircrafts(this, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return View("GetAircrafts", listLOF);
        }

        //Export Aircraft data to excel
        public ActionResult ExportToExcel(string ID)
        {
            List<LOFViewModel> data = null;
            //if (this.Session["accessToken"] == null)
            //{
            //    GetAccessToken();
            //}
            //string generatedToken = "";
            try
            {
                data = _aircraftServices.GetAircrafts(this, ID);
                /* if (data != null && data.Count > 0)
                 {
                     string filename = "AtlasFlightSchedule_" + data.FirstOrDefault().AircraftRegistration.Trim();
                     GridView gv = new GridView();
                     gv.DataSource = data;
                     gv.DataBind();
                     Response.ClearContent();
                     Response.Buffer = true;
                     Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");
                     Response.ContentType = "application/ms-excel";
                     Response.Charset = "";
                     StringWriter sw = new StringWriter();
                     HtmlTextWriter htw = new HtmlTextWriter(sw);
                     gv.RenderControl(htw);
                     Response.Output.Write(sw.ToString());
                     Response.Flush();
                     Response.End();
                 }*/
                if (data != null && data.Count > 0)
                {
                    string filename = "AtlasFlightSchedule_" + data.FirstOrDefault().AircraftRegistration.Trim() + ".xls";
                    //string filename = "acflowexcel.xls";
                    Response.AddHeader("content-disposition", "attachment; filename=" + filename);
                    Response.ContentType = "application/vnd.ms-excel";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            //    return View("GetAircraft", data);
            return PartialView("_ExportExcel", data);
        }

        //Export all Aircrfts data to excel
        public ActionResult ExportAllToExcel()
        {
            //if (this.Session["accessToken"] == null)
            //{
            //    GetAccessToken();
            //}
            //string generatedToken = "";

            List<LOFViewModel> data = null;
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            try
            {
                data = _aircraftServices.GetAircrafts(this, null);
                /*if (data != null && data.Count > 0)
                {
                    var listRegNo = data.GroupBy(m => m.AircraftRegistration).Select(s => s.First());
                    foreach (var lof in listRegNo)
                    {
                        var regSpan = new HtmlGenericControl("b");
                        regSpan.InnerHtml = "Flights for Aircraft  " + lof.AircraftRegistration + " :";

                        Literal ltr = new Literal();
                        ltr.Text = "<br/>";
                        GridView gv = new GridView();
                        var filterData = data.Where(m => m.AircraftRegistration == lof.AircraftRegistration);
                        gv.DataSource = filterData;
                        gv.DataBind();
                        regSpan.RenderControl(htw);
                        gv.RenderControl(htw);
                        ltr.RenderControl(htw);
                    }
                    string filename = "AtlasFlightSchedule_" + DateTime.Now.ToShortDateString();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    Response.Output.Write("<html><head><style type='text/css'></style><body></head>" + sw.ToString() + "</body></html>");
                    Response.Flush();
                    Response.End();
                }*/
                if (data != null && data.Count > 0)
                {
                    //string filename = "PolarFlightSchedule_" + DateTime.Now.ToShortDateString();
                    string filename = "acflowexcel.xls";
                    Response.AddHeader("content-disposition", "attachment; filename=" + filename);
                    Response.ContentType = "application/vnd.ms-excel";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            //return View("GetAircrafts", data);
            return PartialView("_ExportToExcelAll", data);
        }

        public void GetAccessToken()
        {
            AuthorizationClient authorizationClient = new AuthorizationClient();
            authorizationClient.ClientIdentification = ConfigurationManager.AppSettings["ClientIdentification"];
            authorizationClient.ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            string authorizationServerUri = ConfigurationManager.AppSettings["ApiUrl"].ToString();
            var authorizationServer = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(authorizationServerUri + AuthorizePath),
                TokenEndpoint = new Uri(authorizationServerUri + TokenPath)
            };

            webServerClient = new WebServerClient(authorizationServer, authorizationClient.ClientIdentification, authorizationClient.ClientSecret);

            try
            {
                authorizationClient.Password = "test";
                authorizationClient.UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //var state = webServerClient.GetClientAccessToken();
                var state = webServerClient.ExchangeUserCredentialForToken(authorizationClient.UserName, authorizationClient.Password);

                //var state = webServerClient.ExchangeUserCredentialForToken(authorizationClient.UserName, authorizationClient.Password, scopes: new string[] { "bio" });
                accessToken = state.AccessToken;
                refreshToken = state.RefreshToken;
                accessTokenExpiration = state.AccessTokenExpirationUtc ?? DateTime.UtcNow;

                ViewBag.Message = "Your authorization page.";

                HttpContext.Session.Add("accessToken", accessToken);
                HttpContext.Session.Add("refreshToken", refreshToken);
                HttpContext.Session.Add("accessTokenExpires", accessTokenExpiration.ToString());
                HttpContext.Session.Add("username", authorizationClient.UserName);
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
        }
    }
}