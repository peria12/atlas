using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace GlobalNetApps.AtlasFlightSchedule.Models
{
    [DataContract]
    public class AuthorizationToken
    {
        [DataMember]
        public String AccessTokenStr { get; set; }
        [DataMember]
        public String TokenType { get; set; }
        [DataMember]
        public Int32 ExpiresIn { get; set; }
        [DataMember]
        public String RefreshToken { get; set; }
        [DataMember]
        public String ClientIdentification { get; set; }
        [DataMember]
        public String UserName { get; set; }
        [DataMember]
        public Int32 User_ID { get; set; }
        [DataMember]
        public DateTime AccessTokenIssued { get; set; }
        [DataMember]
        public DateTime AccessTokenExpires { get; set; }
    }
}