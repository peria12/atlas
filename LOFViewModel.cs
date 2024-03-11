// <copyright file="LOFViewModel.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>ChennakesavaRao</author>
// </copyright>
// <created Date> 05/10/2016 </created Date>
// <summary>LOF DAO</summary>

namespace GlobalNetApps.AtlasFlightSchedule.Models
{
    public class LOFViewModel
    {
        public string AircraftRegistration { get; set; }
        public string FltNo { get; set; }
        public int? FltSeqNo { get; set; }
        public string DeptStation { get; set; }
        public string ArrivalStation { get; set; }
        public string GroundTime { get; set; }
        public string DeptTime { get; set; }
        public string ArrivalTime { get; set; }
        public string FltStatus { get; set; }
    }
}