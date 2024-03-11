// <copyright file="FlightViewModel.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>ChennakesavaRao</author>
// </copyright>
// <created Date> 05/20/2016 </created Date>
// <summary>Flight DAO</summary>

namespace GlobalNetApps.AtlasFlightSchedule.Models
{
    public class FlightViewModel
    {
        public int FltSeqNo { get; set; }
        public string FltCustomer { get; set; }
        public string AirlineCode { get; set; }
        public int FltNo { get; set; }
        public string AircraftTailNo { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public string OrigDepartureDate { get; set; }
        public string OrigDepartureTime { get; set; }
        public string FltETDDate { get; set; }
        public string FltETDTime { get; set; }
        public string FltOutDate { get; set; }
        public string FltOutTime { get; set; }
        public string OrigArrivalDate { get; set; }
        public string OrigArrivalTime { get; set; }
        public string FltETADate { get; set; }
        public string FltETATime { get; set; }
        public string FltInDate { get; set; }
        public string FltInTime { get; set; }

        public int NextFltSeqNo { get; set; }
        public int PriorFltSeqNo { get; set; }
        public string FltDelayCode { get; set; }
        public string FltDelayAmounts { get; set; }
        public string DelayType { get; set; }
        public string Description { get; set; }
        public int DelaySeqno { get; set; }
        public string ARRDelayCode { get; set; }
        public string ARRDelayAmt { get; set; }
        public int FltStatus { get; set; }
        public string FltOffDt { get; set; }
        public string FltOnDt { get; set; }
        public string FltOffTm { get; set; }
        public string FltOnTm { get; set; }
        public string CustomerName { get; set; }
        public string CustRemarks { get; set; }
        public string FltType { get; set; }
        public string Mission { get; set; }
        public int PlanPyld { get; set; }
        public int ActualPayLoad { get; set; }
        public int MaxAvailPyld { get; set; }
        public int TaxiFuelPlan { get; set; }
        public int FuelBlockOut { get; set; }
        public int PlanBurn { get; set; }
        public int PlanFuelRem { get; set; }
        public int FuelAtLanding { get; set; }
        public int FuelAtBlockIn { get; set; }
        public int FuelUpLiftUSGal { get; set; }
        public string CMNameCA { get; set; }
        public string CMNameFO { get; set; }
        public string CMNameFE { get; set; }
        public int JSCount { get; set; }
        public string Jseat1 { get; set; }
        public string Jseat2 { get; set; }
        public string Jseat3 { get; set; }
        public string Jseat4 { get; set; }
        public string Jseat5 { get; set; }
        public string Jseat6 { get; set; }
        
        public string Jseat7 { get; set; }
        public string Jseat8 { get; set; }
        public string Jseat9 { get; set; }
        public string Jseat10 { get; set; }
        public string Jseat11 { get; set; }
        public string Jseat12 { get; set; }
        public string CMNamePU { get; set; }
        public string CMNameAP { get; set; }
        public string CMNameFA { get; set; }
        public string Al { get; set; }
        public string AircraftRegistration { get; set; }
    }
}