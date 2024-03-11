// <copyright file="ViewModel.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>ChennakesavaRao</author>
// </copyright>
// <created Date> 05/10/2016 </created Date>
// <summary>to pass multiple data sets to view </summary>

namespace GlobalNetApps.AtlasFlightSchedule.Models
{
    using System.Collections.Generic;
    public class ViewModel
    {
        public LookupViewModel LookupModel { get; set; }
        public IEnumerable<FlightViewModel> FlightViewModel { get; set; }
    }
}