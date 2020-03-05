using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// Summary description for SMSResponses
/// </summary>
[DataContract]
public class SMSResponses
{
    [DataMember]
    public string ResultCode { get; set; }
    [DataMember]
    public string ResultMessage { get; set; }
}