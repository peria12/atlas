// <copyright file="FlightServices.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>ChennakesavaRao</author>
// </copyright>
// <created Date> 05/20/2016 </created Date>
// <summary>Flight services  </summary>

namespace GlobalNetApps.AtlasFlightSchedule.Services
{
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using GlobalNetApps.AtlasFlightSchedule.RestClient;
    using Microsoft.Security.Application;
    using System.Web.Mvc;
    using GlobalNetApps.AtlasFlightSchedule.APIClient;
    using GlobalNetApps.AtlasFlightSchedule.DAL.Entity;
    using GlobalNetApps.AtlasFlightSchedule.DAL.Repository.Implementation;
    using Dapper;
    using System.Linq;

    public class FlightServices : IFlightServices
    {
        /// <summary>
        /// Create Log object instance and use where ever required to log info ,exceptions and etc..
        /// </summary>
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(FlightServices));

        public readonly string companyCode = ConfigurationManager.AppSettings["CompanyCode"];
        public readonly string companyCodeH = ConfigurationManager.AppSettings["CompanyCodeH"];
        private FlightRepository<Flight> flightsView = null;
        /// <summary>
        /// to get the flights data
        /// </summary>
        /// <param name="lModel"></param>
        /// <returns></returns>
        public List<FlightViewModel> GetFlights(LookupViewModel lModel, Controller mvcContrllr, string accessToken)
        {
             List<FlightViewModel> lstFlights = new List<FlightViewModel>();
            List<Flight> lstAirInfo = new List<Flight>();
            Dictionary<string, string> param = new Dictionary<string, string>();
            this.flightsView = new FlightRepository<Flight>();
            var parameters = new DynamicParameters();
            try
            {
                parameters.Add("Company", companyCode);
                parameters.Add("StartTime", lModel.StartDate.ToString("g"));
                parameters.Add("EndTime", lModel.EndDate.ToString("g"));
                parameters.Add("Station", Sanitizer.GetSafeHtmlFragment(lModel.Station));
                parameters.Add("Direction", lModel.IsInbound ? "arrival" : "departure");
                //lstFlights = client.GetAsyncWithAutoTokenRefresh<FlightViewModel>(mvcContrllr, relativePath, param);
                //lstFlights = RestClientService.Instance.GetListItems<FlightViewModel>(relativePath, param);
                lstAirInfo = this.flightsView.ExecuteStoredProcedure("UDSP_GetFlightList", parameters).ToList();
                lstFlights = lstAirInfo.Select(a => new FlightViewModel { FltSeqNo = a. flt_seq_no, AircraftRegistration = a.AircraftRegistration, FltCustomer = a.flt_customer,
                    AirlineCode = a.airline_code, FltNo = a.flt_no, AircraftTailNo = a.aircraft_tail_no, DepartureStation = a.dept_station, ArrivalStation = a.arrive_stat,
                    OrigDepartureDate = a.orig_dept_dt, OrigDepartureTime = a.orig_dept_tm, FltETDDate = a.flt_etd_dt, FltETDTime = a.flt_etd_tm, FltOutDate =a.flt_out_dt,
                    FltOutTime =a.flt_out_tm, OrigArrivalDate =a.orig_arrive_dt, OrigArrivalTime =a.orig_arrive_tm, FltETADate =a.flt_eta_dt, FltETATime = a.flt_eta_tm,
                    FltInDate = a.flt_eta_dt, FltInTime = a.flt_in_tm, NextFltSeqNo = a.nextfltseqno, PriorFltSeqNo =a.priorfltseqno, FltDelayCode = a.flt_delay_code,
                    FltDelayAmounts = a.flt_delay_amounts, DelayType = a.delaytype, Description =a.description, DelaySeqno =a.delay_seq_no, ARRDelayCode =a.ARR_delay_code,
                    ARRDelayAmt =a.ARR_delay_amt, FltStatus =a.flt_status, FltOffDt =a.Flt_Off_dt, FltOnDt = a.flt_on_dt, FltOffTm =a.Flt_Off_tm, FltOnTm = a.flt_on_tm,
                    CustomerName = a.customer_name, CustRemarks =a.CustRemcust_remarksarks, FltType =a.flt_type, Mission =a.mission, PlanPyld =a.plan_pyld, ActualPayLoad = a.actual_payload,
                    MaxAvailPyld =a.max_avail_pyld, TaxiFuelPlan = a.taxi_fuel_plan, FuelBlockOut = a.fuel_blockout, PlanBurn = a.plan_burn, PlanFuelRem =a.Plan_Fuel_Rem,
                    FuelAtLanding = a.fuel_at_landing, FuelAtBlockIn = a.fuel_at_Blockin, FuelUpLiftUSGal =a.fuel_uplift_US_Gal, CMNameCA =a.cm_nameCA, CMNameFO = a.cm_nameFO,
                    CMNameFE =a.cm_nameFE, JSCount =a.JSCount, Jseat1 =a.jseat1, Jseat2 =a.jseat2, Jseat3 =a.jseat3, Jseat4 =a.jseat4, Jseat5 =a.jseat5, Jseat6 = a.jseat6,
                    Jseat7 =a.Jseat7, Jseat8 =a.Jseat8, Jseat9 =a.Jseat9, Jseat10 = a.Jseat10,  Jseat11 = a.Jseat11, Jseat12 =a.Jseat12, CMNamePU =a.CMNamePU, CMNameAP =a.CMNameAP,
                    CMNameFA = a.CMNameFA, Al =a.Al}).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return lstFlights;
        }
        /// <summary>
        /// to get the flight detail by flight seqno
        /// </summary>
        /// <param name="FltSeqNo"></param>
        /// <returns></returns>
        public List<FlightViewModel> GetFlightDetails(string FltSeqNo, Controller mvcContrllr, string accessToken)
        {
            List<Flight> lstAirInfo = new List<Flight>();
            List<FlightViewModel> lstFlights = new List<FlightViewModel>();
            Dictionary<string, string> param = new Dictionary<string, string>();
            //APIClientHandler client = new APIClientHandler(accessToken);
            this.flightsView = new FlightRepository<Flight>();
            var parameters = new DynamicParameters();
            try
            {
                parameters.Add("Company", companyCode);
                parameters.Add("flt_seq_no", Sanitizer.GetSafeHtmlFragment(FltSeqNo));
                lstAirInfo = this.flightsView.ExecuteStoredProcedure("UDSP_GetFlightDetails", parameters).ToList();
                //lstFlights = client.GetAsyncWithAutoTokenRefresh<FlightViewModel>(mvcContrllr, relativePath, param);
                //lstFlights = RestClientService.Instance.GetListItems<FlightViewModel>(relativePath, param);
                lstFlights = lstAirInfo.Select(a => new FlightViewModel
                {
                    FltSeqNo = a.flt_seq_no,
                    AircraftRegistration = a.AircraftRegistration,
                    FltCustomer = a.flt_customer,
                    AirlineCode = a.airline_code,
                    FltNo = a.flt_no,
                    AircraftTailNo = a.aircraft_tail_no,
                    DepartureStation = a.dept_station,
                    ArrivalStation = a.arrive_stat,
                    OrigDepartureDate = a.orig_dept_dt,
                    OrigDepartureTime = a.orig_dept_tm,
                    FltETDDate = a.flt_etd_dt,
                    FltETDTime = a.flt_etd_tm,
                    FltOutDate = a.flt_out_dt,
                    FltOutTime = a.flt_out_tm,
                    OrigArrivalDate = a.orig_arrive_dt,
                    OrigArrivalTime = a.orig_arrive_tm,
                    FltETADate = a.flt_eta_dt,
                    FltETATime = a.flt_eta_tm,
                    FltInDate = a.flt_eta_dt,
                    FltInTime = a.flt_in_tm,
                    NextFltSeqNo = a.nextfltseqno,
                    PriorFltSeqNo = a.priorfltseqno,
                    FltDelayCode = a.flt_delay_code,
                    FltDelayAmounts = a.flt_delay_amounts,
                    DelayType = a.delaytype,
                    Description = a.description,
                    DelaySeqno = a.delay_seq_no,
                    ARRDelayCode = a.ARR_delay_code,
                    ARRDelayAmt = a.ARR_delay_amt,
                    FltStatus = a.flt_status,
                    FltOffDt = a.Flt_Off_dt,
                    FltOnDt = a.flt_on_dt,
                    FltOffTm = a.Flt_Off_tm,
                    FltOnTm = a.flt_on_tm,
                    CustomerName = a.customer_name,
                    CustRemarks = a.CustRemcust_remarksarks,
                    FltType = a.flt_type,
                    Mission = a.mission,
                    PlanPyld = a.plan_pyld,
                    ActualPayLoad = a.actual_payload,
                    MaxAvailPyld = a.max_avail_pyld,
                    TaxiFuelPlan = a.taxi_fuel_plan,
                    FuelBlockOut = a.fuel_blockout,
                    PlanBurn = a.plan_burn,
                    PlanFuelRem = a.Plan_Fuel_Rem,
                    FuelAtLanding = a.fuel_at_landing,
                    FuelAtBlockIn = a.fuel_at_Blockin,
                    FuelUpLiftUSGal = a.fuel_uplift_US_Gal,
                    CMNameCA = a.cm_nameCA,
                    CMNameFO = a.cm_nameFO,
                    CMNameFE = a.cm_nameFE,
                    JSCount = a.JSCount,
                    Jseat1 = a.jseat1,
                    Jseat2 = a.jseat2,
                    Jseat3 = a.jseat3,
                    Jseat4 = a.jseat4,
                    Jseat5 = a.jseat5,
                    Jseat6 = a.jseat6,
                    Jseat7 = a.Jseat7,
                    Jseat8 = a.Jseat8,
                    Jseat9 = a.Jseat9,
                    Jseat10 = a.Jseat10,
                    Jseat11 = a.Jseat11,
                    Jseat12 = a.Jseat12,
                    CMNamePU = a.CMNamePU,
                    CMNameAP = a.CMNameAP,
                    CMNameFA = a.CMNameFA,
                    Al = a.Al
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return lstFlights;
        }
        /// <summary>
        /// to get the hotel info by flight seqno
        /// </summary>
        /// <param name="FltSeqNo"></param>
        /// <returns></returns>
        public List<HotelViewModel> GetHotelsBySeqNo(int FltSeqNo, Controller mvcContrllr, string accessToken)
        {
            string relativePath = "api/hotelflight/GetHotelFlightInformation";
            Dictionary<string, string> param = new Dictionary<string, string>();
            List<HotelViewModel> lstHotel = new List<HotelViewModel>();
            APIClientHandler client = new APIClientHandler(accessToken);
            try
            {
                param.Add("CompanyCode", companyCodeH);
                param.Add("ParameterName", "FltSeqNo");
                param.Add("ParameterValue", Sanitizer.GetSafeHtmlFragment(FltSeqNo.ToString()));
                lstHotel = client.GetAsyncWithAutoTokenRefresh<HotelViewModel>(mvcContrllr, relativePath, param);
                //lstHotel = RestClientService.Instance.GetListItems<HotelViewModel>(relativePath, param);
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return lstHotel;
        }
        /// <summary>
        /// to get the hotels list by station
        /// </summary>
        /// <param name="Station"></param>
        /// <returns></returns>
        public List<HotelViewModel> GetHotelsByStation(string Station, Controller mvcContrllr, string accessToken)
        {
            string relativePath = "api/hotelflight/GetHotelFlightInformation";
            Dictionary<string, string> param = new Dictionary<string, string>();
            List<HotelViewModel> lstHotel = new List<HotelViewModel>();
            APIClientHandler client = new APIClientHandler(accessToken);
            try
            {
                param.Add("CompanyCode", companyCodeH);
                param.Add("ParameterName", "Station");
                param.Add("ParameterValue", Sanitizer.GetSafeHtmlFragment(Station));
                lstHotel = client.GetAsyncWithAutoTokenRefresh<HotelViewModel>(mvcContrllr, relativePath, param);
                //lstHotel = RestClientService.Instance.GetListItems<HotelViewModel>(relativePath, param);
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return lstHotel;
        }
        /// <summary>
        /// to get the flight route data by seq no
        /// </summary>
        /// <param name="FltSeqNo"></param>
        /// <returns></returns>
        public List<FlightViewModel> GetFlightRouteData(string FltSeqNo, Controller mcContrllr, string accessToken)
        {
            string relativePath = "api/flight/FlightRouteData";
            Dictionary<string, string> param = new Dictionary<string, string>();
            List<FlightViewModel> lstFlights = new List<FlightViewModel>();
            APIClientHandler client = new APIClientHandler(accessToken);
            try
            {
                param.Add("CompanyCode", companyCode);
                param.Add("FlightSequenceNo", Sanitizer.GetSafeHtmlFragment(FltSeqNo));
               // lstFlights = client.GetAsyncWithAutoTokenRefresh<FlightViewModel>(mvcContrllr, relativePath, param);
                //lstFlights = RestClientService.Instance.GetListItems<FlightViewModel>(relativePath, param);
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
            return lstFlights;
        }

    }
}