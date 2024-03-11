// <copyright file="FlightHotelViewModel.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>ChennakesavaRao</author>
// </copyright>
// <created Date> 05/20/2016 </created Date>
// <summary>to pass multiple data sets to view </summary>

namespace GlobalNetApps.AtlasFlightSchedule.Models
{
    using System.Collections.Generic;

    public class FlightHotelViewModel
    {
        public IEnumerable<FlightViewModel> FlightViewModel { get; set; }
        public IEnumerable<HotelViewModel> HotelViewModel { get; set; }
    }
}