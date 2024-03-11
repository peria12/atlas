// <copyright file="HotelViewModel.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>ChennakesavaRao</author>
// </copyright>
// <created Date> 05/20/2016 </created Date>
// <summary>Hotel DAO</summary>

namespace GlobalNetApps.AtlasFlightSchedule.Models
{
    using System;
    public class HotelViewModel
    {
        public int Supplier { get; set; }
        public int CrewID { get; set; }
        public string CrewName { get; set; }
        public string SupplierStn { get; set; }
        public string Name { get; set; }
        public string Night { get; set; }
        public DateTime LocalCheckInDtTm { get; set; }
        public DateTime LocalCheckOutDtTm { get; set; }
        public string Status { get; set; }
        public string ReservationNumber { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mail { get; set; }
        public int FltSeqNo { get; set; }
    }
}