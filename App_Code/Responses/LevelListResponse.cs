using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for LevelLestResponse
/// </summary>
public class LevelListResponse:Response
{
    public List<Models.Level> levelList { get; set; }
}