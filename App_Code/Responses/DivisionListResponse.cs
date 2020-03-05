using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

/// <summary>
/// Summary description for DivisionListResponse
/// </summary>
[DataContract]
public class DivisionListResponse:Response
{
    [DataMember]
    public List<Models.Division> divisionList { get; set; }
}