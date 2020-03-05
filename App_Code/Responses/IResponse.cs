using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

/// <summary>
/// Summary description for IResponse
/// </summary>
/// 


public interface IResponse
{
    int ResponseCode { get; set; }
    string ResponsMessage { get; set; }

}