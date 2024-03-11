// <copyright file="LOFViewModel.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>ChennakesavaRao</author>
// </copyright>
// <created Date> 05/10/2016 </created Date>
// <summary>to validate the schedule form</summary>

namespace GlobalNetApps.AtlasFlightSchedule.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class LookupViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Enter the station code")]
        public string Station { get; set; }
        public bool IsInbound { get; set; }
    }
}