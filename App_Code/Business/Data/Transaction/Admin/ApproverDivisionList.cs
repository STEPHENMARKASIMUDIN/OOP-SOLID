using Dapper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


/// <summary>
/// Summary description for ApproverDivisionList
/// </summary>
public class ApproverDivisionList
{
    private readonly ILog _Logger = LogManager.GetLogger(typeof(ApproverDivisionList));
    public ApproverDivisionList()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public virtual DivisionListResponse DivisionList(MySqlConnection Connection) 
    {
        try
        {

            List<Models.Division> divisionList = Connection.Query<Models.Division>(StoredProcedures.DIVISION_LIST, null, null, false, 60, CommandType.StoredProcedure).ToList();
            if (divisionList.Count < 1)
            {
                _Logger.Info("No data found!");
                return new DivisionListResponse { ResponseCode = 404, ResponsMessage = "No data found." };
            }

            return new DivisionListResponse { ResponseCode = 200, ResponsMessage = "Success", divisionList = divisionList };
        }
        catch (MySqlException mex)
        {
            _Logger.Fatal(mex.ToString());
            return new DivisionListResponse { ResponseCode = 400, ResponsMessage = "Unable to process request. Please check your data and try again." };
        }
        catch (TimeoutException tex)
        {
            _Logger.Fatal(tex.ToString());
            return new DivisionListResponse { ResponseCode = 408, ResponsMessage = "Unable to process request. Connection timeout occured. Please try again later." };
        }
        catch (Exception ex)
        {
            _Logger.Fatal(ex.ToString());
            return new DivisionListResponse { ResponseCode = 500, ResponsMessage = "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience." };
        }
    }
}