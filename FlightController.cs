// <copyright file="FlightController.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>ChennakesavaRao</author>
// </copyright>
// <created Date> 05/10/2016  </created Date>
// <summary>To get the scheduled flights ,hotel info and view the gendec form </summary>

namespace GlobalNetApps.AtlasFlightSchedule.Controllers
{
    using GlobalNetApps.AtlasFlightSchedule.Interfaces;
    using Models;
    using PdfSharp.Drawing;
    using PdfSharp.Drawing.Layout;
    using PdfSharp.Pdf;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using Filters;
    using System.Web.Security;
    using DotNetOpenAuth.OAuth2;
    using System.Configuration;

   // [BaseAuthorize()]
    //[BaseAuthorize("Flight")]
    public class FlightController : Controller
    {
        /// <summary>
        /// Create Log object instance and use where ever required to log info ,exceptions and etc..
        /// </summary>
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(FlightController));
        protected IFlightServices _flightServices = null;

        public const string AuthorizePath = "/Token";
        public const string TokenPath = "/Token";
        private static WebServerClient webServerClient;
        private static string accessToken, refreshToken;
        private static DateTime accessTokenExpiration;

        public FlightController(IFlightServices flightServices)
        {
            _flightServices = flightServices;

        }

        // GET: Flight
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["grid-page"]))
            {
                return Index(TempData.Peek("LookUpViewModel") as ViewModel);
            }
            ViewModel model = null;
            try
            {
                model = GetFlightViewModel();
                ViewBag.PageAction = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ViewModel lModel)
        {
            TempData["LookUpViewModel"] = lModel;
           
            ViewModel model = new ViewModel();
            try
            {
                //if (this.Session["accessToken"] == null)
                //{
                //    GetAccessToken();
                //}
                string generatedToken = "";
                ViewBag.PageAction = true;
                //newModel.IsInbound = Convert.ToString(Request.Form["LookupModel.IsInbound"]) == "InBound" ? true : false;
                //newModel.StartDate = Convert.ToDateTime(Request.Form["LookupModel.StartDate"]);
                //newModel.EndDate = Convert.ToDateTime(Request.Form["LookupModel.EndDate"]);
                //newModel.Station = Convert.ToString(Request.Form["LookupModel.Station"]);
                
                model.FlightViewModel = _flightServices.GetFlights(lModel.LookupModel, this, generatedToken);
                model.LookupModel = lModel.LookupModel;

            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return View("Index", model);
        }

        //to get the flight data
        [HttpGet]
        public ActionResult GetFlight(String SeqNo, bool isHotel)
        {
            FlightHotelViewModel fhModel = new FlightHotelViewModel();
            try
            {
                //if (this.Session["accessToken"] == null)
                //{
                //    GetAccessToken();
               // }
                string generatedToken = "";

                List<FlightViewModel> lstFlights = _flightServices.GetFlightDetails(SeqNo, this, generatedToken);
                fhModel.FlightViewModel = lstFlights;
                fhModel.HotelViewModel = isHotel ? _flightServices.GetHotelsBySeqNo(Convert.ToInt32(SeqNo), this, generatedToken) : null;
                if (lstFlights != null && lstFlights.Count > 0)
                {
                    DateTime compareDate = DateTime.Now.Month == 12 ? Convert.ToDateTime("01/01/" + (DateTime.Now.Year + 1)) : Convert.ToDateTime((DateTime.Now.Month + 1) + "/01/" + DateTime.Now.Year);
                    ViewBag.isDisplayNames = (DateTime.Now.Day >= 23 && DateTime.Now.Day < 26 && (Convert.ToDateTime(lstFlights[0].OrigDepartureDate) > compareDate)) ? false : true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return View("GetFlight", fhModel);
        }

        //to get the holtels by station
        [HttpGet]
        public ActionResult GetHotels(String Station)
        {
            //if (this.Session["accessToken"] == null)
            //{
            //    GetAccessToken();
            //}
            string generatedToken = "";

            List<HotelViewModel> hModel = new List<HotelViewModel>();
            try
            {
                hModel = _flightServices.GetHotelsByStation(Station, this, generatedToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return View("GetHotels", hModel);
        }

        [HttpGet]
        public ActionResult GetGenDec(String SeqNo)
        {
            FlightViewModel fltModel = new FlightViewModel();
            FlightGenDecViewModel fltGenModel = new FlightGenDecViewModel();
            string CrewIDs = string.Empty;
            string CabinCrewIDs = string.Empty;
            string[] CA = { };
            string[] FO = { };
            string[] FE = { };
            string[] PU = { };
            string[] AP = { };
            string[] FA = { };

            //if (this.Session["accessToken"] == null)
            //{
            //    GetAccessToken();
           // }
            string generatedToken = "";
            try
            {
                var flightRouteData = _flightServices.GetFlightRouteData(SeqNo, this, generatedToken);
                if (flightRouteData != null && flightRouteData.Count > 0)
                {
                    fltModel = flightRouteData[0];
                    fltGenModel.FlightViewModel = fltModel;

                    flightRouteData.Remove(fltModel);
                    fltGenModel.FlightListViewModel = flightRouteData;

                    if (!string.IsNullOrEmpty(fltModel.CMNameCA))
                    {
                        CA = fltModel.CMNameCA.Split(';');
                        foreach (string s in CA)
                        {
                            string[] arr = s.Split('-');
                            CrewIDs = !string.IsNullOrEmpty(arr[0]) ? CrewIDs + arr[0].Trim() + "," : CrewIDs;
                        }
                    }
                    if (!string.IsNullOrEmpty(fltModel.CMNameFO))
                    {
                        FO = fltModel.CMNameFO.Split(';');
                        foreach (string s in FO)
                        {
                            string[] arr = s.Split('-');
                            CrewIDs = !string.IsNullOrEmpty(arr[0]) ? CrewIDs + arr[0].Trim() + "," : CrewIDs;
                        }
                    }
                    if (!string.IsNullOrEmpty(fltModel.CMNameFE))
                    {
                        FE = fltModel.CMNameFE.Split(';');
                        foreach (string s in FE)
                        {
                            string[] arr = s.Split('-');
                            CrewIDs = !string.IsNullOrEmpty(arr[0]) ? CrewIDs + arr[0].Trim() + "," : CrewIDs;
                        }
                    }
                    if (!string.IsNullOrEmpty(fltModel.CMNamePU))
                    {
                        foreach (string s in PU)
                        {
                            PU = fltModel.CMNamePU.Split(';');
                            string[] arr = s.Split('-');
                            CabinCrewIDs = !string.IsNullOrEmpty(arr[0]) ? CabinCrewIDs + arr[0].Trim() + "," : CabinCrewIDs;
                        }
                    }
                    if (!string.IsNullOrEmpty(fltModel.CMNameAP))
                    {
                        foreach (string s in AP)
                        {
                            AP = fltModel.CMNameAP.Split(';');
                            string[] arr = s.Split('-');
                            CabinCrewIDs = !string.IsNullOrEmpty(arr[0]) ? CabinCrewIDs + arr[0].Trim() + "," : CabinCrewIDs;
                        }
                    }
                    if (!string.IsNullOrEmpty(fltModel.CMNameFA))
                    {
                        foreach (string s in FA)
                        {
                            FA = fltModel.CMNameFA.Split(';');
                            string[] arr = s.Split('-');
                            CabinCrewIDs = !string.IsNullOrEmpty(arr[0]) ? CabinCrewIDs + arr[0].Trim() + "," : CabinCrewIDs;
                        }
                    }

                    CrewIDs = !string.IsNullOrEmpty(CrewIDs) ? CrewIDs.Substring(0, CrewIDs.Length - 1) : CrewIDs;
                    CabinCrewIDs = !string.IsNullOrEmpty(CabinCrewIDs) ? CabinCrewIDs.Substring(0, CabinCrewIDs.Length - 1) : CabinCrewIDs;

                    ViewBag.TotalCrew = CA.Length + FO.Length + FE.Length;
                    ViewBag.TotalCabinCrew = PU.Length + AP.Length + FA.Length;

                    //fltGenModel.CrewViewModel = _flightServices.GetCrewData(CrewIDs, CabinCrewIDs);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return View(fltGenModel);
        }

        //to get the gendec pdf
        [HttpGet]
        public ActionResult GetGenDecForm(String SeqNo)
        {
            FlightViewModel fltModel = new FlightViewModel();
            FlightGenDecViewModel fltGenModel = new FlightGenDecViewModel();
            string CrewIDs = string.Empty;
            string CabinCrewIDs = string.Empty;
            string[] CA = { };
            string[] FO = { };
            string[] FE = { };
            string[] PU = { };
            string[] AP = { };
            string[] FA = { };

            //if (this.Session["accessToken"] == null)
            //{
            //    GetAccessToken();
          //  }
            string generatedToken = "";
            try
            {
                var flightRouteData = _flightServices.GetFlightRouteData(SeqNo, this, generatedToken);
                if (flightRouteData != null && flightRouteData.Count > 0)
                {
                    fltModel = flightRouteData[0];
                    fltGenModel.FlightViewModel = fltModel;

                    flightRouteData.Remove(fltModel);
                    fltGenModel.FlightListViewModel = flightRouteData;

                    if (!string.IsNullOrEmpty(fltModel.CMNameCA))
                    {
                        CA = fltModel.CMNameCA.Split(';');
                        foreach (string s in CA)
                        {
                            string[] arr = s.Split('-');
                            CrewIDs = !string.IsNullOrEmpty(arr[0]) ? CrewIDs + arr[0].Trim() + "," : CrewIDs;
                        }
                    }
                    if (!string.IsNullOrEmpty(fltModel.CMNameFO))
                    {
                        FO = fltModel.CMNameFO.Split(';');
                        foreach (string s in FO)
                        {
                            string[] arr = s.Split('-');
                            CrewIDs = !string.IsNullOrEmpty(arr[0]) ? CrewIDs + arr[0].Trim() + "," : CrewIDs;
                        }
                    }
                    if (!string.IsNullOrEmpty(fltModel.CMNameFE))
                    {
                        FE = fltModel.CMNameFE.Split(';');
                        foreach (string s in FE)
                        {
                            string[] arr = s.Split('-');
                            CrewIDs = !string.IsNullOrEmpty(arr[0]) ? CrewIDs + arr[0].Trim() + "," : CrewIDs;
                        }
                    }
                    if (!string.IsNullOrEmpty(fltModel.CMNamePU))
                    {
                        foreach (string s in PU)
                        {
                            PU = fltModel.CMNamePU.Split(';');
                            string[] arr = s.Split('-');
                            CabinCrewIDs = !string.IsNullOrEmpty(arr[0]) ? CabinCrewIDs + arr[0].Trim() + "," : CabinCrewIDs;
                        }
                    }
                    if (!string.IsNullOrEmpty(fltModel.CMNameAP))
                    {
                        foreach (string s in AP)
                        {
                            AP = fltModel.CMNameAP.Split(';');
                            string[] arr = s.Split('-');
                            CabinCrewIDs = !string.IsNullOrEmpty(arr[0]) ? CabinCrewIDs + arr[0].Trim() + "," : CabinCrewIDs;
                        }
                    }
                    if (!string.IsNullOrEmpty(fltModel.CMNameFA))
                    {
                        foreach (string s in FA)
                        {
                            FA = fltModel.CMNameFA.Split(';');
                            string[] arr = s.Split('-');
                            CabinCrewIDs = !string.IsNullOrEmpty(arr[0]) ? CabinCrewIDs + arr[0].Trim() + "," : CabinCrewIDs;
                        }
                    }

                    CrewIDs = !string.IsNullOrEmpty(CrewIDs) ? CrewIDs.Substring(0, CrewIDs.Length - 1) : CrewIDs;
                    CabinCrewIDs = !string.IsNullOrEmpty(CabinCrewIDs) ? CabinCrewIDs.Substring(0, CabinCrewIDs.Length - 1) : CabinCrewIDs;

                    ViewBag.TotalCrew = CA.Length + FO.Length + FE.Length;
                    ViewBag.TotalCabinCrew = PU.Length + AP.Length + FA.Length;

                    List<CrewViewModel> crewViewModel = null;
                    //_flightServices.GetCrewData(CrewIDs, CabinCrewIDs);
                    PdfDocument gendec = CreateGendec(fltModel, crewViewModel, ViewBag.TotalCrew, ViewBag.TotalCabinCrew);

                    MemoryStream stream = new MemoryStream();
                    gendec.Save(stream, false);
                    byte[] bytes = stream.ToArray();

                    Response.Clear();

                    Response.Cache.SetCacheability(HttpCacheability.Public);
                    Response.Cache.SetExpires(DateTime.MinValue);

                    Response.Buffer = true;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + "Gendec_" + fltModel.FltETDDate + fltModel.FltNo + "-" + fltModel.DepartureStation + "_" + fltModel.ArrivalStation + ".pdf");
                    Response.Charset = "";
                    Response.Cache.SetCacheability(HttpCacheability.Private);
                    Response.BinaryWrite(bytes);
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return RedirectToAction("Index");
        }

        private ViewModel GetFlightViewModel()
        {
            ViewModel vModel = new ViewModel();
            LookupViewModel lModel = new LookupViewModel();
            try
            {
                lModel.IsInbound = true;
                lModel.StartDate = DateTime.Now;
                lModel.EndDate = DateTime.Now.AddDays(+7);
                vModel.LookupModel = lModel;
                vModel.FlightViewModel = null;
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return vModel;
        }
        //to create the pdf document
        public PdfDocument CreateGendec(FlightViewModel flight, List<CrewViewModel> crewModel, int totalCrew, int totalCabinCrew)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "GenDec Form";
            PdfPage page = document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);
            try
            {

                #region Header
                gfx.DrawString("GENERAL DECLARATION",
                                new XFont("Verdana", 14, XFontStyle.Underline),
                                XBrushes.Black,
                                new XRect(new XPoint(0, 0), new XSize(page.Width, 40)),
                                XStringFormats.Center);

                gfx.DrawString("OUTWARD/INWARD",
                                new XFont("Verdana", 12, XFontStyle.Regular),
                                XBrushes.Black,
                                new XRect(new XPoint(0, 0), new XSize(page.Width, 70)),
                                XStringFormats.Center);

                XRect annex_rectangle = new XRect(page.Width - 80, 20, 60, 20);
                gfx.DrawRectangle(new XPen(XColors.Black, 1), annex_rectangle);

                annex_rectangle.Y += -4;
                gfx.DrawString("ICAO ANNEX 9",
                                new XFont("Verdana", 6, XFontStyle.Regular),
                                XBrushes.Black,
                                annex_rectangle,
                                XStringFormats.Center);

                annex_rectangle.Y += 8;
                gfx.DrawString("Appendix 1",
                        new XFont("Verdana", 6, XFontStyle.Regular),
                        XBrushes.Black,
                        annex_rectangle,
                        XStringFormats.Center);

                gfx.DrawString("Operator",
                        new XFont("Verdana", 10, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(new XPoint(10, 60), new XSize(10, 80)),
                        XStringFormats.TopLeft);

                gfx.DrawString("ATLAS AIR",
                        new XFont("Verdana", 10, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(80, 60), new XSize(60, 80)),
                        XStringFormats.TopLeft);

                if (flight.FltType != null)
                    gfx.DrawString(flight.FltType,
                            new XFont("Verdana", 10, XFontStyle.Bold),
                            XBrushes.Black,
                            new XRect(new XPoint(380, 60), new XSize(60, 80)),
                            XStringFormats.TopLeft);

                gfx.DrawString("Marks of Nationality and Registration",
                        new XFont("Verdana", 10, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(new XPoint(10, 80), new XSize(10, 80)),
                        XStringFormats.TopLeft);

                gfx.DrawString(flight.AircraftRegistration,
                        new XFont("Verdana", 10, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(230, 80), new XSize(10, 80)),
                        XStringFormats.TopLeft);

                gfx.DrawString("Flight No.",
                        new XFont("Verdana", 10, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(new XPoint(330, 80), new XSize(10, 80)),
                        XStringFormats.TopLeft);

                gfx.DrawString(Convert.ToString(flight.FltNo),
                        new XFont("Verdana", 10, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(380, 80), new XSize(10, 80)),
                        XStringFormats.TopLeft);

                gfx.DrawString("Date",
                        new XFont("Verdana", 10, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(new XPoint(450, 80), new XSize(10, 80)),
                        XStringFormats.TopLeft);

                gfx.DrawString(flight.FltETDDate,
                        new XFont("Verdana", 10, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(480, 80), new XSize(10, 80)),
                        XStringFormats.TopLeft);


                gfx.DrawString("Departure from",
                        new XFont("Verdana", 10, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(new XPoint(10, 100), new XSize(10, 80)),
                        XStringFormats.TopLeft);


                gfx.DrawString(flight.DepartureStation,
                    new XFont("Verdana", 10, XFontStyle.Bold),
                    XBrushes.Black,
                    new XRect(new XPoint(120, 100), new XSize(10, 80)),
                    XStringFormats.TopLeft);

                gfx.DrawString("Arrival at",
                        new XFont("Verdana", 10, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(new XPoint(330, 100), new XSize(10, 80)),
                        XStringFormats.TopLeft);

                gfx.DrawString(flight.ArrivalStation,
                        new XFont("Verdana", 10, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(390, 100), new XSize(10, 80)),
                        XStringFormats.TopLeft);

                gfx.DrawRectangle(new XPen(XColors.Black, 1), new XRect(10, 133, page.Width - 20, 2));

                gfx.DrawString("FLIGHT ROUTING",
                        new XFont("Verdana", 9, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(10, 123), new XSize(page.Width, 40)),
                        XStringFormats.Center);

                gfx.DrawString("(\"Airport:\" Column always to list origin, every en-route stop and destination)",
                        new XFont("Verdana", 6, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(new XPoint(10, 130), new XSize(page.Width, 40)),
                        XStringFormats.Center);

                #endregion


                Int32 crew_count = totalCrew + flight.JSCount + totalCabinCrew;
                Int32 sob = crew_count;

                Int32 sob_box_height = crew_count * 11 + 40;
                if (sob_box_height < 160)
                    sob_box_height = 160;

                Int32 sob_box_top = 160;

                gfx.DrawRectangle(new XPen(XColors.Black, 1), new XRect(10, sob_box_top, page.Width - 15, 30));
                gfx.DrawRectangle(new XPen(XColors.Black, 1), new XRect(10, sob_box_top + 30, page.Width - 15, sob_box_height - 30));

                Int32 airport_text_right = 12;
                gfx.DrawString("AIR",
                        new XFont("Verdana", 8, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(airport_text_right, sob_box_top + 2), new XSize(10, 30)),
                        XStringFormats.TopLeft);
                gfx.DrawString("PORT",
                        new XFont("Verdana", 8, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(airport_text_right, sob_box_top + 12), new XSize(10, 30)),
                        XStringFormats.TopLeft);
                gfx.DrawLine(new XPen(XColors.Black, 1), 40, sob_box_top, 40, sob_box_top + sob_box_height); //airport

                Int32 crew_info_text_right = 42;
                gfx.DrawString("CREW INFORMATION",
                        new XFont("Verdana", 8, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(crew_info_text_right, sob_box_top + 2), new XSize(10, 30)),
                        XStringFormats.TopLeft);
                gfx.DrawLine(new XPen(XColors.Black, 1), 505, sob_box_top, 505, sob_box_top + sob_box_height); //crew

                Int32 pax_number_text_right = 507;
                gfx.DrawString("NO. OF PASSENGERS",
                        new XFont("Verdana", 8, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(pax_number_text_right, sob_box_top + 2), new XSize(10, 60)),
                        XStringFormats.TopLeft);
                gfx.DrawString("ON THIS STAGE",
                        new XFont("Verdana", 8, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(pax_number_text_right, sob_box_top + 12), new XSize(10, 60)),
                        XStringFormats.TopLeft);


                Int32 top = sob_box_top + 32;


                String stage_line = "...............................\n";
                String stage_string = "Departure Place:\n" +
                                      stage_line +
                                      "Embarking:\n" +
                                      stage_line +
                                      "Through on same flight:\n" +
                                      stage_line +
                                      "Arrival Place:\n" +
                                      stage_line +
                                      "Disembarking:\n" +
                                      stage_line +
                                      "Through on same flight:\n" +
                                      stage_line;

                XTextFormatter t_f = new XTextFormatter(gfx) { Alignment = XParagraphAlignment.Left };
                t_f.DrawString(stage_string,
                        new XFont("Verdana", 6, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(new XPoint(pax_number_text_right, top), new XSize(100, 80)),
                        XStringFormats.TopLeft);


                gfx.DrawString(flight.DepartureStation,
                        new XFont("Verdana", 8, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(airport_text_right, top), new XSize(10, 30)),
                        XStringFormats.TopLeft);

                gfx.DrawString(flight.ArrivalStation,
                        new XFont("Verdana", 8, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(new XPoint(airport_text_right, top + 10), new XSize(10, 30)),
                        XStringFormats.TopLeft);
                foreach (CrewViewModel cModel in crewModel)
                {
                    gfx.DrawString(cModel.CrewInformation,
                              new XFont("Verdana", 8, XFontStyle.Regular),
                              XBrushes.Black,
                              new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                              XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat1))
                {
                    flight.Jseat1 = flight.Jseat1.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat1,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat2))
                {
                    flight.Jseat2 = flight.Jseat2.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat2,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat3))
                {
                    flight.Jseat3 = flight.Jseat3.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat3,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat4))
                {
                    flight.Jseat4 = flight.Jseat4.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat4,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat5))
                {
                    flight.Jseat5 = flight.Jseat5.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat5,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat6))
                {
                    flight.Jseat6 = flight.Jseat6.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat6,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat7))
                {
                    flight.Jseat7 = flight.Jseat7.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat7,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat8))
                {
                    flight.Jseat8 = flight.Jseat8.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat8,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat9))
                {
                    flight.Jseat9 = flight.Jseat9.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat9,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat10))
                {
                    flight.Jseat10 = flight.Jseat10.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat10,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat11))
                {
                    flight.Jseat11 = flight.Jseat11.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat11,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }
                if (!string.IsNullOrEmpty(flight.Jseat12))
                {
                    flight.Jseat12 = flight.Jseat12.Replace(",,", ",").Replace(",,", ",");
                    gfx.DrawString(flight.Jseat12,
                            new XFont("Verdana", 8, XFontStyle.Regular),
                            XBrushes.Black,
                            new XRect(new XPoint(crew_info_text_right, top), new XSize(10, 30)),
                            XStringFormats.TopLeft);
                    top += 10;
                }

                //Lower large box
                gfx.DrawRectangle(new XPen(XColors.Black, 1), new XRect(10, 500, page.Width - 20, 240));

                gfx.DrawString("DECLARATION OF HEALTH",
                        new XFont("Verdana", 8, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(12, 502, 100, 10),
                        XStringFormats.TopLeft);


                String declaration_text = "Name and seat number or function of persons on board with illness other than " +
                                          "airsickness or effects of accidents, who may be suffering from a communicable " +
                                          "disease (a fever-temperature 38°C/100°F or greater-associated with one or more of the " +
                                          "following signs or symptoms, e.g. appearing obviously unwell; persistent coughing " +
                                          "impaired breathing; persistent diarrhea; persistent vomiting; skin rash; bruising " +
                                          "or bleading without previous injury; or confusion of recent onset, increases the " +
                                          "likelihood that the person is suffering from a communicable disease) as well as such cases " +
                                          "of illness disembarked during a previous stop...........................................................................................\n" +
                                          ".............................................................................................................................................\n" +
                                          "Details of each disinsecting or sanitary treatment (place, date, time, method) during the flight. " +
                                          "if no disinsecting has been carried out during the flight, give details of the most recent disinsecting\n" +
                                          ".............................................................................................................................................\n" +
                                          " Signed, if required, with the time and date................................................................................\n" +
                                          "                                                                                                              Crew Member concerned";

                XTextFormatter tf = new XTextFormatter(gfx) { Alignment = XParagraphAlignment.Justify };
                tf.DrawString(declaration_text,
                        new XFont("Verdana", 8, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(12, 512, 410, 130),
                        XStringFormats.TopLeft);


                //for official use only
                gfx.DrawRectangle(new XPen(XColors.Black, 1), new XRect(430, 500, 172, 40));
                gfx.DrawString("FOR OFFICIAL USE",
                        new XFont("Verdana", 10, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(430, 504, 172, 20),
                        XStringFormats.Center);
                gfx.DrawString("ONLY",
                        new XFont("Verdana", 10, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(430, 514, 172, 20),
                        XStringFormats.Center);

                //Official use only stamp box
                gfx.DrawRectangle(new XPen(XColors.Black, 1), new XRect(430, 540, 172, 100));

                //signature box
                gfx.DrawRectangle(new XPen(XColors.Black, 1), new XRect(10, 640, page.Width - 20, 100));

                String signature_text =
                    "I declare that all statements and particulars contained in this General Declaration and in any supplementary forms required to be " +
                    "presented with this General Declaration are complete, exact and true to the best of my knowledge and that all through passengers " +
                    "will continue/have continued on the flight.";

                tf.DrawString(signature_text,
                        new XFont("Verdana", 8, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(12, 642, 500, 30),
                        XStringFormats.TopLeft);

                gfx.DrawString("Signature............................................................................",
                        new XFont("Verdana", 8, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(250, 700, 350, 10),
                        XStringFormats.TopLeft);

                gfx.DrawString("Authorized Agent or Pilot-In-Command",
                        new XFont("Verdana", 8, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(250, 710, 350, 10),
                        XStringFormats.Center);

                gfx.DrawString("Delete as necessary",
                        new XFont("Verdana", 8, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(250, 720, 350, 10),
                        XStringFormats.Center);


                gfx.DrawString("REMARKS:",
                        new XFont("Verdana", 8, XFontStyle.Bold),
                        XBrushes.Black,
                        new XRect(12, 755, 50, 10),
                        XStringFormats.TopLeft);

                String empty_line = "..................................................................................................................................................";

                gfx.DrawString(empty_line,
                        new XFont("Verdana", 8, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(65, 755, 500, 10),
                        XStringFormats.TopLeft);

                gfx.DrawString(empty_line,
                        new XFont("Verdana", 8, XFontStyle.Regular),
                        XBrushes.Black,
                        new XRect(65, 775, 500, 10),
                        XStringFormats.TopLeft);
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return document;

        }

        //To get access token
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
