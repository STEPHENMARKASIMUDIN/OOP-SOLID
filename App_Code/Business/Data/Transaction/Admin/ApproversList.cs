using Dapper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

/// <summary>
/// Summary description for ApproversList
/// </summary>
public class ApproversList
{
    private readonly ILog _Logger = LogManager.GetLogger(typeof(ApproversList));
    public ApproversList()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public virtual ListOfApproverResponse ApproverList(MySqlConnection Connection, IModel Model) 
    {
        try
        {

            List<Models.Admin> approverList = Connection.Query<Models.Admin>(StoredProcedures.APPROVER_LIST, null, null, false, 60, CommandType.StoredProcedure).ToList();
            if (approverList.Count < 1)
            {
                _Logger.Info("No data found!");
                return new ListOfApproverResponse { ResponseCode = 404, ResponsMessage = "No data found." };
            }

            return new ListOfApproverResponse { ResponseCode = 200, ResponsMessage = "Success", adminList = approverList };
        }
        catch (MySqlException mex)
        {
            _Logger.Fatal(mex.ToString());
            return new ListOfApproverResponse { ResponseCode = 400, ResponsMessage = "Unable to process request. Please check your data and try again." };
        }
        catch (TimeoutException tex)
        {
            _Logger.Fatal(tex.ToString());
            return new ListOfApproverResponse { ResponseCode = 408, ResponsMessage = "Unable to process request. Connection timeout occured. Please try again later." };
        }
        catch (Exception ex)
        {
            _Logger.Fatal(ex.ToString());
            return new ListOfApproverResponse { ResponseCode = 500, ResponsMessage = "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience." };
        }
    }
}