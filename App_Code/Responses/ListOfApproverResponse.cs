using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// Summary description for ListOfApproverResponse
/// </summary>

[DataContract]
public class ListOfApproverResponse:Response
{
    [DataMember]
    public List<Models.Admin> adminList { get; set; }
}