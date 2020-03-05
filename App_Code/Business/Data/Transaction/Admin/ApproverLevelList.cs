using Dapper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
/// <summary>
/// Summary description for ApproverLevelList
/// </summary>
public class ApproverLevelList
{
    private readonly ILog _Logger = LogManager.GetLogger(typeof(ApproverLevelList));
    public ApproverLevelList()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public virtual LevelListResponse LevelList(MySqlConnection Connection) 
    {
        try
        {
            List<Models.Level> levelList = Connection.Query<Models.Level>(StoredProcedures.LEVEL_LIST, null, null, false, 60, CommandType.StoredProcedure).ToList();
            if (levelList.Count < 1)
            {
                _Logger.Info("No Data Found");
                return new LevelListResponse { ResponseCode = 404, ResponsMessage = "No Data Found" };
            }
            return new LevelListResponse { ResponseCode = 200, ResponsMessage = "Success", levelList = levelList };
        }
        catch (MySqlException mex)
        {
            _Logger.Fatal(mex.ToString());
            return new LevelListResponse { ResponseCode = 400, ResponsMessage = "Unable to process request. Please check your data and try again." };
        }
        catch (TimeoutException tex)
        {
            _Logger.Fatal(tex.ToString());
            return new LevelListResponse { ResponseCode = 408, ResponsMessage = "Unable to process request. Connection timeout occured. Please try again later." };
        }
        catch (Exception ex)
        {
            _Logger.Fatal(ex.ToString());
            return new LevelListResponse { ResponseCode = 500, ResponsMessage = "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience." };
        }
    }
}