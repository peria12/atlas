using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GlobalNetApps.AtlasFlightSchedule.Models
{
    [DataContract]
    public class AuthorizationClient
    {
        [DataMember]
        public String GrantType { get; set; }
        [DataMember]
        public String UserName { get; set; }
        [DataMember]
        public String Password { get; set; }
        [DataMember]
        public String ClientIdentification { get; set; }
        [DataMember]
        public String ClientSecret { get; set; }
    }
}