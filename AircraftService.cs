// <copyright file="ODSServices.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>ChennakesavaRao</author>
// </copyright>
// <created Date> 04/18/2016 </created Date>
// <summary>ODS services  </summary>

namespace GlobalNetApps.AtlasFlightSchedule.Services
{
    using GlobalNetApps.AtlasFlightSchedule.APIClient;
    using GlobalNetApps.AtlasFlightSchedule.RestClient;
    using Interfaces;
    using Microsoft.Security.Application;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Mvc;
    using GlobalNetApps.AtlasFlightSchedule.DAL;
    using GlobalNetApps.AtlasFlightSchedule.DAL.Entity;
    using GlobalNetApps.AtlasFlightSchedule.DAL.Repository.Implementation;
    using Dapper;
    using System.Linq;

    public class AircraftService : IAircraftService
    {
        /// <summary>
        /// Create Log object instance and use where ever required to log info ,exceptions and etc..
        /// </summary>
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(FlightServices));
        public readonly string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
        public readonly int ThresholdDays = Convert.ToInt32(ConfigurationManager.AppSettings["ThresholdDays"]);
        private FlightRepository<AircraftRegistrationInfo> flightLOFView = null;
        private FlightRepository<LOFViewModelNew> flightLOFView1 = null;

        /// <summary>
        /// to get the all aircrft reno
        /// </summary>
        /// <returns></returns>
        public List<LOFViewModel> GetAircraftRegNo(Controller mvcContrllr)
        {
            List<LOFViewModel> lstTailNo = new List<LOFViewModel>();
            List<AircraftRegistrationInfo> lstAirInfo = new List<AircraftRegistrationInfo>();
            //string relativePath = "api/flight/AircraftRegistration";
            Dictionary<string, string> param = new Dictionary<string, string>();
            // APIClientHandler client = new APIClientHandler(accessToken);
            this.flightLOFView = new FlightRepository<AircraftRegistrationInfo>();
            var parameters = new DynamicParameters();
            try
            {
                parameters.Add("company", CompanyCode);
                lstAirInfo = this.flightLOFView.ExecuteStoredProcedure("udsp_getaircraftregistration", parameters).ToList();
                //lstTailNo = client.GetAsyncWithAutoTokenRefresh<LOFViewModel>(mvcContrllr, relativePath, param);
                //lstTailNo = RestClientService.Instance.GetListItems<LOFViewModel>(relativePath, param);
                lstTailNo = lstAirInfo.Select(a => new LOFViewModel { AircraftRegistration = a.aircraft_registration }).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return lstTailNo;
        }
        /// <summary>
        /// to get the aircrft data by regno
        /// </summary>
        /// <param name="AircraftRegNo"></param>
        /// <returns></returns>
        public List<LOFViewModel> GetAircrafts(Controller mvcContrllr, string AircraftRegNo, string accesstoken)
        {
            List<LOFViewModel> lstTailNo = new List<LOFViewModel>();
            List<LOFViewModelNew> lstAirInfo = new List<LOFViewModelNew>();
            //string relativePath = "api/flight/LofAtlasPolar";
            DateTime StartDate = DateTime.Now.AddDays(-ThresholdDays);
            DateTime EndDate = DateTime.Now.AddDays(ThresholdDays);
            Dictionary<string, string> param = new Dictionary<string, string>();
            // APIClientHandler client = new APIClientHandler(accessToken);
            this.flightLOFView1 = new FlightRepository<LOFViewModelNew>();
            var parameters = new DynamicParameters();
            try
            {
                parameters.Add("Company", CompanyCode);
                parameters.Add("AircraftRegistration", Sanitizer.GetSafeHtmlFragment(AircraftRegNo));
                parameters.Add("StartDate", StartDate.ToString());
                parameters.Add("EndDate", EndDate.ToString());
                lstAirInfo = this.flightLOFView1.ExecuteStoredProcedure("udsp_LOFInfo_FS", parameters).ToList();
                //lstTailNo = client.GetAsyncWithAutoTokenRefresh<LOFViewModel>(mvcContrllr, relativePath, param);
                //lstTailNo = RestClientService.Instance.GetListItems<LOFViewModel>(relativePath, param);
                lstTailNo = lstAirInfo.Select(a => new LOFViewModel
                {
                    AircraftRegistration = a.aircraft_registration,
                    FltNo = a.flt_no,
                    FltSeqNo = a.flt_seq_no,
                    DeptStation = a.dept_station,
                    ArrivalStation = a.arrive_stat,
                    GroundTime = a.GroundTm,
                    DeptTime = a.DeptTime,
                    ArrivalTime = a.arrivetime,
                    FltStatus = a.Flt_status
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return lstTailNo;
        }
    }
}