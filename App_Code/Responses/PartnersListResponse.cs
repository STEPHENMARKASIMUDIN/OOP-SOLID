using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PartnersListResponse
/// </summary>
public class PartnersListResponse:Response
{
    public List<Models.PartnersData> partnersList { get; set; }
}