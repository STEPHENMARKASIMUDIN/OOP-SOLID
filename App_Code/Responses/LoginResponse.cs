using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// Summary description for LoginResponse
/// </summary>
[DataContract]
public class LoginResponse:Response
{
    [DataMember]
    public Models.PartnersData loginData { get; set; }
    [DataMember]
    public Models.Admin adminData { get; set; }
}