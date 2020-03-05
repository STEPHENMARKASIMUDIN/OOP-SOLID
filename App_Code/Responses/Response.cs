using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// Summary description for Response
/// </summary>

[DataContract]
public class Response : IResponse
{

    [DataMember]
    public int ResponseCode { get; set; }
    [DataMember]
    public string ResponsMessage { get; set; }



}